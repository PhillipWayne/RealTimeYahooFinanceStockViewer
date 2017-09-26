using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace RealTimeStockPriceViewer.ViewModels
{
    public class HistoricStockPriceViewModel : BaseViewModel
    {
        public HistoricStockPriceViewModel()
        {
            
        }
        public ObservableCollection<StockPrice> HistoricStockPrices
        {
            get;
            set;
        }
    }
}
