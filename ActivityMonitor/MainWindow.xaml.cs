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
        public ObservableCollection<DataPoint> DataPoints { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            InputData inputData = new InputData();

            KeyListener keyListener = new KeyListener(inputData);
            Thread thread = new Thread(keyListener.Run);
            thread.Start();


            DataPoints = new ObservableCollection<DataPoint>
            {
                new DataPoint { X = 1, Y = 10 },
                new DataPoint { X = 2, Y = 20 },
                new DataPoint { X = 3, Y = 15 },
                new DataPoint { X = 4, Y = 25 },
            };

            lock (inputData.LockObject) {
                for (int i = 0; i < inputData.MouseMoves.Length; i++) {
                    DataPoints.Add(new DataPoint { })
                }
            }

            // Ustawianie źródła danych dla wykresu
            LineSeries lineSeries = new LineSeries();
            lineSeries.ItemsSource = DataPoints;

            // Dodawanie wykresu do kontrolki Chart
            chart.Series.Add(lineSeries);


        }

    }
}
