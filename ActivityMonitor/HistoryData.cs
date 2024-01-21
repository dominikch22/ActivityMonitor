using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActivityMonitor
{
    class HistoryData
    {
        public long ChromeTimestamp;
        public long FirefoxTimestamp;
        public long EdgeTimestamp;

        public Dictionary<string, HistoryElement> DomainHistory;

        public HistoryData() {
            DomainHistory = new Dictionary<string, HistoryElement>();

   

            DateTime today = DateTime.Now.Date;
            FirefoxTimestamp = (long)(today - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;

            ChromeTimestamp = 0;
            
            EdgeTimestamp = 0;
        }

        public void loadTodayHistoryFromBrowser() {
            loadHisotryFromEdge();
            loadHistoryFromChrome();

            loadHistoryFromFirefox();
        }

        private void loadHisotryFromEdge() {
            string webCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\History");
            string ticks = DateTime.Now.Ticks.ToString();
            try
            {
                File.Copy(webCachePath, Application.StartupPath + "\\" + ticks);
                string connectionString = $"Data Source={Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString()};Version=3;";


                //string query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= strftime('%s', 'now', 'start of day')";
                string query;
                if (EdgeTimestamp == 0)
                    query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= strftime('%s', 'now', 'start of day') ORDER BY last_visit_time";
                else
                    query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= {EdgeTimestamp} ORDER BY last_visit_time";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                string url = reader["url"].ToString();
                                string domain = new Uri(url).Host;
                                long lastVisitTime = Convert.ToInt64(reader["last_visit_time"]);

                                DateTime dt = new DateTime(1601, 01, 01).AddTicks(lastVisitTime * 10);
                                DateTimeOffset dateTimeOffset = new DateTimeOffset(dt);
                                long milli = dateTimeOffset.ToUnixTimeMilliseconds();
                                if (lastVisitTime > EdgeTimestamp)
                                {
                                    AddElementToHistory(milli, domain);
                                    EdgeTimestamp = lastVisitTime;
                                }


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally {
                File.Delete(Application.StartupPath + "\\" + ticks);
            }
        }
        private void loadHistoryFromChrome() {

            string ticks = DateTime.Now.Ticks.ToString();
            try
            {
                string defaultPath = Path.Combine(
           Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
           @"Google\Chrome\User Data\Default\History"
             );
                File.Copy(defaultPath, Application.StartupPath + "\\" + ticks);
                string connectionString = $"Data Source={Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString()};Version=3;";

                string path = Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString();
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

                    DateTime today = DateTime.Now.Date;

                    long startOfTodayTimestamp = (long)(today - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;

                    string query;
                    if (ChromeTimestamp == 0)
                        query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= strftime('%s', 'now', 'start of day') ORDER BY last_visit_time";
                    else
                        query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= {ChromeTimestamp} ORDER BY last_visit_time";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string url = reader["url"].ToString();
                                string domain = new Uri(url).Host;
                                string x = reader["last_visit_time"].ToString();
                                long lastVisitTime = Convert.ToInt64(reader["last_visit_time"]);

                                DateTime dt = new DateTime(1601, 01, 01).AddTicks(lastVisitTime*10);
                                DateTimeOffset dateTimeOffset = new DateTimeOffset(dt);
                                long milli = dateTimeOffset.ToUnixTimeMilliseconds();
                                if (lastVisitTime > ChromeTimestamp)
                                {
                                    AddElementToHistory(milli, domain);
                                    ChromeTimestamp = lastVisitTime;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally {
                File.Delete(Application.StartupPath + "\\" + ticks);
            }
        }

        private void loadHistoryFromFirefox() {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\";
            string fileName = "places.sqlite";
            string[] files = Directory.GetFiles(folderPath, fileName, SearchOption.AllDirectories);

            if (files.Length == 0)
                return;
            string firefoxHistoryPath = files[0];

            //DateTime today = DateTime.Now.Date;
           // long startOfTodayTimestamp = (long)(today - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
         
            //string query = $"SELECT url, title, last_visit_date FROM moz_places WHERE last_visit_date >= {startOfTodayTimestamp} ORDER BY last_visit_date DESC;";
            string query = $"SELECT url, title, last_visit_date FROM moz_places WHERE last_visit_date >= {FirefoxTimestamp} ORDER BY last_visit_date;";

            try
            {
              
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={firefoxHistoryPath};Version=3;"))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string url = reader["url"].ToString();
                                string domain = new Uri(url).Host;
                                long timestamp = Convert.ToInt64(reader["last_visit_date"]);
                                if (timestamp > FirefoxTimestamp) {
                                    AddElementToHistory(timestamp, domain);
                                    FirefoxTimestamp = timestamp;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddElementToHistory(long timestamp, string domain) {
            DateTime creationTime;           
            creationTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp / 1000).DateTime;


            HistoryElement historyElement = new HistoryElement(domain);

            if (!DomainHistory.ContainsKey(domain))
            {
                DomainHistory.Add(domain, historyElement);
            }
            DomainHistory.TryGetValue(domain, out historyElement);
            int minuteTime = creationTime.Hour * 60 + creationTime.Minute;

            int count = 0;
            historyElement.history.TryGetValue(minuteTime, out count);
            historyElement.history.Remove(minuteTime);
            historyElement.history.Add(minuteTime, count + 1);

        }

        public void SaveDomainsHistory()
        {
            string json = JsonConvert.SerializeObject(this);

            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");

            string path = Directory.GetCurrentDirectory() + "/data" + "/" + formattedDate;
            try
            {
                Directory.CreateDirectory(path);
                File.WriteAllText(path + "/domainsHistory.json", json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static HistoryData LoadHistryDataByDate(string formattedDate) {
            HistoryData historyData;
            try
            {
                string path = Directory.GetCurrentDirectory() + "/data" + "/" + formattedDate + "/domainsHistory.json";
                string jsonContent = File.ReadAllText(path);
                historyData = JsonConvert.DeserializeObject<HistoryData>(jsonContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                historyData = new HistoryData();

            }


            return historyData;
        }

    
    }
}
