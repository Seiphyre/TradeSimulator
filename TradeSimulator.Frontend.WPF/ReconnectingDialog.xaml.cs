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
    /// Interaction logic for ReconnectingDialog.xaml
    /// </summary>
    public partial class ReconnectingDialog : Window
    {
        public bool Dismissed { get; private set; } = false;
        public ReconnectingDialog()
        {
            InitializeComponent();
        }

        public void Dismiss()
        {
            Dismissed = true;
            Close();
        }
    }
}
