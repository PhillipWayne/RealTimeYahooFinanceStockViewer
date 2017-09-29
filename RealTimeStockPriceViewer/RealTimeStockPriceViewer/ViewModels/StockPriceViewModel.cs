using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        #region Properties
        //MVVM command to get the prices
        public RelayCommand StartRealTimeFeedCommand { get; set; }

        public RelayCommand StopRealTimeFeedCommand { get; set; }

        //show historic window command
        public RelayCommand ShowHistoricWindowCommand { get; set; }
        
        //observable collection to store stock prices
        public ObservableCollection<StockPrice> StockPrices
        {
            get;
            set;
        }
#endregion

        #region Fields
        //url of yahoo finance to fetch the prices
        private readonly string _serviceUrl;

        //fields charactors to fetch from the web
        private readonly string _fieldsToFetch;

        //stock service
        private readonly StockService _stockService;

        private readonly DispatcherTimer _stockPriceTimer;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
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

            //get the symbols from the config, this can be inclued to be taken from UI as well
            CsvStockSymbols = ConfigurationManager.AppSettings["CsvSymbols"];

            //instantiate the stock service
            _stockService = new StockService();

            //instantiate the observable collection
            StockPrices = new ObservableCollection<StockPrice>();

            //initilize the timer ticks from app.config, if no value then default to 50 sec
            int interval;
            int.TryParse(ConfigurationManager.AppSettings["TimerIntervalInSeconds"], out interval);
            _stockPriceTimer = new DispatcherTimer();
            _stockPriceTimer.Tick += _stockPriceTimer_Tick;
            _stockPriceTimer.Interval = new TimeSpan(0, 0, 0, interval == 0 ? 50 : interval);
        }

        #endregion

        #region Methods
        private bool CanShowHistoricWindow(object obj)
        {
            return true;
        }

        //show the historic view
        private void ShowHistoricWindow(object obj)
        {
            var historicView = new HistoricStockPriceView();
            historicView.Show();
        }

        /// <summary>
        /// CsvStockSymbols property
        /// </summary>
        public string CsvStockSymbols
        {
            get;
            set;
        }

        private bool CanStartCommand(object obj)
        {
            return true;
        }


        /// <summary>
        /// FormattedUrl needed for the service
        /// </summary>
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

        //Get the stock price asynchronously and start the timer
        private void GetStockPriceFromService(object obj)
        {
            if (!string.IsNullOrWhiteSpace(_fieldsToFetch))
            {
                GetPricesAsync();
                if (!_stockPriceTimer.IsEnabled)
                {
                    _stockPriceTimer.Start();
                }
            }
            else
            {
                ViewModelHelper.ShowMessage("FieldsToFetch is empty. Please specify the fields to be fetched in the app.config", "Error");
            }
        }

        /// <summary>
        /// Time tick event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _stockPriceTimer_Tick(object sender, EventArgs e)
        {
            _stockPriceTimer.Stop();
            GetPricesAsync();
            _stockPriceTimer.Start();
        }

        /// <summary>
        /// GetPrices from service asynchronously
        /// </summary>
        private void GetPricesAsync()
        {
            var bgworker = new BackgroundWorker();
            bgworker.DoWork += GetPrices;
            bgworker.RunWorkerCompleted += bgworker_RunWorkerCompleted;
            bgworker.RunWorkerAsync();
        }

        /// <summary>
        /// background woek completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="runWorkEventArgs"></param>
        void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkEventArgs)
        {
            var stockListResults = runWorkEventArgs.Result as List<StockPrice>;
            AddOrUpdateStockPriceCollection(stockListResults);
        }

        /// <summary>
        /// Background workers dowork method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GetPrices(object sender, DoWorkEventArgs args)
        {
            var csvDetailResults = _stockService.FetchStockPrices(FormattedUrl);

            if (!string.IsNullOrEmpty(csvDetailResults))
            {
                args.Result = _stockService.ParseStockPrices(csvDetailResults, _fieldsToFetch);
            }
        }

        //Add stock to collection if exists else update the collection
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

        //update stock item if exists
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
                StockPrices[index].TimeStamp = DateTime.Now;
            }
        }
        #endregion
       
    }
}
