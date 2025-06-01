using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for OrderBookWindow.xaml
    /// </summary>
    public partial class OrderBookWindow : Window
    {
        private TradeService _tradeService;
        private Ticker _ticker;
        private OrderBook _orderBook;
        private ObservableCollection<Order> _orders;



        /* -------------------------------------------------------------- */

        public OrderBookWindow(TradeService tradeService, OrderBook orderBook, Ticker ticker)
        {
            _tradeService = tradeService;
            _ticker = ticker;
            _orderBook = orderBook;

            var orders = _ticker.Orders.OrderBy(o => o.TransactionType).ThenBy(o => o.Price);
            _orders = new ObservableCollection<Order>(orders);

            InitializeComponent();

            OrdersListView.ItemsSource = _orders;
            TickerTextBox.Text = $"Ticker: {_ticker.DisplayName}";
            Title = $"Order book ({_ticker.DisplayName})";
        }



        /* -------------------------------------------------------------- */

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_tradeService != null && _tradeService.IsConnected)
                await _tradeService.OpenOrderBook(_orderBook.Id);
        }

        protected async override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_tradeService != null && _tradeService.IsConnected)
                await _tradeService.CloseOrderBook(_orderBook.Id);
        }



        /* -------------------------------------------------------------- */

        private async void TradeRowBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Order order = ((FrameworkElement)sender).DataContext as Order;

            if (_tradeService != null && _tradeService.IsConnected)
            {
                await _tradeService.CreateTransaction(_orderBook.BrokerId, _ticker.DisplayName, order.Price, order.Quantity, order.TransactionType);

                _orders.Remove(order);
            }
        }

        /* ----------------------------------------------------- */

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
