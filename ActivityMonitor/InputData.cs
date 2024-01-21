﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActivityMonitor
{
    class InputData
    {
        [JsonIgnore]
        private int lastX;
        [JsonIgnore]
        private int lastY;
        [JsonIgnore]
        public readonly object LockObject;

        public int[] MouseClicks;
        public int[] MouseMoves;
        public int[] KeysPressed;

        public InputData() {
            MouseClicks = new int[1440];
            MouseMoves = new int[1440];
            KeysPressed = new int[1440];

            LockObject = new object();

            lastX = 0;
            lastY = 0;
        }

        public void addKeysPressed() {
            lock (LockObject)
            {
                KeysPressed[getCurrentMinute()]++;
            }
        }

        public void addMouseClick() {
            lock (LockObject)
            {
                MouseClicks[getCurrentMinute()]++;
            }
        }

        public void addMouseMovment(int x, int y) {
            lock (LockObject) {
                int distance = (int)Math.Sqrt(Math.Pow(x - lastX, 2) + Math.Pow(y - lastY, 2));

                lastX = x;
                lastY = y;

                MouseMoves[getCurrentMinute()] += distance;
            }          
        }

        public int getCurrentMinute() {
            DateTime currentTime = DateTime.Now;
            return currentTime.Hour * 60 + currentTime.Minute;
        }

        public void saveInputData() {
            string json = JsonConvert.SerializeObject(this);

            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");

            string path = Directory.GetCurrentDirectory() + "/data" + "/" + formattedDate;
            try
            {
                Directory.CreateDirectory(path);
                File.WriteAllText(path+"/inputData.json", json);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static InputData LoadDataByDate(string formattedDate) {
            string path = Directory.GetCurrentDirectory() + "/data" + "/" + formattedDate;
            string jsonContent = File.ReadAllText(path);

            InputData inputData;
            try {
                inputData = JsonConvert.DeserializeObject<InputData>(jsonContent);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            inputData = new InputData();


            return inputData;
        }
    }
}