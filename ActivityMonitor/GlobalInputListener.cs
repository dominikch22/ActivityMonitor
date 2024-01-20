using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActivityMonitor
{
    public class GlobalInputListener
    {
        private bool MousedIsPressed;
        private POINT LastMousePosition;

        public event Action KeyPressed;
        public event Action MouseClick;
        public event Action<Point> MouseMove;


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public struct POINT
        {
            public int X;
            public int Y;
        }

        public void Start()
        {
            while (true)
            {
                onMouseClick();
                onKeyPressed();
                onMouseMove();

                Thread.Sleep(50);
            }

        }

        private void onMouseClick()
        {
            short leftMouseButtonState = GetAsyncKeyState(0x01);

            short rightMouseButtonState = GetAsyncKeyState(0x02);

            if ((leftMouseButtonState & 0x8001) != 0 || (rightMouseButtonState & 0x8002) != 0)
            {
                
                if (MousedIsPressed == false)
                {
                    MouseClick?.Invoke();
                    MousedIsPressed = true;
                }

            }
            else
            {
                MousedIsPressed = false;
            }

        }

        private void onMouseMove()
        {
            POINT cursorPos;
            GetCursorPos(out cursorPos);
            if (LastMousePosition.X != cursorPos.X || LastMousePosition.Y != cursorPos.Y)
            {
                MouseMove?.Invoke(new Point(cursorPos.X, cursorPos.Y));
                LastMousePosition = cursorPos;
            }
        }

        private void onKeyPressed()
        {
            for (int i = 0; i < 256; i++)
            {
                short key = GetAsyncKeyState(i);

                if (key == 1 || key == -32767)
                {
                    KeyPressed?.Invoke();
                }

            }
        }
    }

}
