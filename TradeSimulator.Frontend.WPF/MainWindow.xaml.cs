using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private TradeService TradeService = new TradeService();

        private string BrokerId;
        private Broker Broker;
        private List<Ticker> Tickers = new();
        private ObservableCollection<OrderBook> OrderBooks = new();
        private ObservableCollection<Transaction> Transactions = new();

        private ReconnectingDialog ReconnectingDialog = null;

        private List<Window> windows = new();


        public MainWindow()
        {
            InitializeComponent();

            ConnectBtn.Visibility = Visibility.Visible;
            DisconnectBtn.Visibility = Visibility.Collapsed;
            BrokerIdTextBox.IsEnabled = true;
            Dashboard.Visibility = Visibility.Collapsed;

            BrokerIdTextBox.Text = GenerateRandomBrokerId();

            TradeService.OnCreatedOrderBook += TradeHub_OnCreateOrderBook;
            TradeService.OnDeletedOrderBook += TradeHub_OnDeleteOrderBook;
            TradeService.OnCreatedTransaction += TradeHub_OnCreatedTransaction;
            TradeService.OnReconnecting += TradeService_OnReconnecting;
            TradeService.OnReconnected += TradeService_OnReconnected;
        }

        public void Dispose()
        {
            TradeService.OnCreatedOrderBook -= TradeHub_OnCreateOrderBook;
            TradeService.OnDeletedOrderBook -= TradeHub_OnDeleteOrderBook;
            TradeService.OnCreatedTransaction -= TradeHub_OnCreatedTransaction;
            TradeService.OnReconnecting -= TradeService_OnReconnecting;
            TradeService.OnReconnected -= TradeService_OnReconnected;
        }

        private void TradeHub_OnCreatedTransaction(string username, Transaction transaction)
        {
            Dispatcher.InvokeAsync(() =>
            {
                AddTransaction(transaction);
            });
        }

        private void TradeService_OnReconnected()
        {
            Dispatcher.InvokeAsync(() =>
            {
                ReconnectingDialog.Close();
            });
        }

        private void TradeService_OnReconnecting()
        {
            Dispatcher.InvokeAsync(async () =>
            {
                ReconnectingDialog = new ReconnectingDialog();
                ReconnectingDialog.Owner = this;

                bool? result = ReconnectingDialog.ShowDialog();

                if (!TradeService.IsConnected)
                    await Disconnect();
                else
                {
                    await RefreshData();
                }
            });
        }

        public async Task RefreshData()
        {
            await GetOrCreateBroker();
            await GetTickers();

            var orderBooks = await FetchOrderBooks();
            SetOrderBooks(orderBooks);

            var transactions = await FetchTransactions();
            SetTransactions(transactions);
        }

        public async Task<bool> Connect()
        {
            ConnectBtn.IsEnabled = false;
            ConnectBtn.Content = "Connecting...";

            try
            {
                await TradeService.Connect("http://localhost:5038/trade-hub", BrokerIdTextBox.Text);

                DisconnectBtn.Visibility = Visibility.Visible;
                ConnectBtn.Visibility = Visibility.Collapsed;
                BrokerIdTextBox.IsEnabled = false;
                Dashboard.Visibility = Visibility.Visible;

                BrokerId = BrokerIdTextBox.Text;

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

        public async Task Disconnect()
        {
            DisconnectBtn.IsEnabled = false;
            DisconnectBtn.Content = "Disconnecting...";

            try
            {
                await TradeService.Disconnect();

                ConnectBtn.Visibility = Visibility.Visible;
                DisconnectBtn.Visibility = Visibility.Collapsed;
                BrokerIdTextBox.IsEnabled = true;
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

        /* ---------------------------------------------------------- */

        protected string GenerateRandomBrokerId()
        {
            return "Victor";
        }

        protected async Task GetOrCreateBroker()
        {
            Broker = await TradeService.GetOrCreateBroker(BrokerId);
        }

        /* ---------------------------------------------------------- */

        protected async Task GetTickers()
        {
            Tickers = await TradeService.GetTickers();
        }

        /* ---------------------------------------------------------- */

        protected async Task<List<OrderBook>> FetchOrderBooks()
        {
            var orderBooks = await TradeService.GetOrderBooks(BrokerId);

            return orderBooks;
        }

        protected async Task<OrderBook> CreateOrderBook(string tickerId)
        {
            var orderBook = await TradeService.CreateOrderBook(BrokerId, tickerId);

            return orderBook;
        }

        protected async Task DeleteOrderBook(OrderBook orderBook)
        {
            await TradeService.DeleteOrderBook(orderBook.Id);
        }

        protected void SetOrderBooks(List<OrderBook> orderBooks)
        {
            OrderBooks = new ObservableCollection<OrderBook>(orderBooks);
            OrderBooksListView.ItemsSource = OrderBooks;

            bool isVisibile = OrderBooks == null || OrderBooks.Count == 0;
            OrderBooksListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
        }

        protected void AddOrderBook(OrderBook newOrderBook)
        {
            if (OrderBooks == null)
            {
                OrderBooks = new ObservableCollection<OrderBook>();
                OrderBooksListView.ItemsSource = OrderBooks;
            }

            if (!OrderBooks.Any(orderBook => orderBook.Id == newOrderBook.Id))
            {
                OrderBooks.Add(newOrderBook);

                OrderBooksListView.ItemsSource = null;
                OrderBooksListView.ItemsSource = OrderBooks;
                OrderBooksListView.Visibility = Visibility.Visible;
            }
        }

        protected void RemoveOrderBook(string orderBookId)
        {
            if (OrderBooks == null)
                return;

            var orderbook = OrderBooks.FirstOrDefault(ob => ob.Id == orderBookId);

            if (orderbook != null)
            {
                OrderBooks.Remove(orderbook);

                bool isVisibile = OrderBooks == null || OrderBooks.Count == 0;
                OrderBooksListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected void OpenOrderBookWindow(OrderBook orderBook)
        {
            var orderBookTicker = Tickers.FirstOrDefault(ticker => ticker.Id == orderBook.TickerId);

            var window = new OrderBookWindow(TradeService, orderBook, orderBookTicker);

            window.Show();
        }

        public void CloseAllSubWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                var window = App.Current.Windows[intCounter];

                if (window == this)
                    continue;

                window.Close();
            }
        }



        /* ---------------------------------------------------------- */

        protected async Task<List<Transaction>> FetchTransactions()
        {
            var transactions = await TradeService.GetTransactions(BrokerId);

            return transactions;
        }

        protected void SetTransactions(List<Transaction> transactions)
        {
            Transactions = new ObservableCollection<Transaction>(transactions);
            TransactionsListView.ItemsSource = Transactions;

            bool isVisibile = Transactions == null || Transactions.Count == 0;
            TransactionsListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
        }

        protected void AddTransaction(Transaction transaction)
        {
            if (Transactions == null)
            {
                Transactions = new ObservableCollection<Transaction>();
                TransactionsListView.ItemsSource = Transactions;
            }

            if (!Transactions.Any(t => t.Id == transaction.Id))
            {
                Transactions.Add(transaction);

                TransactionsListView.ItemsSource = null;
                TransactionsListView.ItemsSource = Transactions;
            }
        }

        protected void RemoveTransaction(string transactionId)
        {
            if (Transactions == null)
                return;

            var transaction = Transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transaction != null)
            {
                Transactions.Remove(transaction);
            }
        }

        private void OpenTransactionsWindow()
        {
            var window = new TransactionsWindow(TradeService, Transactions.ToList());
            window.Show();
        }



        /* ---------------------------------------------------------- */

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

        private void OpenTransactions_OnClick(object sender, RoutedEventArgs e)
        {
            OpenTransactionsWindow();
        }

        private async void OpenOrderBook_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenOrderBookDialog(Tickers);
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

        private void TradeHub_OnCreateOrderBook(string username, OrderBook orderBook)
        {
            if (orderBook.BrokerId == BrokerId)
            {
                Dispatcher.Invoke(() => AddOrderBook(orderBook));
            }
        }

        private void TradeHub_OnDeleteOrderBook(string username, OrderBook orderBook)
        {
            if (orderBook.BrokerId == BrokerId)
            {
                Dispatcher.Invoke(() => RemoveOrderBook(orderBook.Id));
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
    }
}