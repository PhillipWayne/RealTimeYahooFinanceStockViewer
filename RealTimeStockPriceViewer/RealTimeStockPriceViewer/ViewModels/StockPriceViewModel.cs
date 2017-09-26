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
    public class StockPriceViewModel : BaseViewModel
    {
        //MVVM command to get the prices
        public RelayCommand StartRealTimeFeedCommand { get; set; }

        public RelayCommand StopRealTimeFeedCommand { get; set; }

        //show historic window command
        public RelayCommand ShowHistoricWindowCommand { get; set; }
        
        //url of yahoo finance to fetch the prices
        private readonly string _serviceUrl;
        
        //fields charactors to fetch from the web
        private readonly string _fieldsToFetch;

        //stock service
        private readonly StockService _stockService;

        private readonly DispatcherTimer _stockPriceTimer;
        public StockPriceViewModel()
        {
            //initialize the start real time feed command
            StartRealTimeFeedCommand = new RelayCommand(GetStockPriceFromService, CanStartCommand);

            //initialize the stop real time feed command
            StopRealTimeFeedCommand = new RelayCommand(StopFeedFromService, CanStartCommand);

            ShowHistoricWindowCommand = new RelayCommand(ShowHistoricWindow, CanShowHistoricWindow);
            //get the url and fields info from the app.config
            _serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
            _fieldsToFetch = ConfigurationManager.AppSettings["FieldsToFetch"];

            CsvStockSymbols = ConfigurationManager.AppSettings["CsvSymbols"];
            
            //instantiate the stock service
            _stockService = new StockService();
            
            //instantiate the observable collection
            StockPrices = new ObservableCollection<StockPrice>();

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
            var historicView = new HistoricStockPriceView(); 
            historicView.Show();
        }
        
        
        public string CsvStockSymbols
        {
            get; set;
        }
        private bool CanStartCommand(object obj)
        {
            return true;
        }

        private string FormattedUrl
        {
            get { return string.Format(_serviceUrl + _fieldsToFetch, CsvStockSymbols); }
        }

        private void StopFeedFromService(object obj)
        {
            if (_stockPriceTimer.IsEnabled)
            {
                _stockPriceTimer.Stop();
            }
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
                int index = StockPrices.IndexOf(addedStock);
                StockPrices[index].Ask = stock.Ask;
                StockPrices[index].Bid = stock.Bid;
                StockPrices[index].LastTradedPrice = stock.LastTradedPrice;
                StockPrices[index].TradedVolume = stock.TradedVolume;
                StockPrices[index].OpenPrice = stock.OpenPrice;
            }
        }

        public ObservableCollection<StockPrice> StockPrices
        {
            get;
            set;
        }
    }
}
