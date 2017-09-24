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
                && fieldsToFetch.IndexOfAny(new char[] { 's', 'n', 'b', 'a', 'o','v', 'l', '1' }) >= 0)
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
                        stockPrice.Symbol = stockColumns[fieldsToFetch.IndexOf('s')];
                        stockPrice.Name = stockColumns[fieldsToFetch.IndexOf('n')];

                        stockPrice.Bid = stockColumns[fieldsToFetch.IndexOf('b')] == "N/A"
                            ? 0
                            : Convert.ToDecimal(stockColumns[fieldsToFetch.IndexOf('b')]);

                        stockPrice.Ask = stockColumns[fieldsToFetch.IndexOf('a')] == "N/A"
                            ? 0
                            : Convert.ToDecimal(stockColumns[fieldsToFetch.IndexOf('a')]);

                        stockPrice.OpenPrice = stockColumns[fieldsToFetch.IndexOf('o')] == "N/A"
                            ? 0
                            : Convert.ToDecimal(stockColumns[fieldsToFetch.IndexOf('o')]);

                        stockPrice.TradedVolume = stockColumns[fieldsToFetch.IndexOf('v')] == "N/A"
                            ? 0
                            : Convert.ToDecimal(stockColumns[fieldsToFetch.IndexOf('v')]);

                        stockPrice.LastTradedPrice = stockColumns[fieldsToFetch.IndexOf("l1", System.StringComparison.Ordinal)] == "N/A"
                            ? 0
                            : Convert.ToDecimal(stockColumns[fieldsToFetch.IndexOf("l1", System.StringComparison.Ordinal)]);

                        sotckPricesrices.Add(stockPrice);
                    }
                }
            }

            return sotckPricesrices;
        }

    }
}
