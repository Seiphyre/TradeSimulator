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

        public OrderBookWindow(TradeService tradeService, Ticker ticker)
        {
            InitializeComponent();

            TradeService = tradeService;
            Ticker = ticker;

            OrdersListView.ItemsSource = Ticker.Orders;
            TickerTextBox.Text = $"Ticker: {Ticker.DisplayName}";
        }
    }
}
