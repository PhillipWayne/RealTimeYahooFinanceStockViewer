using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
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
        #region Properties
        //Stock sybmol
        public string Symbol
        {
            get;
            set;
        }

        //Coma separated symbols
        public string[] CsvStockSymbols
        {
            get;
            set;
        }

        /// <summary>
        /// Start date to load history
        /// </summary>
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

        /// <summary>
        /// End date to load history
        /// </summary>
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

        //Observable collection of entity model to store the historic data
        public ObservableCollection<HistoricPrice> HistoricStockPrices { get; set; }

        //grid row count
        private int _rowcount;

        public int RowCount
        {
            get { return _rowcount; }
            set
            {
                _rowcount = value;
                OnPropertyChanged("RowCount");
            }
        }

#endregion

        #region Fields

        private readonly HistoricDataEntities _historicEntity;
        public RelayCommand GetHistoricDataCommand { get; set; }

        private DateTime _startDate;
        private DateTime _endDate;
        #endregion

        #region Constructor
        public HistoricStockPriceViewModel()
        {
            //initialize the GetHistoricData command
            GetHistoricDataCommand = new RelayCommand(GetHistoricData, CanGetHistoricData);

            //initialize the observable collection
            HistoricStockPrices = new ObservableCollection<HistoricPrice>();

            //initialize the Entityframework context
            _historicEntity = new HistoricDataEntities();

            //set the dates default to one month difference
            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;

            //get the symbols from the config, this can be inclued to be taken from UI as well
            CsvStockSymbols = ConfigurationManager.AppSettings["CsvSymbols"].Split(',');

        }
        #endregion

        #region Methods
        private bool CanGetHistoricData(object obj)
        {
            if(string.IsNullOrWhiteSpace(Symbol) || StartDate <=DateTime.MinValue || EndDate <= DateTime.MinValue)
            return false;
            return true;
        }

        private void GetHistoricData(object obj)
        {
            GetPricesAsync();
        }

        /// <summary>
        /// Get Data asynchronously
        /// </summary>
        private void GetPricesAsync()
        {
            //clear the observable collection
            HistoricStockPrices.Clear();
            RowCount = HistoricStockPrices.Count();

            //start a new background thread
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
                item => item.Symbol == Symbol && item.AsAtDate >= StartDate.Date && item.AsAtDate <= EndDate.Date).ToList();
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
            RowCount = HistoricStockPrices.Count();
            ViewModelHelper.ShowMessage(HistoricStockPrices.Any()? string.Format("{0} Rows Loaded Successfully", RowCount) :
                "No data found, Please modify the search paramaters and try again." , "Info");
        }

        /// <summary>
        /// Update the observable collection
        /// </summary>
        /// <param name="historicStock"></param>
        private void UpdateDataCollection(List<HistoricPrice> historicStock)
        {
            if (historicStock.Any())
            {
                historicStock.ForEach(historicItem => HistoricStockPrices.Add(historicItem));
            }
        }
        #endregion
    }
}
