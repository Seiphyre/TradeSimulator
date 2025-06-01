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

        private string BrokerId => BrokerIdTextBox.Text;
        private Broker Broker;
        private List<Ticker> Tickers = new();
        private List<OrderBook> OrderBooks = new();
        private List<Transaction> Transactions = new();

        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public MainWindow()
        {
            InitializeComponent();

            BrokerIdTextBox.Text = GenerateRandomBrokerId();

            //TradeService.OnCreatedOrderBook += TradeHub_OnCreateOrderBook;
            //TradeService.OnDeletedOrderBook += TradeHub_OnDeleteOrderBook;
        }

        public void Dispose()
        {
            //TradeService.OnCreatedOrderBook -= TradeHub_OnCreateOrderBook;
            //TradeService.OnDeletedOrderBook -= TradeHub_OnDeleteOrderBook;
        }

        public async Task Connect()
        {
            ConnectBtn.IsEnabled = false;
            ConnectBtn.Content = "Connecting...";

            try
            {
                await TradeService.Connect("http://localhost:5038/trade-hub", BrokerIdTextBox.Text);

                DisconnectBtn.Visibility = Visibility.Visible;
                ConnectBtn.Visibility = Visibility.Collapsed;
                BrokerIdTextBox.IsEnabled = false;

                Console.WriteLine("Connected !");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to connect.");
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

        protected async Task GetOrderBooks()
        {
            OrderBooks = await TradeService.GetOrderBooks(BrokerId);
        }

        /* ---------------------------------------------------------- */

        protected async Task GetTransactions()
        {
            Transactions = await TradeService.GetTransactions(BrokerId);
            TransactionsListView.ItemsSource = Transactions;

            bool isVisibile = Transactions == null || Transactions.Count == 0;
            TransactionsListView.Visibility = isVisibile ? Visibility.Collapsed : Visibility.Visible;
        }

        protected string GenerateRandomBrokerId()
        {
            return "Victor";
        }

        private void results_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection sortDirection;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        sortDirection = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            sortDirection = ListSortDirection.Descending;
                        }
                        else
                        {
                            sortDirection = ListSortDirection.Ascending;
                        }
                    }

                    string sortPropertyName = (headerClicked.Column.DisplayMemberBinding as Binding).Path.Path;
                    Sort(sortPropertyName, sortDirection);

                    if (sortDirection == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = sortDirection;
                }
            }
        }

        // Sort code
        private void Sort(string propertyName, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(OrderBookListView.ItemsSource);

            dataView.SortDescriptions.Clear();

            SortDescription sd = new SortDescription(propertyName, direction);
            dataView.SortDescriptions.Add(sd);

            dataView.Refresh();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ConnectBtn_OnClick(object sender, RoutedEventArgs e)
        {
            await Connect();

            await GetOrCreateBroker();
            await GetTickers();
            await GetOrderBooks();
            await GetTransactions();
        }

        private async void DisconnectBtn_OnClick(object sender, RoutedEventArgs e)
        {
            await Disconnect();
        }
    }
}