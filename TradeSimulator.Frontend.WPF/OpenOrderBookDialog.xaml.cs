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

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for OpenOrderBookDialog.xaml
    /// </summary>
    public partial class OpenOrderBookDialog : Window
    {
        private List<Ticker> Tickers;

        public Ticker SelectedTicker { get; private set; }

        public OpenOrderBookDialog(List<Ticker> tickers)
        {
            InitializeComponent();

            Tickers = tickers;

            foreach (Ticker ticker in Tickers)
            {
                TickerComboBox.Items.Add(new ComboBoxItem() { Content = ticker.DisplayName });
            }

            TickerComboBox.SelectedItem = TickerComboBox.Items[0];
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = TickerComboBox.SelectedIndex;

            SelectedTicker = Tickers[selectedIndex];

            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
