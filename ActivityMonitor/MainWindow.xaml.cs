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
        public MainWindow()
        {
            InitializeComponent();

            InputData = new InputData();
            ProgramsData = new ProgramsData();

            KeyListener keyListener = new KeyListener(InputData);
            Thread thread = new Thread(keyListener.Run);
            thread.Start();

            ActiveProgramListener active = new ActiveProgramListener(ProgramsData);
            Thread activeProgramThread = new Thread(active.Run);
            activeProgramThread.Start();

            HistoryData history = new HistoryData();
            history.loadTodayHistoryFromBrowser();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lock (InputData.LockObject)
            {
                chart1.Series[0].Points.Clear();
                for (int i = 0; i < InputData.MouseMoves.Length; i++)
                {
                    chart1.Series[0].Points.Add(InputData.MouseMoves[i] / 1000, i).AxisLabel = (i/60).ToString()+":"+(i%60).ToString();

                }
                InputData.saveInputData();
            }
        }

    }
}
