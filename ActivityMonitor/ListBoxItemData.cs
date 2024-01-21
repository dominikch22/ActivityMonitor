using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor
{
    class ListBoxItemData
    {
        public string DisplayText { get; set; }
        public string AdditionalData { get; set; }

        public ListBoxItemData(string displayText, string additionalData)
        {
            DisplayText = displayText;
            AdditionalData = additionalData;
        }
    }
}
