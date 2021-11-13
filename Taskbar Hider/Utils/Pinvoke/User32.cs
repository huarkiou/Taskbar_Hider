using System;
using System.Runtime.InteropServices;

namespace Taskbar_Hider.Utils.Pinvoke
{
    public static class User32
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("User32.dll", EntryPoint = "IsWindowVisible")]
        public extern static bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public extern static int ShowWindow(IntPtr hWnd, SW show);

        public enum SW
        {
            SW_HIDE = 0,
            SW_MAXMIZE = 3,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10
        }

    }
}
