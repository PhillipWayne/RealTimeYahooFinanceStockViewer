using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public class StockServiceFacade
    {
        private string _stocks, _url, _fields;
        private StockPriceService _stockPriceService;

        /// <summary>
        /// Creates an instance of the StockServiceFacade
        /// </summary>
        /// <param name="stocks">+ separated string reperentation of the stocks</param>
        /// <param name="url">the web url to fetch the stocks</param>
        /// <param name="fields">fields requested to be fetched</param>
        public StockServiceFacade(string stocks, string url, string fields)
        {
            //intialize the fields
            _stocks = stocks;
            _url = string.Format(url + fields, stocks);
            _fields = fields;


            //instantiat the StockPriceService to get the data
            _stockPriceService = new StockPriceService();
        }

        /// <summary>
        /// Get the Stock prices from the StockPriceService
        /// </summary>
        /// <returns>List of StockPrices</returns>
        public List<StockPrice> GetStockPrices()
        {
            //Fetch the stocks from the url in CSV format
            var stocksInCsvFormat =_stockPriceService.FetchStockPrices(_url);

            //Parse and return the stocks in CSV format to List of StockPrices
            return _stockPriceService.ParseStockPrices(stocksInCsvFormat, _fields);
        }
        
    }
}
