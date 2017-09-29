using System;
using System.ComponentModel;

namespace Model
{
    public class StockPrice : IComparable<StockPrice>, INotifyPropertyChanged
    {
        //Symbol of the Stock
        public string Symbol { get ; set; }

        //name of the stock
        public string Name { get; set; }

        //LastTradedPrice price of the Stock
        private decimal? _lastTradedPrice;
        public decimal? LastTradedPrice
        {
            get { return _lastTradedPrice; }
            set
            {
                _lastTradedPrice = value;
                OnPropertyChanged("LastTradedPrice");
            }
        }

        //Open price of the Stock
        private decimal? _openPrice;
        public decimal? OpenPrice
        {
            get { return _openPrice; }
            set
            {
                _openPrice = value;
                OnPropertyChanged("OpenPrice");
            }
        }

        //Close price of the Stock
        private decimal? _closePrice;
        public decimal? ClosePrice
        {
            get { return _closePrice; }
            set
            {
                _closePrice = value;
                OnPropertyChanged("ClosePrice");
            }
        }

        //Stock high indicator
        private decimal? _high;
        public decimal? High
        {
            get { return _high; }
            set
            {
                _high = value;
                OnPropertyChanged("High");
            }
        }

        //Stock low indicator
        private decimal? _low;
        public decimal? Low
        {
            get { return _low; }
            set
            {
                _low = value;
                OnPropertyChanged("Low");
            }
        }


        //Volume of the Stock
        private decimal? _tradedVolume;
        public decimal? TradedVolume
        {
            get { return _tradedVolume; }
            set
            {
                _tradedVolume = value;
                OnPropertyChanged("TradedVolume");
            }
        }
       

        //Bid price
        private decimal? _bid;
        public decimal? Bid
        {
            get { return _bid; }
            set
            {
                _bid = value;
                OnPropertyChanged("Bid");
            }
        }
       
        //Ask price
        private decimal? _ask;
        public decimal? Ask
        {
            get { return _ask; }
            set
            {
                _ask = value;
                OnPropertyChanged("Ask");
            }
        }
        
        //Price as at date
        public DateTime PriceAtDate { get; set; }

        //Refreshed//Ask price
        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;
                OnPropertyChanged("TimeStamp");
            }
        } 

        public int CompareTo(StockPrice other)
        {
            if (Symbol == other.Symbol)
            {
                return string.Compare(Symbol, other.Symbol, StringComparison.OrdinalIgnoreCase);
            }

            return string.Compare(other.Symbol, Symbol, StringComparison.OrdinalIgnoreCase);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        } 
    }
}
