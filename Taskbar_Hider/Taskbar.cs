using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Taskbar_Hider.Utils.Pinvoke;


namespace Taskbar_Hider
{
    internal class Taskbar
    {
        private DispatcherTimer timer;

        public bool Visibility
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
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (TBhWnd == IntPtr.Zero)
                {
                    TBhWnd = User32.FindWindow("Shell_TrayWnd", null);
                    if(sw.Elapsed == new TimeSpan(0,0,5))
                    {
                        throw new Exception("找不到任务栏句柄");
                    }
                }
            }

            CheckWhetherHiden();

            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0,milliseconds: 500),
            };
            timer.Tick += OnTimerHandler;
            timer.Start();


        }

        private void OnTimerHandler(object sender, EventArgs e)
        {
            if(ConfirmHiden())
            {
                if(User32.IsWindowVisible(TBhWnd) != Visibility)
                {
                    ConfirmHiden();
                }
            }
        }

        private bool ConfirmHiden()
        {
            if (User32.IsWindowVisible(TBhWnd) != this.Visibility)
            {
                SetTaskbarState(this.Visibility);
                Show(this.Visibility);
            }
            return User32.IsWindowVisible(TBhWnd) == this.Visibility;
        }

        public void ResetHandle()
        {
            TBhWnd = new IntPtr(0);
            TBhWnd = User32.FindWindow("System_TrayWnd", null);
            if (TBhWnd == IntPtr.Zero)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (TBhWnd == IntPtr.Zero)
                {
                    TBhWnd = User32.FindWindow("Shell_TrayWnd", null);
                    if (sw.Elapsed == new TimeSpan(0, 0, 5))
                    {
                        throw new Exception("找不到任务栏句柄");
                    }
                }
            }
        }

        public void CheckWhetherHiden()
        {
            this.Visibility = User32.IsWindowVisible(TBhWnd);
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
            Visibility = !Visibility;

            SetTaskbarState(Visibility);
            System.Threading.Thread.Sleep(80);
            Show(Visibility);

            ConfigHelper.ShowTaskbarOnStartup = Visibility;
        }

        

        /// <summary>
        /// Set the Taskbar State option
        /// </summary>
        /// <param name="option">AppBarState to activate</param>
        public void SetTaskbarState(bool alwaysOnTop)
        {
            Shell32.APPBARDATA msgData = new Shell32.APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = TBhWnd;
            Shell32.AppBarStates option;
            if(alwaysOnTop)
            {
                option = Shell32.AppBarStates.AlwaysOnTop;
            }
            else
            {
                option = Shell32.AppBarStates.AutoHide;
            }
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
