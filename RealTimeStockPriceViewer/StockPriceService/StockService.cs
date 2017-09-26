using System;
using System.Collections.Generic;
using System.Net;
using Model;

namespace StockPriceService
{
    public class StockService : IStockService
    {
        /// <summary>
        /// Get the Stock Prices from the Yahoo finance
        /// </summary>
        /// <param name="url"></param>
        /// <returns> CSV formatted string data of the stocks</returns>
        public string FetchStockPrices(string url)
        {
            string csvData = null;
            try
            {
                using (var web = new WebClient())
                {
                    csvData = web.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return csvData;
        }

        Random rd = new Random(100);
        /// <summary>
        /// Parse the CSV data received from the web and create the stock price collection
        /// </summary>
        /// <param name="csvData"> The CSV data format of the Stocks</param>
        /// <param name="fieldsToFetch"> The format of the field sent in the web request</param>
        /// <returns>List of StockPrices</returns>
        public List<StockPrice> ParseStockPrices(string csvData, string fieldsToFetch)
        {
            //Creates the stock price collection
            var sotckPricesrices = new List<StockPrice>();

            if (!string.IsNullOrWhiteSpace(csvData) && !string.IsNullOrWhiteSpace(fieldsToFetch)
                && fieldsToFetch.IndexOfAny(new char[] { 's', 'b', 'a', 'o','v', 'l', '1' }) >= 0)
            {
                //split the CSV data for each line as each line represents a stock
                string[] stockRows = csvData.Replace("\r", "").Split('\n');

                if (stockRows.Length > 0)
                {
                    foreach (string sotckRow in stockRows)
                    {
                        //Check if the stock is available
                        if (String.IsNullOrEmpty(sotckRow)) continue;

                        //Each stock row from CSV data is coma seperated and needs to be split
                        string[] stockColumns = sotckRow.Split(',');

                        //Create the Stockprice object
                        var stockPrice = new StockPrice();
                        
                        //Map each column index to the index of the fieldFormat
                        stockPrice.Symbol = stockColumns[fieldsToFetch.IndexOf('s')].Replace("\"", string.Empty);
                        stockPrice.Bid = GetColumnValue("b", stockColumns, fieldsToFetch);
                        stockPrice.Ask = GetColumnValue("a", stockColumns, fieldsToFetch);
                        stockPrice.OpenPrice = GetColumnValue("o", stockColumns, fieldsToFetch);
                        stockPrice.TradedVolume = GetColumnValue("v", stockColumns, fieldsToFetch);
                        stockPrice.LastTradedPrice = GetColumnValue("l1", stockColumns, fieldsToFetch);

                        sotckPricesrices.Add(stockPrice);
                    }
                }
            }

            return sotckPricesrices;
        }

        private decimal? GetColumnValue(string field, string[] stockColumns, string fieldsToFetch)
        {
            if (stockColumns[fieldsToFetch.IndexOf(field, System.StringComparison.OrdinalIgnoreCase)] == "N/A")
            {
                return null;
            }
            decimal columnValue;
            decimal.TryParse(stockColumns[fieldsToFetch.IndexOf(field, System.StringComparison.OrdinalIgnoreCase)],
                out columnValue);
            return columnValue;
        }
    }
}
