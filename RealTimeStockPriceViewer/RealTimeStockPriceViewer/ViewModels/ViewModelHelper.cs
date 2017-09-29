using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealTimeStockPriceViewer.ViewModels
{
    public class ViewModelHelper
    {
        public static void ShowMessage(string message, string caption, MessageBoxButton button = MessageBoxButton.OK)
        {
            MessageBox.Show(message, caption, button);
        }
    }
}
