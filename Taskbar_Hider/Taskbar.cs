using System;
using System.Runtime.InteropServices;

namespace Taskbar_Hider
{
    internal class Taskbar
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "IsWindowVisible")]
        private extern static bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        private extern static int ShowWindow(IntPtr hWnd, SW show);

        [DllImport("shell32.dll", EntryPoint = "SHAppBarMessage")]
        private extern static uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

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
            TBhWnd = FindWindow("System_TrayWnd", null);
            if (TBhWnd == IntPtr.Zero)
            {
                TBhWnd = FindWindow("Shell_TrayWnd", null);
            }

            CheckWhetherHiden();
        }

        public void CheckWhetherHiden()
        {
            this.IsHiden = !IsWindowVisible(TBhWnd);
        }

        public enum SW
        {
            SW_HIDE = 0,
            SW_MAXMIZE = 3,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10
        }

        public void Show(bool show)
        {
            if (show)
            {
                ShowWindow(TBhWnd, SW.SW_SHOW);
            }
            else
            {
                ShowWindow(TBhWnd, SW.SW_HIDE);
            }
        }

        public void ChangeState()
        {
            CheckWhetherHiden();
            if (IsHiden)
            {
                SetTaskbarState(AppBarStates.AlwaysOnTop);
                Show(true);
            }
            else
            {
                SetTaskbarState(AppBarStates.AutoHide);
                System.Threading.Thread.Sleep(80);
                Show(false);
            }
        }

        //定义一些结构
        public enum AppBarMessages
        {
            New = 0x00,
            Remove = 0x01,
            QueryPos = 0x02,
            SetPos = 0x03,
            GetState = 0x04,
            GetTaskBarPos = 0x05,
            Activate = 0x06,
            GetAutoHideBar = 0x07,
            SetAutoHideBar = 0x08,
            WindowPosChanged = 0x09,
            SetState = 0x0a
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum AppBarStates
        {
            AutoHide = 0x01,
            AlwaysOnTop = 0x02
        }

        /// <summary>
        /// Set the Taskbar State option
        /// </summary>
        /// <param name="option">AppBarState to activate</param>
        public void SetTaskbarState(AppBarStates option)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = TBhWnd;
            msgData.lParam = (int)(option);
            SHAppBarMessage((uint)AppBarMessages.SetState, ref msgData);
        }

        /// <summary>
        /// Gets the current Taskbar state
        /// </summary>
        /// <returns>current Taskbar state</returns>
        public AppBarStates GetTaskbarState()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = TBhWnd;
            return (AppBarStates)SHAppBarMessage((uint)AppBarMessages.GetState, ref msgData);
        }

    } // class Taskbar

} // namspace Taskbar_Hider
