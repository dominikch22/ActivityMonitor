using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor
{
    class ActiveProgramKey
    {
        public string ProcessName;
        public string WindowTitle;

        public ActiveProgramKey(string processName, string windowTitle)
        {
            ProcessName = processName;
            WindowTitle = windowTitle;
        }

        public override string ToString()
        {
            return ProcessName + " : "+ WindowTitle;
        }

        public override bool Equals(object obj)
        {
            return obj is ActiveProgramKey key &&
                   ProcessName == key.ProcessName &&
                   WindowTitle == key.WindowTitle;
        }

        public override int GetHashCode()
        {
            int hashCode = -765495302;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProcessName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WindowTitle);
            return hashCode;
        }
    }
}
