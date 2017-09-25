using System.ComponentModel;

namespace RealTimeStockPriceViewer.ViewModels
{
    public class BaseViewModel: INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
