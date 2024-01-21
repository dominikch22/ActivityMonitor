using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor
{
    class ProgramsData
    {
        public Dictionary<string, List<int>> ActiveProgramsHistory;

        public ProgramsData() {
            ActiveProgramsHistory = new Dictionary<string, List<int>>();
        }

        public int getCurrentMinute()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime.Hour * 60 + currentTime.Minute;
        }
        public void addProgramActivity(string windowTitle) {
            List<int> history;
            if (ActiveProgramsHistory.ContainsKey(windowTitle)) {
                ActiveProgramsHistory.TryGetValue(windowTitle, out history);
                history.Add(getCurrentMinute());
            }
            else {
                ActiveProgramsHistory.Add(windowTitle, new List<int> { getCurrentMinute()});
            }
        }
    }
}
