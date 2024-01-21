using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActivityMonitor
{
    class HistoryUpdator
    {
        public HistoryData HistoryData;

        public HistoryUpdator(HistoryData historyData)
        {
            HistoryData = historyData;
        }

        public void Run() {
            while (true) {
                Thread.Sleep(1000 * 60 * 1);
                HistoryData.loadTodayHistoryFromBrowser();
                HistoryData.SaveDomainsHistory();
                
            }
        }
    }
}
