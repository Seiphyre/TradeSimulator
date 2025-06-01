using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for OrderBookWindow.xaml
    /// </summary>
    public partial class OrderBookWindow : Window
    {
        private TradeService TradeService;
        private Ticker Ticker;
        private OrderBook OrderBook;

        public OrderBookWindow(TradeService tradeService, OrderBook orderBook, Ticker ticker)
        {
            TradeService = tradeService;
            Ticker = ticker;
            OrderBook = orderBook;

            InitializeComponent();

            OrdersListView.ItemsSource = Ticker.Orders;
            TickerTextBox.Text = $"Ticker: {Ticker.DisplayName}";
            Title = $"Order book ({Ticker.DisplayName})";
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (TradeService != null && TradeService.IsConnected)
                await TradeService.OpenOrderBook(OrderBook.Id);
        }

        protected async override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (TradeService != null && TradeService.IsConnected)
                await TradeService.CloseOrderBook(OrderBook.Id);
        }

        private async void TradeRowBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Order order = ((FrameworkElement)sender).DataContext as Order;

            if (TradeService != null && TradeService.IsConnected)
            {
                await TradeService.CreateTransaction(OrderBook.BrokerId, Ticker.DisplayName, order.Price, order.Quantity, order.TransactionType);

                Ticker.Orders.Remove(order);

                OrdersListView.ItemsSource = null;
                OrdersListView.ItemsSource = Ticker.Orders;
            }
        }
    }
}
