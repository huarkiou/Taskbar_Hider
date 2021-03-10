using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Taskbar_Hider.Utils.Pinvoke;


namespace Taskbar_Hider
{
    internal class Taskbar
    {
        private DispatcherTimer timer;

        public bool IsHiden
        {
            get;
            set;
        }

        public IntPtr TBhWnd
        {
            get;
            set;
        }

        public Taskbar()
        {
            TBhWnd = new IntPtr(0);
            TBhWnd = User32.FindWindow("System_TrayWnd", null);
            if (TBhWnd == IntPtr.Zero)
            {
                TBhWnd = User32.FindWindow("Shell_TrayWnd", null);
            }

            CheckWhetherHiden();

            timer = null;

        }

        private void OnTimerHandler(object sender, EventArgs e)
        {
            if(ConfirmHiden())
            {
                ((DispatcherTimer)sender).Stop();
            }
        }

        private bool ConfirmHiden()
        {
            if(User32.IsWindowVisible(TBhWnd) == this.IsHiden)
            {
                if (IsHiden)
                {
                    SetTaskbarState(Shell32.AppBarStates.AutoHide);
                    System.Threading.Thread.Sleep(80);
                    Show(false);
                }
                else
                {
                    SetTaskbarState(Shell32.AppBarStates.AlwaysOnTop);
                    Show(true);
                }
            }
            return User32.IsWindowVisible(TBhWnd) != this.IsHiden;
        }

        public void CheckWhetherHiden()
        {
            this.IsHiden = !User32.IsWindowVisible(TBhWnd);
        }


        public void Show(bool show)
        {
            if (show)
            {
                User32.ShowWindow(TBhWnd, User32.SW.SW_SHOW);
            }
            else
            {
                User32.ShowWindow(TBhWnd, User32.SW.SW_HIDE);
            }
        }

        public void ChangeState()
        {
            IsHiden = !IsHiden;
            ConfirmHiden();

            if (timer != null)
            {
                timer.Stop();
            }
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0,milliseconds: 200),
            };
            timer.Tick += OnTimerHandler;
            timer.Start();
        }

        

        /// <summary>
        /// Set the Taskbar State option
        /// </summary>
        /// <param name="option">AppBarState to activate</param>
        public void SetTaskbarState(Shell32.AppBarStates option)
        {
            Shell32.APPBARDATA msgData = new Shell32.APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = TBhWnd;
            msgData.lParam = (int)(option);
            Shell32.SHAppBarMessage((uint)Shell32.AppBarMessages.SetState, ref msgData);
        }

        /// <summary>
        /// Gets the current Taskbar state
        /// </summary>
        /// <returns>current Taskbar state</returns>
        public Shell32.AppBarStates GetTaskbarState()
        {
            Shell32.APPBARDATA msgData = new Shell32.APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = TBhWnd;
            return (Shell32.AppBarStates)Shell32.SHAppBarMessage((uint)Shell32.AppBarMessages.GetState, ref msgData);
        }

    } // class Taskbar

} // namspace Taskbar_Hider
