using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Threading;
using Commands;
using Model;
using StockPriceService;

namespace RealTimeStockPriceViewer.ViewModels
{
    public class StockPriceViewModel
    {
        //MVVM command to get the prices
        public RelayCommand GetPricesCommand { get; set; }

        //show historic window command
        public RelayCommand ShowHistoricWindowCommand { get; set; }
        
        //url of yahoo finance to fetch the prices
        private readonly string _serviceUrl;
        
        //fields charactors to fetch from the web
        private readonly string _fieldsToFetch;

        //stock service
        private readonly StockService _stockService;

        private DispatcherTimer _stockPriceTimer;
        public StockPriceViewModel()
        {
            //initialize the command
            GetPricesCommand = new RelayCommand(GetStockPriceFromService, CanGetPrices);

            ShowHistoricWindowCommand = new RelayCommand(ShowHistoricWindow, CanShowHistoricWindow);
            //get the url and fields info from the app.config
            _serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
            _fieldsToFetch = ConfigurationManager.AppSettings["FieldsToFetch"];
            
            //instantiate the stock service
            _stockService = new StockService();
            
            //instantiate the observable collection
            StockPrices = new ObservableCollection<StockPrice>();
            
            //set the stocksymbols to default values in the GUI
            if (string.IsNullOrWhiteSpace(CsvStockSymbols))
                CsvStockSymbols = "0200.HK,0941.HK,2318.HK";

            //initilize the timer
            int interval;
            int.TryParse(ConfigurationManager.AppSettings["TimerIntervalInSeconds"], out interval);
            _stockPriceTimer = new DispatcherTimer();
            _stockPriceTimer.Tick += _stockPriceTimer_Tick;
            _stockPriceTimer.Interval = new TimeSpan(0, 0, 0, interval == 0 ? 50 : interval);
           }

        private bool CanShowHistoricWindow(object obj)
        {
            return true;
        }

        private void ShowHistoricWindow(object obj)
        {
         
        }
        
        
        public string CsvStockSymbols
        {
            get; set;
        }
        private bool CanGetPrices(object obj)
        {
            return true;
        }

        private string FormattedUrl
        {
            get { return string.Format(_serviceUrl + _fieldsToFetch, CsvStockSymbols); }
        }

        private void GetStockPriceFromService(object obj)
        {
            GetPrices();
            if (!_stockPriceTimer.IsEnabled)
            {
                _stockPriceTimer.Start();
            }
        }

        void _stockPriceTimer_Tick(object sender, EventArgs e)
        {
            GetPrices();
        }

        private void GetPrices()
        {
            var csvDetailResults = _stockService.FetchStockPrices(FormattedUrl);

            if (!string.IsNullOrEmpty(csvDetailResults))
            {
                AddOrUpdateStockPriceCollection(_stockService.ParseStockPrices(csvDetailResults, _fieldsToFetch));
            }
        }

        private void AddOrUpdateStockPriceCollection(List<StockPrice> stockPriceList)
        {
            if (stockPriceList.Any())
            {
                foreach (var stock in stockPriceList)
                {
                    if (StockPrices.Any(item => item.Symbol == stock.Symbol))
                    {
                        UpdateStockItem(stock);
                    }
                    else
                    {
                        StockPrices.Add(stock);
                    }
                }
            }
        }

        private void UpdateStockItem(StockPrice stock)
        {
            var addedStock = StockPrices.FirstOrDefault(item => item.Symbol == stock.Symbol);
            if (addedStock != null)
            {
                addedStock.Ask = stock.Ask;
                addedStock.Bid = stock.Bid;
                addedStock.LastTradedPrice = stock.LastTradedPrice;
                addedStock.TradedVolume = stock.TradedVolume;
                addedStock.OpenPrice = stock.OpenPrice;
            }
        }

        public ObservableCollection<StockPrice> StockPrices
        {
            get; set;
        }
    }
}
