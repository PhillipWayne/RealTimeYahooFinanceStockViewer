using System.Linq;
using MbUnit.Framework;
using StockPriceService;

namespace UnitTests
{
    [TestFixture]
    public class ServiceTests
    {
        private StockService _stockService;
        
        [Test]
        public void TestParseCsv()
        {
            //test to check ParseStockPrices
            _stockService = new StockService();
            const string csvString = @"0200.HK,N/A,MELCO INT'L DEV,N/A,N/A,N/A,N/A,N/A,22.000,N/A,2554470,N/A,22.150,N/A";
            var result = _stockService.ParseStockPrices(csvString, "s,n,b,a,o,v,l1");
            Assert.AreEqual(result.Count, 1);
        }

        public void TestFetchData()
        {
            //test to check for FetchStockPrices
            _stockService = new StockService();
            var result = _stockService.FetchStockPrices("http://finance.yahoo.com/d/quotes.csv?s=0200.HK&f=s,n,b,a,o,v,l1");
            Assert.IsNotNull(result);
        }
    }
}
