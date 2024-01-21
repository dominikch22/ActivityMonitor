using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActivityMonitor
{
    class ActiveProgramListener
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
 

        private ProgramsData ProgramsData;

        public ActiveProgramListener(ProgramsData programsData) {
            ProgramsData = programsData;
        }
        public void Run() {
            while (true) {
                string programName = getActiveWindowTitle();
                if (!programName.Equals(""))
                    ProgramsData.addProgramActivity(programName);

                Thread.Sleep(5000);
            }
        }

        public string getActiveWindowTitle() {
            const int nChars = 256;
            StringBuilder sb = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, sb, nChars) > 0)
            {
                return sb.ToString();
            }

            return "";
        }
    }
}
