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
        public Dictionary<string, HistoryElement> DomainHistory;

        public HistoryData() {
            DomainHistory = new Dictionary<string, HistoryElement>();
        }

        public void loadTodayHistoryFromBrowser() {
            loadHisotryFromEdge();
            loadHistoryFromChrome();

            loadHistoryFromFirefox();
        }

        private void loadHisotryFromEdge() {
            string webCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\History");

            try
            {
                // Connection string for SQLite
                File.Copy(webCachePath, Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString());
                string connectionString = $"Data Source={Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString()};Version=3;";

                // SQL query to get visited URLs from today with timestamps

                string query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= strftime('%s', 'now', 'start of day')";              

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

                                AddElementToHistory(lastVisitTime, domain);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private void loadHistoryFromChrome() {
            try
            {
                string defaultPath = Path.Combine(
           Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
           @"Google\Chrome\User Data\Default\History"
             );
                // Connection string for SQLite database
                File.Copy(defaultPath, Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString());
                string connectionString = $"Data Source={Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString()};Version=3;";

                string path = Application.StartupPath + "\\" + DateTime.Now.Ticks.ToString();
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Get the current date
                    string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

                    // Query to retrieve the browsing history for today
                    DateTime today = DateTime.Now.Date;

                    long startOfTodayTimestamp = (long)(today - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;

                    string query = $"SELECT url, last_visit_time FROM urls WHERE  last_visit_time >= strftime('%s', 'now', 'start of day')";
                    //string query = $"SELECT url, last_visit_time FROM urls";
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
                                //lastVisitTime = (long)(lastVisitTime - (new DateTime(1970, 1, 1) - new DateTime(1601, 1, 1)).TotalMilliseconds *1000);
                               // DateTime dateTimeVar = new DateTime(1601, 1, 1).AddMilliseconds(lastVisitTime/1000);

                                lastVisitTime = lastVisitTime /1000000 - 11644473600;
                                AddElementToHistory(lastVisitTime, domain);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private void loadHistoryFromFirefox() {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\";
            string fileName = "places.sqlite";
            string[] files = Directory.GetFiles(folderPath, fileName, SearchOption.AllDirectories);

            //string firefoxHistoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +  @"\Mozilla\Firefox\Profiles\kilchfst.default-release\places.sqlite";
            if (files.Length == 0)
                return;
            string firefoxHistoryPath = files[0];

            DateTime today = DateTime.Now.Date;
            long startOfTodayTimestamp = (long)(today - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
         
            string query = $"SELECT url, title, last_visit_date FROM moz_places WHERE last_visit_date >= {startOfTodayTimestamp} ORDER BY last_visit_date DESC;";


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
                                AddElementToHistory(timestamp, domain);
                             
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

    }
}
