using System.Collections.Generic;
using Model;

namespace StockPriceService
{
    public interface IStockService
    {
        /// <summary>
        /// Get the Stock Prices from the web
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string FetchStockPrices(string url);

        /// <summary>
        /// Parse the stock price string to a list of stock prices
        /// </summary>
        /// <param name="csvData"></param>
        /// <param name="fieldsToFetch"></param>
        /// <returns></returns>
        List<StockPrice> ParseStockPrices(string csvData, string fieldsToFetch);
    }
}
