using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Commands;
using Model;

namespace RealTimeStockPriceViewer.ViewModels
{
    public class HistoricStockPriceViewModel : BaseViewModel
    {
        public string CsvSymbols
        {
            get;
            set;
        }

        private DateTime _startDate;
        private DateTime _endDate; 
        
        [Required(ErrorMessage = "Start Date is a required field.")]
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {

                if (_startDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                {

                    if (value > _endDate)
                    {
                        MessageBox.Show("Start date must be less than End date");
                        value = this.StartDate;
                    }
                }
                _startDate = value;

                OnPropertyChanged("StartDate");
            }
        }

        [Required(ErrorMessage = "End Date is a required field.")]
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_startDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                {
                    if (_startDate > value)
                    {
                        MessageBox.Show("End date must be after Start date");
                        value = this.EndDate;
                    }
                }

                _endDate = value;

                OnPropertyChanged("EndDate");
            }
        }
        public ObservableCollection<HistoricPrice> HistoricStockPrices { get; set; }

        private readonly HistoricDataEntities _historicEntity;

        public RelayCommand GetHistoricDataCommand { get; set; }

        public HistoricStockPriceViewModel()
        {
           //initialize the GetHistoricData command
            GetHistoricDataCommand = new RelayCommand(GetHistoricData, CanGetHistoricData);

            //initialize the observable collection
            HistoricStockPrices = new ObservableCollection<HistoricPrice>();

            //initialize the Entityframework context
            _historicEntity = new HistoricDataEntities();

            //set the default date to current day
            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;
        }

        private bool CanGetHistoricData(object obj)
        {
            if(string.IsNullOrWhiteSpace(CsvSymbols) || StartDate <=DateTime.MinValue || EndDate <= DateTime.MinValue)
            return false;
            return true;
        }

        private void GetHistoricData(object obj)
        {
            GetPricesAsync();
        }

        private void GetPricesAsync()
        {
            HistoricStockPrices.Clear();
            var bgworker = new BackgroundWorker();
            bgworker.DoWork += GetPrices;
            bgworker.RunWorkerCompleted += bgworker_RunWorkerCompleted;
            bgworker.RunWorkerAsync();
        }

        /// <summary>
        /// Background workers dowork method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GetPrices(object sender, DoWorkEventArgs args)
        {
            var results = _historicEntity.HistoricPrices.Where(
                item => item.Symbol == CsvSymbols && item.AsAtDate >= StartDate.Date && item.AsAtDate <= EndDate.Date).ToList();
            args.Result = results;
        }

        /// <summary>
        /// background work completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="runWorkEventArgs"></param>
        void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkEventArgs)
        {
            var stockListResults = runWorkEventArgs.Result as List<HistoricPrice>;
            UpdateDataCollection(stockListResults);
        }

        private void UpdateDataCollection(List<HistoricPrice> historicStock)
        {
            if (historicStock.Any())
            {
                historicStock.ForEach(historicItem => HistoricStockPrices.Add(historicItem));
            }
            else
            {
                ViewModelHelper.ShowMessage("No data found, Please modify the search paramaters and try again.", "Info");
            }
        }
    }
}
