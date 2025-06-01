using System.Windows;

namespace TradeSimulator.Frontend.WPF
{
    /// <summary>
    /// Interaction logic for ReconnectingDialog.xaml
    /// </summary>
    public partial class ReconnectingDialog : Window
    {
        public bool Dismissed { get; private set; } = false;



        /* ------------------------------------------------- */

        public ReconnectingDialog()
        {
            InitializeComponent();
        }



        /* ------------------------------------------------- */

        public void Dismiss()
        {
            Dismissed = true;
            Close();
        }
    }
}
