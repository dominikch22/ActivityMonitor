using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void addProgramActivity(string key) {
            List<int> history;
            if (ActiveProgramsHistory.ContainsKey(key)) {
                ActiveProgramsHistory.TryGetValue(key, out history);
                history.Add(getCurrentMinute());
            }
            else {
                ActiveProgramsHistory.Add(key, new List<int> { getCurrentMinute()});
            }
        }

        public void SaveProgramsData() {
            string json = JsonConvert.SerializeObject(this);

            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");

            string path = Directory.GetCurrentDirectory() + "/data" + "/" + formattedDate;
            try
            {
                Directory.CreateDirectory(path);
                File.WriteAllText(path + "/programsData.json", json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static ProgramsData LaodProgramDataByDate(string formattedDate) {
           

            ProgramsData programsData;
            try
            {
                string path = Directory.GetCurrentDirectory() + "/data" + "/" + formattedDate + "/programsData.json";
                string jsonContent = File.ReadAllText(path);
                programsData = JsonConvert.DeserializeObject<ProgramsData>(jsonContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                programsData = new ProgramsData();
            }


            return programsData;
        }
    }

}
