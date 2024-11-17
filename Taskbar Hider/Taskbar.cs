using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Tbh.Utils.Pinvoke;


namespace Tbh
{
    internal class Taskbar
    {
        private DispatcherTimer timer;

        private bool DefaultAutoHide = false;

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
            // 设置任务栏句柄
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

            // 检查并保存当前任务栏状态
            Shell32.AppBarStates taskbar_state = GetTaskbarState();
            this.DefaultAutoHide = (taskbar_state == Shell32.AppBarStates.AutoHide)
                || ((taskbar_state == (Shell32.AppBarStates.AutoHide | Shell32.AppBarStates.AlwaysOnTop)));
            this.Visibility = User32.IsWindowVisible(TBhWnd);

            // 设置定时器用于定时检查任务栏的隐藏情况
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0, milliseconds: 500),
            };
            timer.Tick += OnTimerHandler;
            timer.Start();


        }

        private void OnTimerHandler(object? sender, EventArgs e)
        {
            if (ConfirmHiden())
            {
                if (User32.IsWindowVisible(TBhWnd) != Visibility)
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
                Stopwatch sw = new();
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

            if (!this.DefaultAutoHide)
            {
                SetTaskbarState(Visibility);
            }
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
            if (alwaysOnTop)
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
