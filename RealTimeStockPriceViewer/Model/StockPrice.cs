using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StockPrice
    {
        //Symbol of the Stock
        public string Symbol { get; set; }

        //Name of the Stock
        public string Name { get; set; }

        //LastTradedPrice price of the Stock
        public decimal LastTradedPrice { get; set; }

        //ClosePrice price of the Stock
        public decimal OpenPrice { get; set; }
        
        //Volume of the Stock
        public decimal TradedVolume { get; set; }

        //Bid price
        public decimal Bid { get; set; }

        //Ask price
        public decimal Ask { get; set; }
    }
}
