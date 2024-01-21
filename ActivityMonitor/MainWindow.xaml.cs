using SharpPcap;
using System;
using System.Collections.Generic;
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

using DataVis = System.Windows.Forms.DataVisualization;

using System;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Windows.Forms.DataVisualization.Charting;

namespace ActivityMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputData InputData;
        private ProgramsData ProgramsData;
        private HistoryData HistoryData;
        private InputData SelectedInputData;
        private ProgramsData SelectedProgramsData;
        private HistoryData SelectedHistoryData;
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Closing += this.MainWindow_Closing;

            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");
            LoadData(formattedDate);

            KeyListener keyListener = new KeyListener(InputData);
            Thread thread = new Thread(keyListener.Run);
            thread.Start();

            ActiveProgramListener active = new ActiveProgramListener(ProgramsData);
            Thread activeProgramThread = new Thread(active.Run);
            activeProgramThread.Start();

            HistoryUpdator HistoryUpdator = new HistoryUpdator(HistoryData);
            Thread HistoryUpdatorThread = new Thread(HistoryUpdator.Run);
            HistoryUpdatorThread.Start();

            HistoryData.loadTodayHistoryFromBrowser();
            CreateCalendar();

          

            InputData.saveInputData();           

            AddTitles();

            loadDataIntoCharts(formattedDate);
           

        }

        private void LoadData(string formattedDate)
        {
            

            InputData = InputData.LoadDataByDate(formattedDate);
            ProgramsData = ProgramsData.LaodProgramDataByDate(formattedDate);
            HistoryData = HistoryData.LoadHistryDataByDate(formattedDate);
        }

        private void AddTitles() {
            keyPressedChart.Titles.Add("Naciśnięte klawisze");
            mouseClickChart.Titles.Add("Kliknięcia myszy");
            mouseDistanceChart.Titles.Add("Dystans myszy");
            domainsHistoryChart.Titles.Add("Czas użycia domeny");
            ActiveProgramsChart.Titles.Add("Czas używania programu");
        }

        public void ActivateInputData() {
            keyPressedGrid.Visibility = Visibility.Visible;
            mouseClickGrid.Visibility = Visibility.Visible;
            mouseDistanceGrid.Visibility = Visibility.Visible;

            domainsHistoryGrid.Visibility = Visibility.Hidden;
            ActiveProgramsGrid.Visibility = Visibility.Hidden;

            inputButton.Background = Brushes.DeepSkyBlue;
            historyButton.Background = Brushes.LightGray;
        }

        public void inputData_click(object sender, RoutedEventArgs e) {
            ActivateInputData();
        }

        public void historyData_click(object sender, RoutedEventArgs e) {
            ActivateProgramsAndDomainData();
        }

        public void ActivateProgramsAndDomainData() {
            keyPressedGrid.Visibility = Visibility.Hidden;
            mouseClickGrid.Visibility = Visibility.Hidden;
            mouseDistanceGrid.Visibility = Visibility.Hidden;

            domainsHistoryGrid.Visibility = Visibility.Visible;
            ActiveProgramsGrid.Visibility = Visibility.Visible;

            historyButton.Background = Brushes.DeepSkyBlue;
            inputButton.Background = Brushes.LightGray;

        }
     

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            ActivateInputData();

            string formattedDate = ((Button)sender).ToolTip.ToString();
            loadDataIntoCharts(formattedDate);        
        }

        private void loadDataIntoCharts(string formattedDate) {
            SelectedInputData = InputData.LoadDataByDate(formattedDate);
            SelectedHistoryData = HistoryData.LoadHistryDataByDate(formattedDate);
            SelectedProgramsData = ProgramsData.LaodProgramDataByDate(formattedDate);

            keyPressedChart.Series[0].Points.Clear();
            mouseClickChart.Series[0].Points.Clear();
            mouseDistanceChart.Series[0].Points.Clear();

            lock (SelectedInputData.LockObject)
            {
                keyPressedChart.Series[0].Points.Clear();
                mouseClickChart.Series[0].Points.Clear();
                mouseDistanceChart.Series[0].Points.Clear();
                for (int i = 0; i < SelectedInputData.MouseMoves.Length; i++)
                {
                    mouseDistanceChart.Series[0].Points.Add(SelectedInputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                    keyPressedChart.Series[0].Points.Add(SelectedInputData.KeysPressed[i], i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                    mouseClickChart.Series[0].Points.Add(SelectedInputData.MouseClicks[i], i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                }
            }

            DomainsHistory.Items.Clear();
            foreach (KeyValuePair<string, HistoryElement> entry in SelectedHistoryData.DomainHistory) {
                int count = 0;
                foreach(KeyValuePair<int, int> nextEntry in entry.Value.history) {
                    count += nextEntry.Value;
                }
                //DomainsHistory.Items.Add($"{entry.Key} : {count}");
                ListBoxItemData item = new ListBoxItemData($"{entry.Key} : {count}", entry.Key);
                DomainsHistory.Items.Add(item);
            }

            ActiveProgramsList.Items.Clear();
            foreach (KeyValuePair<string, List<int>> entry in SelectedProgramsData.ActiveProgramsHistory)
            {
                int count = 0;
                int lastMinute = 0;
                foreach (int minute in entry.Value)
                {
                    if (lastMinute != minute)
                        count++;
                }
                //ActiveProgramsList.Items.Add($"{entry.Key} : {count}");
                ListBoxItemData item = new ListBoxItemData($"{entry.Key} : {count}", entry.Key);
                ActiveProgramsList.Items.Add(item);
            }
        }

        private void CreateCalendar() {
            System.Windows.Controls.Grid calenderGrid = (System.Windows.Controls.Grid)this.FindName("CalenderGrid");
            List<string> dates = GenerateDates(60);
            int count = 59;
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Button button = new Button
                    {
                        Content = $"{dates[count].Substring(0,2)}",
                        Margin = new Thickness(2),
                        Width = 25,
                        Height = 25,
                        Background = Brushes.White,
                        ToolTip = dates[count],                      
                    };
                    count--;

                    System.Windows.Controls.Grid.SetRow(button, row);
                    System.Windows.Controls.Grid.SetColumn(button, col);

                    button.Click += Calendar_Click;

                    calenderGrid.Children.Add(button);
                }
            }
        }

        static List<string> GenerateDates(int numberOfDays)
        {
            List<string> dates = new List<string>();

            DateTime currentDate = DateTime.Now;

            for (int i = 0; i < numberOfDays; i++)
            {
                dates.Add(currentDate.ToString("dd-MM-yyyy"));
                currentDate = currentDate.AddDays(-1);
            }

            return dates;
        }

        private void DomainsHistoryList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ActivateProgramsAndDomainData();

            if (DomainsHistory.SelectedItem != null)
            {
                ListBoxItemData item = (ListBoxItemData)DomainsHistory.SelectedItem;
                Dictionary<int, int> domainHistory;
                HistoryElement historyElement;
                SelectedHistoryData.DomainHistory.TryGetValue(item.AdditionalData, out historyElement);
                domainHistory = historyElement.history;
                domainsHistoryChart.Series[0].Points.Clear();
                foreach (KeyValuePair<int, int> entry in domainHistory) {
                    domainsHistoryChart.Series[0].Points.Add(entry.Value, entry.Key).AxisLabel = (entry.Key / 60).ToString() + ":" + (entry.Key % 60).ToString();
                }
            }
        }



        private void ActiveProgramsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ActivateProgramsAndDomainData();
            if (ActiveProgramsList.SelectedItem != null)
            {
                ListBoxItemData item = (ListBoxItemData)ActiveProgramsList.SelectedItem;

                List<int> programHistory;
                SelectedProgramsData.ActiveProgramsHistory.TryGetValue(item.AdditionalData, out programHistory);
                ActiveProgramsChart.Series[0].Points.Clear();
                for (int i = 0; i < 1440; i++) {
                    ActiveProgramsChart.Series[0].Points.Add(0, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                }
                for (int i = 0; i < programHistory.Count(); i++) {
                    ActiveProgramsChart.Series[0].Points.Add(1, programHistory[i]).AxisLabel = (programHistory[i] / 60).ToString() + ":" + (programHistory[i] % 60).ToString();
                }

            }
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
            Environment.Exit(0);
        }

        private void SaveData()
        {
            InputData.saveInputData();
            ProgramsData.SaveProgramsData();
            HistoryData.SaveDomainsHistory();
        }


    }
}
