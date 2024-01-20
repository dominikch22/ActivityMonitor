using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActivityMonitor
{
    class InputData
    {
        private int lastX;
        private int lastY;

        public readonly object LockObject = new object();
        public int[] MouseClicks;
        public int[] MouseMoves;
        public int[] KeysPressed;

        public InputData() {
            MouseClicks = new int[1440];
            MouseMoves = new int[1440];
            KeysPressed = new int[1440];

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
    }
}
