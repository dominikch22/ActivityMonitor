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
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            InputData = new InputData();

            KeyListener keyListener = new KeyListener(InputData);
            Thread thread = new Thread(keyListener.Run);
            thread.Start();


            CreateCalender();

            keyPressedChart.Series[0].Points.Clear();
            mouseClickChart.Series[0].Points.Clear();
            mouseDistanceChart.Series[0].Points.Clear();
            for (int i = 0; i < InputData.MouseMoves.Length; i++)
            {
                keyPressedChart.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                mouseClickChart.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                mouseDistanceChart.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();


            }

            addEleentsToListBoxes();

        }

        private void addEleentsToListBoxes() {
            for (int i = 1; i <= 100; i++)
            {
                ActiveProgramsList.Items.Add($"Item {i}");
                DomainsHistory.Items.Add($"Item {i}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lock (InputData.LockObject)
            {
                keyPressedChart.Series[0].Points.Clear();
                mouseClickChart.Series[0].Points.Clear();
                mouseDistanceChart.Series[0].Points.Clear();
                for (int i = 0; i < InputData.MouseMoves.Length; i++)
                {
                    keyPressedChart.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                    mouseClickChart.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();
                    mouseDistanceChart.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i / 60).ToString() + ":" + (i % 60).ToString();


                }
            }
        }

        private void CreateCalender() {
            System.Windows.Controls.Grid calenderGrid = (System.Windows.Controls.Grid)this.FindName("CalenderGrid");
            List<string> dates = GenerateDates(60);
            int count = 59;
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Button button = new Button
                    {
                        Content = $" {row * 10 + col + 1}",
                        Margin = new Thickness(2),
                        Width = 25,
                        Height = 25,
                        Background = Brushes.White,
                        ToolTip = dates[count],
                        //HorizontalAlignment = HorizontalAlignment.Stretch,
                        //VerticalAlignment = VerticalAlignment.Stretch
                    };
                    count--;

                    System.Windows.Controls.Grid.SetRow(button, row);
                    System.Windows.Controls.Grid.SetColumn(button, col);

                    // Attach an event handler if needed
                    button.Click += Button_Click;

                    // Add the button to the grid
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
            // Handle the selection change event here
            if (DomainsHistory.SelectedItem != null)
            {
                MessageBox.Show($"You clicked on: {DomainsHistory.SelectedItem.ToString()}");
            }
        }

        private void ActiveProgramsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Handle the selection change event here
            if (ActiveProgramsList.SelectedItem != null)
            {
                MessageBox.Show($"You clicked on: {ActiveProgramsList.SelectedItem.ToString()}");
            }
        }

    }
}
