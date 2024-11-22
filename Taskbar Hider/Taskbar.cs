using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using static Tbh.Utils.Pinvoke.Shell32;
using static Tbh.Utils.Pinvoke.User32;

namespace Tbh
{
    internal class Taskbar
    {
        private readonly DispatcherTimer _timer;

        private readonly bool _defaultAutoHide;

        public bool Visibility { get; set; }

        private IntPtr HWnd { get; set; }

        public Taskbar()
        {
            // 设置任务栏句柄
            HWnd = new IntPtr(0);
            HWnd = FindWindow("System_TrayWnd", null);
            if (HWnd == IntPtr.Zero)
            {
                var sw = new Stopwatch();
                sw.Start();
                while (HWnd == IntPtr.Zero)
                {
                    HWnd = FindWindow("Shell_TrayWnd", null);
                    if (sw.Elapsed == new TimeSpan(0, 0, 5))
                    {
                        throw new Exception("找不到任务栏句柄");
                    }
                }
            }

            // 检查并保存当前任务栏状态
            var taskbarState = GetTaskbarState();
            this._defaultAutoHide = taskbarState is AppBarStates.AutoHide
                or (AppBarStates.AutoHide | AppBarStates.AlwaysOnTop);
            this.Visibility = IsWindowVisible(HWnd);

            // 设置定时器用于定时检查任务栏的隐藏情况
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0, milliseconds: 500),
            };
            _timer.Tick += OnTimerHandler;
            _timer.Start();
        }

        private void OnTimerHandler(object? sender, EventArgs e)
        {
            if (IsVisible())
            {
                if (IsWindowVisible(HWnd) != Visibility)
                {
                    IsVisible();
                }
            }
        }

        private bool IsVisible()
        {
            if (IsWindowVisible(HWnd) != this.Visibility)
            {
                SetTaskbarState(this.Visibility);
                Show(this.Visibility);
            }

            return IsWindowVisible(HWnd) == this.Visibility;
        }

        public void ResetHandle()
        {
            HWnd = new IntPtr(0);
            HWnd = FindWindow("System_TrayWnd", null);
            if (HWnd == IntPtr.Zero)
            {
                Stopwatch sw = new();
                sw.Start();
                while (HWnd == IntPtr.Zero)
                {
                    HWnd = FindWindow("Shell_TrayWnd", null);
                    if (sw.Elapsed == new TimeSpan(0, 0, 5))
                    {
                        throw new Exception("找不到任务栏句柄");
                    }
                }
            }
        }

        private void Show(bool show)
        {
            ShowWindow(HWnd, show ? SW.SW_SHOW : SW.SW_HIDE);
        }

        public void ChangeState()
        {
            Visibility = !Visibility;

            if (!this._defaultAutoHide)
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
        /// <param name="alwaysOnTop"></param>
        private void SetTaskbarState(bool alwaysOnTop)
        {
            var msgData = new APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = HWnd;
            var option = alwaysOnTop ? AppBarStates.AlwaysOnTop : AppBarStates.AutoHide;

            msgData.lParam = (int)(option);
            SHAppBarMessage((uint)AppBarMessages.SetState, ref msgData);
        }

        /// <summary>
        /// Gets the current Taskbar state
        /// </summary>
        /// <returns>current Taskbar state</returns>
        private AppBarStates GetTaskbarState()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = HWnd;
            return (AppBarStates)SHAppBarMessage((uint)AppBarMessages.GetState, ref msgData);
        }
    } // class Taskbar
} // namespace Taskbar_Hider