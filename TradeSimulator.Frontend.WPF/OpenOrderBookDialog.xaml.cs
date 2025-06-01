using System.Windows;
using System.Windows.Controls;

using TradeSimulator.Shared.Models;

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for OpenOrderBookDialog.xaml
    /// </summary>
    public partial class OpenOrderBookDialog : Window
    {
        private List<Ticker> _tickers;

        public Ticker SelectedTicker { get; private set; }



        /* ------------------------------------------------- */

        public OpenOrderBookDialog(List<Ticker> tickers)
        {
            InitializeComponent();

            _tickers = tickers;

            foreach (Ticker ticker in _tickers)
            {
                TickerComboBox.Items.Add(new ComboBoxItem() { Content = ticker.DisplayName });
            }

            TickerComboBox.SelectedItem = TickerComboBox.Items[0];
        }



        /* ------------------------------------------------- */

        private void OkBtn_OnClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = TickerComboBox.SelectedIndex;

            SelectedTicker = _tickers[selectedIndex];

            DialogResult = true;
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
