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
        public void TestInvalidStock()
        {
            //test to check for invalid stock the service returns no data
            _stockService = new StockService();
            var result = _stockService.FetchStockPrices("http://finance.yahoo.com/d/quotes.csv?s=123&f=sab");
            //Assert.AreEqual(result.Count, 0);
        }


        [Test]
        public void TestNullfields()
        {
            //test to check if no fields are requested from the web the service returns no data
            var serviceFacade = _stockService.FetchStockPrices("http://finance.yahoo.com/d/quotes.csv?s=AAPL&f=");
            //Assert.AreEqual(result.Count, 0);
        }

        //[Test]
        //public void TestEmptyFields()
        //{
        //    //test to check if emtpty fields are requested from the web the service returns no data
        //    var serviceFacade = new StockServiceFacade("AAPL", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "");
        //    var result = serviceFacade.GetStockPrices();
        //    Assert.AreEqual(result.Count, 0);
        //}

        //[Test]
        //public void TestInvalidFields()
        //{
        //    //test to check if invalid fields are requested from the web the service returns no data
        //    var serviceFacade = new StockServiceFacade("AAPL", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "123");
        //    var result = serviceFacade.GetStockPrices();
        //    Assert.AreEqual(result.Count, 0);
        //}

        //[Test]
        //public void TestValidSingleStock()
        //{
        //    //test to check data is returned when one valid stock is requested
        //    var serviceFacade = new StockServiceFacade("AAPL", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "snab");
        //    var result = serviceFacade.GetStockPrices();
        //    Assert.AreEqual(result.Count, 1);
        //}

        //[Test]
        //public void TestValidMultipleStocks()
        //{
        //    //test to check data is returned when multiple valid stock separated by + is requested
        //    var serviceFacade = new StockServiceFacade("AAPL+GOOG+DIS", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "snab");
        //    var result = serviceFacade.GetStockPrices();
        //    Assert.AreEqual(result.Count, 3);
        //}

        //[Test]
        //public void TestValidMultipleStocksCommaSeparted()
        //{
        //    //test to check data is returned when multiple valid stock separated by + is requested
        //    var serviceFacade = new StockServiceFacade("AAPL,GOOG,DIS", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "snab");
        //    var result = serviceFacade.GetStockPrices();
        //    Assert.AreEqual(result.Count, 3);
        //}

        //[Test]
        //public void TestInvalidMultipleStockFormat()
        //{
        //    //test to check no data is returned when multiple valid stock separated by ; instead of + is requested
        //    var serviceFacade = new StockServiceFacade("AAPL;GOOG;DIS", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "snab");
        //    var result = serviceFacade.GetStockPrices();
        //    Assert.AreEqual(result.Count, 3);
        //}

        //[Test]
        //public void TestValidUrlAndFieldsButNotValidStock()
        //{
        //    //test to check data with name as N/A and bid ask as 0 is returned
        //    var serviceFacade = new StockServiceFacade("123", "http://finance.yahoo.com/d/quotes.csv?s={0}&f=", "snab");
        //    var result = serviceFacade.GetStockPrices().First();
        //    Assert.AreEqual(result.Name, "N/A");
        //    Assert.AreEqual(result.Bid, 0);
        //    Assert.AreEqual(result.Ask, 0);
        //}
    }
}
