using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EnDate
        {
            get;
            set;
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
            StartDate = DateTime.Now;
            EnDate = DateTime.Now;
        }

        private bool CanGetHistoricData(object obj)
        {
            if(obj is string)
                return !string.IsNullOrWhiteSpace(obj as string);
            return false;
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
                item => item.Symbol == CsvSymbols && item.AsAtDate >= StartDate.Date && item.AsAtDate <= EnDate.Date).ToList();
            args.Result = results;
        }

        /// <summary>
        /// background woek completed event handler
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
