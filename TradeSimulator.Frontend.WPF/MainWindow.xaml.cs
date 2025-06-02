using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private TradeService _tradeService = new TradeService();

        private string _brokerId;
        private Broker _broker;
        private List<Ticker> _tickers = new();
        private ObservableCollection<OrderBook> _orderBooks = new();
        private ObservableCollection<Transaction> _transactions = new();

        private ReconnectingDialog _reconnectingDialog = null;



        /* ----------------------------------------------------------------- */

        public MainWindow()
        {
            InitializeComponent();

            ConnectBtn.Visibility = Visibility.Visible;
            DisconnectBtn.Visibility = Visibility.Collapsed;
            BrokerIdTextBox.IsEnabled = true;
            Dashboard.Visibility = Visibility.Collapsed;

            BrokerIdTextBox.Text = GenerateRandomBrokerId();
            UrlTextBox.Text = "http://localhost:5038";

            _tradeService.OnCreatedOrderBook += TradeHub_OnCreateOrderBook;
            _tradeService.OnDeletedOrderBook += TradeHub_OnDeleteOrderBook;
            _tradeService.OnCreatedTransaction += TradeHub_OnCreatedTransaction;
            _tradeService.OnReconnecting += TradeService_OnReconnecting;
            _tradeService.OnReconnected += TradeService_OnReconnected;
        }

        public void Dispose()
        {
            _tradeService.OnCreatedOrderBook -= TradeHub_OnCreateOrderBook;
            _tradeService.OnDeletedOrderBook -= TradeHub_OnDeleteOrderBook;
            _tradeService.OnCreatedTransaction -= TradeHub_OnCreatedTransaction;
            _tradeService.OnReconnecting -= TradeService_OnReconnecting;
            _tradeService.OnReconnected -= TradeService_OnReconnected;
        }



        /* ----------------------------------------------------------------- */

        private void CloseAllSubWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                var window = App.Current.Windows[intCounter];

                if (window == this)
                    continue;

                window.Close();
            }
        }



        /* ----------------------------------------------------------------- */

        private async Task<bool> Connect()
        {
            ConnectBtn.IsEnabled = false;
            ConnectBtn.Content = "Connecting...";

            try
            {
                string url = UrlTextBox.Text;

                if (url != null && !url.EndsWith('/'))
                    url += '/';

                url = Path.Combine(url, "trade-hub");

                await _tradeService.Connect(url, BrokerIdTextBox.Text);

                DisconnectBtn.Visibility = Visibility.Visible;
                ConnectBtn.Visibility = Visibility.Collapsed;
                BrokerIdTextBox.IsEnabled = false;
                UrlTextBox.IsEnabled = false;
                Dashboard.Visibility = Visibility.Visible;

                _brokerId = BrokerIdTextBox.Text;

                return true;
            }
            catch
            {
                MessageBox.Show("Failed to connect to server.", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            finally
            {
                ConnectBtn.IsEnabled = true;
                ConnectBtn.Content = "Connect";
            }
        }

        private async Task Disconnect()
        {
            DisconnectBtn.IsEnabled = false;
            DisconnectBtn.Content = "Disconnecting...";

            try
            {
                await _tradeService.Disconnect();

                ConnectBtn.Visibility = Visibility.Visible;
                DisconnectBtn.Visibility = Visibility.Collapsed;
                BrokerIdTextBox.IsEnabled = true;
                UrlTextBox.IsEnabled = true;
                Dashboard.Visibility = Visibility.Collapsed;

                CloseAllSubWindows();

                Console.WriteLine("Disconnected.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to disconnect.");
            }
            finally
            {
                DisconnectBtn.IsEnabled = true;
                DisconnectBtn.Content = "Disconnect";
            }
        }

        private async Task RefreshData()
        {
            await GetOrCreateBroker();

            var tickers = await FetchTickers();
            SetTickers(tickers);

            var orderBooks = await FetchOrderBooks();
            SetOrderBooks(orderBooks);

            var transactions = await FetchTransactions();
            SetTransactions(transactions);
        }



        /* ---------------------------------------------------------- */

        private async Task GetOrCreateBroker()
        {
            _broker = await _tradeService.GetOrCreateBroker(_brokerId);
        }

        private string GenerateRandomBrokerId()
        {
            var rand = new Random();
            int tag = rand.Next(1000, 9999);

            return $"broker#{tag}";
        }


        /* ---------------------------------------------------------- */

        private async Task<List<Ticker>> FetchTickers()
        {
            var tickers = await _tradeService.GetTickers();

            return tickers;
        }

        private void SetTickers(List<Ticker> tickers)
        {
            _tickers = tickers;
        }



        /* ---------------------------------------------------------- */

        private async Task<List<OrderBook>> FetchOrderBooks()
        {
            var orderBooks = await _tradeService.GetOrderBooks(_brokerId);

            return orderBooks;
        }

        private async Task<OrderBook> CreateOrderBook(string tickerId)
        {
            var orderBook = await _tradeService.CreateOrderBook(_brokerId, tickerId);

            return orderBook;
        }

        private async Task DeleteOrderBook(OrderBook orderBook)
        {
            await _tradeService.DeleteOrderBook(orderBook.Id);
        }

        private void SetOrderBooks(List<OrderBook> orderBooks)
        {
            _orderBooks = new ObservableCollection<OrderBook>(orderBooks);
            OrderBooksListView.ItemsSource = _orderBooks;

            bool isVisibile = _orderBooks == null || _orderBooks.Count == 0;
            OrderBooksListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AddOrderBook(OrderBook newOrderBook)
        {
            if (_orderBooks == null)
            {
                _orderBooks = new ObservableCollection<OrderBook>();
                OrderBooksListView.ItemsSource = _orderBooks;
            }

            if (!_orderBooks.Any(orderBook => orderBook.Id == newOrderBook.Id))
            {
                _orderBooks.Add(newOrderBook);

                OrderBooksListView.ItemsSource = null;
                OrderBooksListView.ItemsSource = _orderBooks;
                OrderBooksListView.Visibility = Visibility.Visible;
            }
        }

        private void RemoveOrderBook(string orderBookId)
        {
            if (_orderBooks == null)
                return;

            var orderbook = _orderBooks.FirstOrDefault(ob => ob.Id == orderBookId);

            if (orderbook != null)
            {
                _orderBooks.Remove(orderbook);

                bool isVisibile = _orderBooks == null || _orderBooks.Count == 0;
                OrderBooksListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void OpenOrderBookWindow(OrderBook orderBook)
        {
            var orderBookTicker = _tickers.FirstOrDefault(ticker => ticker.Id == orderBook.TickerId);

            var window = new OrderBookWindow(_tradeService, orderBook, orderBookTicker);

            window.Show();
        }



        /* ---------------------------------------------------------- */

        private async Task<List<Transaction>> FetchTransactions()
        {
            var transactions = await _tradeService.GetTransactions(_brokerId);

            return transactions;
        }

        private void SetTransactions(List<Transaction> transactions)
        {
            if (transactions != null)
            {
                transactions = transactions.OrderBy(t => t.CreationDate).ToList();

                _transactions = new ObservableCollection<Transaction>(transactions);
                TransactionsListView.ItemsSource = _transactions;
            }

            bool isVisibile = _transactions == null || _transactions.Count == 0;
            TransactionsListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AddTransaction(Transaction transaction)
        {
            if (_transactions == null)
            {
                _transactions = new ObservableCollection<Transaction>();
                TransactionsListView.ItemsSource = _transactions;
            }

            if (!_transactions.Any(t => t.Id == transaction.Id))
            {
                _transactions.Add(transaction);
            }
        }

        private void OpenTransactionsWindow()
        {
            var window = new TransactionsWindow(_tradeService, _transactions.ToList());
            window.Show();
        }



        /* ---------------------------------------------------------- */

        private void TradeService_OnReconnected()
        {
            Dispatcher.InvokeAsync(() =>
            {
                _reconnectingDialog.Close();
            });
        }

        private void TradeService_OnReconnecting()
        {
            Dispatcher.InvokeAsync(async () =>
            {
                _reconnectingDialog = new ReconnectingDialog();
                _reconnectingDialog.Owner = this;

                bool? result = _reconnectingDialog.ShowDialog();

                if (!_tradeService.IsConnected)
                    await Disconnect();
                else
                {
                    await RefreshData();
                }
            });
        }

        private void TradeHub_OnCreatedTransaction(string username, Transaction transaction)
        {
            Dispatcher.InvokeAsync(() =>
            {
                AddTransaction(transaction);
            });
        }

        private void TradeHub_OnCreateOrderBook(string username, OrderBook orderBook)
        {
            if (orderBook.BrokerId == _brokerId)
            {
                Dispatcher.Invoke(() => AddOrderBook(orderBook));
            }
        }

        private void TradeHub_OnDeleteOrderBook(string username, OrderBook orderBook)
        {
            if (orderBook.BrokerId == _brokerId)
            {
                Dispatcher.Invoke(() => RemoveOrderBook(orderBook.Id));
            }
        }

        // --

        private async void ConnectBtn_OnClick(object sender, RoutedEventArgs e)
        {
            bool isConnected = await Connect();

            if (isConnected)
                await RefreshData();
        }

        private async void DisconnectBtn_OnClick(object sender, RoutedEventArgs e)
        {
            await Disconnect();
        }

        //--

        private void OpenTransactions_OnClick(object sender, RoutedEventArgs e)
        {
            OpenTransactionsWindow();
        }

        // --

        private async void CreateOrderBook_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenOrderBookDialog(_tickers);
            dialog.Owner = this;

            // Display the dialog box and read the response
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var selectedTicker = dialog.SelectedTicker;

                var orderBook = await CreateOrderBook(selectedTicker.Id);

                OpenOrderBookWindow(orderBook);
            }
        }

        private void OpenRowBtn_OnClick(object sender, RoutedEventArgs e)
        {
            OrderBook orderBook = ((FrameworkElement)sender).DataContext as OrderBook;

            OpenOrderBookWindow(orderBook);
        }

        private async void DeleteRowBtn_OnClick(object sender, RoutedEventArgs e)
        {
            OrderBook orderBook = ((FrameworkElement)sender).DataContext as OrderBook;

            await DeleteOrderBook(orderBook);
        }

        // --

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}