using COMP229_FinalTerm.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace COMP229_FinalTerm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ==================
        // ADD YOUR CODE HERE
        // ==================
        // Q1.4

        AppDbContext _dbContext = new AppDbContext();

        

        // ================== 
        private string _filename = "";

        public MainWindow()
        {
            InitializeComponent();
            // ==================
            // ADD YOUR CODE HERE
            // ==================
            // Q1.5
            //_dbContext.Database.EnsureCreated();

            // ================== 
        }


        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                _filename = openFileDialog.FileName;
                txtFileLoaded.Text = _filename;               
            }             
        }

        private void btnInsertData_Click(object sender, RoutedEventArgs e)
        {
            // Runs insertDataIntoDatabase() in another thread and notifies the ProgressBar.
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += insertDataIntoDatabase;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        private void insertDataIntoDatabase(object sender, DoWorkEventArgs e) 
        {
            try
            {
                foreach(var item in _dbContext.Province)
                {
                    _dbContext.Province.Remove(item);
                }

                foreach (var item in _dbContext.CasesReports)
                {
                    _dbContext.CasesReports.Remove(item);
                }

                _dbContext.SaveChanges();


                (sender as BackgroundWorker).ReportProgress(10);
                string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(_filename, ".csv"));
                                
                string[] columns = lines[0].Split(',');
                                
                (sender as BackgroundWorker).ReportProgress(20);

                // Inserts Provinces
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] cell = lines[i].Split(',');

                    Province? province = _dbContext.Province.FirstOrDefault(
                        c => c.ProvinceName == cell[0] &
                        c.CountryName == cell[1]);

                    if (province == null)
                    {
                        province = new()
                        {
                            ProvinceName = cell[0],
                            Latitude = cell[2] != "" ? Convert.ToDouble(cell[2]) : 0,
                            Longitude = cell[3] != "" ? Convert.ToDouble(cell[3]) : 0,
                            CountryName = cell[1],
                        };
                        _dbContext.Province.Add(province);
                        _dbContext.SaveChanges();
                    }
                }

                // Notifies the progress bar
                (sender as BackgroundWorker).ReportProgress(40);


                // Inserts Cases Reports
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] cell = lines[i].Split(',');

                    // ==================
                    // ADD YOUR CODE HERE
                    // ==================
                    // Q2.1 id not need

                    CasesReport? casesReport = _dbContext.CasesReports.Where(
                        (c) => c.Province.ProvinceName == cell[0] &
                        c.Province.CountryName == cell[1]).FirstOrDefault();

                    Province? province = _dbContext.Province.FirstOrDefault(
                        c => c.ProvinceName == cell[0] &
                        c.CountryName == cell[1]);

                    // ================== 

                    //  Inserts Cases Data thru the columns          
                    for (int j = 4; j < columns.Length; j++)
                    {
                        string[] col = columns[j].Split(',');

                        string expectedFormat = "m/d/yyyy";

                        int amount = Convert.ToInt32(cell[j]);

                        // ==================
                        // ADD YOUR CODE HERE
                        // ==================
                        // Q2.1 and Q2.2 dont save change. just add.

                        casesReport = new()
                        {
                            date = DateTime.ParseExact(col[0], expectedFormat, CultureInfo.InvariantCulture),
                            amount = cell[j] != "" ? Convert.ToInt32(cell[j]) : 0,
                            Province = province
                        };

                        _dbContext.CasesReports.Add(casesReport);

                        // ================== 
                    }
                }

                // Notifies the progress bar
                (sender as BackgroundWorker).ReportProgress(80);


                // Saves the final changes
                // ==================
                // ADD YOUR CODE HERE
                // ==================
                // Q2.3 save here
                _dbContext.SaveChanges();
                // ==================


                // Notifies the progress bar
                (sender as BackgroundWorker).ReportProgress(100);                
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbInsertDb.Value = e.ProgressPercentage;

            if (e.ProgressPercentage == 100)
            {
                txtBar.Text = "Completed";
                MessageBox.Show("All data has been inserted successfully into the database.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }            
        }

        private void btnLoadCountries_Click(object sender, RoutedEventArgs e)
        {
            var countries = (from p in _dbContext.Province
                             select p.CountryName)
                           .Distinct()
                           .ToList();

            cbCountries.ItemsSource = countries;

            MessageBox.Show("All countires has been loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void cbCountries_DropDownClosed(object sender, EventArgs e)
        {
            cbGroupResult.IsChecked = false;
            if (cbCountries.SelectedItem != null)
            {
                try
                {
                    // ==================
                    // ADD YOUR CODE HERE
                    // ==================
                    // Q3.2 linq above or for loop?

                    var _CountryName = cbCountries.SelectedItem;

                    var provinces = (from p in _dbContext.Province
                                     where p.CountryName == _CountryName
                                     select p.ProvinceName)
                           .Distinct()
                           .ToList();

                    //foreach (var province in provinces)
                    //{
                    //    cbProvinces.Items.Add(province);
                    //}

                    cbProvinces.ItemsSource = provinces;

                    // ==================

                    // ==================
                    // ADD YOUR CODE HERE
                    // ==================
                    // Q3.3 populate the grid and province

                    var query = from c in _dbContext.CasesReports
                                where _CountryName == c.Province.CountryName
                                select new
                                {
                                    CasesReportId = c.CasesReportId,
                                    date = c.date,
                                    amount = c.amount,
                                    ProvinceName = c.Province.ProvinceName
                                };
                    var results = query.ToList();
                    dgCases.ItemsSource = results;


                    // ==================

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cbProvinces_DropDownClosed(object sender, EventArgs e)
        {
            cbGroupResult.IsChecked = false;
            if (cbProvinces.SelectedItem != null)
                try
                {
                    // ==================
                    // ADD YOUR CODE HERE
                    // ==================
                    // Q3.4 populate only data besed on province
                    var _ProvinceName = cbProvinces.SelectedItem;

                    var query = from c in _dbContext.CasesReports
                                where _ProvinceName == c.Province.ProvinceName
                                select new
                                {
                                    CasesReportId = c.CasesReportId,
                                    date = c.date,
                                    amount = c.amount,
                                    ProvinceName = c.Province.ProvinceName
                                };
                    var results = query.ToList();
                    dgCases.ItemsSource = results;


                    // ==================
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }

        private void cbGroupResult_Checked(object sender, RoutedEventArgs e)
        {
            if (cbProvinces.SelectedItem != null)
                try
                {
                    // ==================
                    // ADD YOUR CODE HERE
                    // ==================
                    // Q3.5 


                    // ==================
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            else { 
                cbGroupResult.IsChecked = false; 
                MessageBox.Show("Please, select one province.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
