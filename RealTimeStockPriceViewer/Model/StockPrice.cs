using System;

namespace Model
{
    public class StockPrice : IComparable<StockPrice>
    {
        //Symbol of the Stock
        public string Symbol { get; set; }

        //LastTradedPrice price of the Stock
        public decimal? LastTradedPrice { get; set; }

        //ClosePrice price of the Stock
        public decimal? OpenPrice { get; set; }
        
        //Volume of the Stock
        public decimal? TradedVolume { get; set; }

        //Bid price
        public decimal? Bid { get; set; }

        //Ask price
        public decimal? Ask { get; set; }

        public int CompareTo(StockPrice other)
        {
            if (Symbol == other.Symbol)
            {
                return string.Compare(Symbol, other.Symbol, StringComparison.OrdinalIgnoreCase);
            }

            return string.Compare(other.Symbol, Symbol, StringComparison.OrdinalIgnoreCase);
        }
    }
}
