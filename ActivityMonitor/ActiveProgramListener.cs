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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        
        private ProgramsData ProgramsData;

        public ActiveProgramListener(ProgramsData programsData) {
            ProgramsData = programsData;
        }
        public void Run() {
            while (true) {
                string programName = GetActiveProgramName();
                string windowsTitle = getActiveWindowTitle();
                ActiveProgramKey key = new ActiveProgramKey(programName, windowsTitle);
                if (!windowsTitle.Equals(""))
                    ProgramsData.addProgramActivity(key);

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

        public string GetActiveProgramName()
        {
            IntPtr handle = GetForegroundWindow();
            uint processId;

            GetWindowThreadProcessId(handle, out processId);

            try
            {
                Process process = Process.GetProcessById((int)processId);
                return process.ProcessName;
            }
            catch (ArgumentException)
            {
                return "Unable to retrieve the active program name.";
            }
        }
    }
}
