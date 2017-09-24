using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commands;
using Model;
using StockPriceService;

namespace ViewModels
{
    public class StockPriceViewModel
    {
        public RelayCommand GetPricesCommand { get; set; }
        private readonly string _serviceUrl;
        private readonly string _fieldsToFetch;
        private readonly StockService stockService;
        
        public StockPriceViewModel()
        {
            GetPricesCommand = new RelayCommand(GetStockPriceFromService, CanGetPrices);
            _serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
            _fieldsToFetch = ConfigurationManager.AppSettings["FieldsToFetch"];
            stockService = new StockService();
            StockPrices = new ObservableCollection<StockPrice>();
        }

        public string CsvStockSymbols
        {
            get; set;
        }
        private bool CanGetPrices(object obj)
        {
            //hardcoded to return true
            return true;
        }

        private string FormattedUrl
        {
            get { return string.Format(_serviceUrl + _fieldsToFetch, CsvStockSymbols); }
        }

        private void GetStockPriceFromService(object obj)
        {
            var csvDetailResults = stockService.FetchStockPrices(FormattedUrl);

            if (!string.IsNullOrEmpty(csvDetailResults))
            {
                AddToObservableCollection(stockService.ParseStockPrices(csvDetailResults, _fieldsToFetch));
            }
        }

        private void AddToObservableCollection(List<StockPrice> stockPriceList)
        {
            if (stockPriceList.Any())
            {
                foreach (var stock in stockPriceList)
                {
                    StockPrices.Add(stock);
                }
            }
        }
        public ObservableCollection<StockPrice> StockPrices
        {
            get; set;
        }
    }
}
