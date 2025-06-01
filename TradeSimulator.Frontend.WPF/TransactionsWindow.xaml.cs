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
    /// Interaction logic for TransactionsWindow.xaml
    /// </summary>
    public partial class TransactionsWindow : Window
    {
        private TradeService TradeService;
        private List<Transaction> Transactions;

        public TransactionsWindow(TradeService tradeService, List<Transaction> transactions)
        {
            TradeService = tradeService;
            Transactions = transactions;

            InitializeComponent();

            TransactionsListView.ItemsSource = Transactions;
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            await TradeService.OpenTransactionHistory();
        }

        protected async override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            await TradeService.CloseTransactionHistory();
        }
    }
}
