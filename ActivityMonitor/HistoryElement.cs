using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor
{
    class HistoryElement
    {
        public string domain;
        public Dictionary<int, int> history;

        public HistoryElement(String domain) {
            this.domain = domain;
            history = new Dictionary<int, int>();
        }
    }
}
