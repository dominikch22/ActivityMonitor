using Gma.System.MouseKeyHook;
using System;
using System.Windows.Forms;

namespace ActivityMonitor
{
    class KeyListener
    {
        private InputData InputData;

        public KeyListener(InputData inputData) {
            InputData = inputData;
        }

        public void Run() {
            GlobalInputListener globalInputListener = new GlobalInputListener();
            globalInputListener.MouseClick += MouseClick;
            globalInputListener.KeyPressed += KeyPress;
            globalInputListener.MouseMove += MouseMove;

            globalInputListener.Start();
        }

        private void MouseClick() {
            InputData.addMouseClick();
        }

        private void KeyPress() {
            InputData.addKeysPressed();
        }

        private void MouseMove(Point point) {
            InputData.addMouseMovment(point.x, point.y);
        }
  

    }
}
