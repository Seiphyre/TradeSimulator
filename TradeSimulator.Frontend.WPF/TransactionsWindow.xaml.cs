using System.Windows;

using TradeSimulator.Shared.Models;
using TradeSimulator.Shared.Services;

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for TransactionsWindow.xaml
    /// </summary>
    public partial class TransactionsWindow : Window
    {
        private TradeService _tradeService;
        private List<Transaction> _transactions;



        /* ----------------------------------------------------- */

        public TransactionsWindow(TradeService tradeService, List<Transaction> transactions)
        {
            _tradeService = tradeService;
            _transactions = transactions;

            InitializeComponent();

            TransactionsListView.ItemsSource = _transactions;
        }



        /* ----------------------------------------------------- */

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_tradeService != null && _tradeService.IsConnected)
                await _tradeService.OpenTransactionHistory();
        }

        protected async override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_tradeService != null && _tradeService.IsConnected)
                await _tradeService.CloseTransactionHistory();
        }
    }
}
