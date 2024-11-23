using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using Avalonia.Threading;

namespace Taskbar_Hider_Avalonia;

internal class Taskbar
{
    private readonly bool _defaultAutoHide;
    private readonly DispatcherTimer _timer;

    public Taskbar()
    {
        // 设置任务栏句柄
        HWnd = HWND.Null;
        HWnd = PInvoke.FindWindow("System_TrayWnd", null);
        if (HWnd == HWND.Null)
        {
            var sw = new Stopwatch();
            sw.Start();
            while (HWnd == HWND.Null)
            {
                HWnd = PInvoke.FindWindow("Shell_TrayWnd", null);
                if (sw.Elapsed == new TimeSpan(0, 0, 5)) throw new Exception("找不到任务栏句柄");
            }
        }

        // 检查并保存当前任务栏状态
        var taskbarState = GetTaskbarState();
        _defaultAutoHide = taskbarState is PInvoke.ABS_AUTOHIDE
            or (PInvoke.ABS_AUTOHIDE | PInvoke.ABS_ALWAYSONTOP);
        Visibility = PInvoke.IsWindowVisible(HWnd);

        // 设置定时器用于定时检查任务栏的隐藏情况
        _timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        _timer.Tick += OnTimerHandler;
        _timer.Start();
    }

    public bool Visibility { get; set; }

    private HWND HWnd { get; set; }

    private void OnTimerHandler(object? sender, EventArgs e)
    {
        if (PInvoke.IsWindowVisible(HWnd) == Visibility) Show(Visibility);
    }


    public void ResetHandle()
    {
        HWnd = PInvoke.FindWindow("System_TrayWnd", null);
        if (HWnd != HWND.Null) return;
        Stopwatch sw = new();
        sw.Start();
        while (HWnd == HWND.Null)
        {
            HWnd = PInvoke.FindWindow("Shell_TrayWnd", null);
            if (sw.Elapsed == new TimeSpan(0, 0, 5)) throw new Exception("找不到任务栏句柄");
        }
    }

    private void Show(bool show)
    {
        PInvoke.ShowWindow(HWnd,
            show
                ? SHOW_WINDOW_CMD.SW_SHOW
                : SHOW_WINDOW_CMD.SW_HIDE);
    }

    public void ChangeState()
    {
        Visibility = !Visibility;

        if (!_defaultAutoHide) SetTaskbarState(Visibility);

        Thread.Sleep(80);
        Show(Visibility);

        AppConfiguration.Instance.Config.ShowTaskbarOnStartup = Visibility;
    }

    /// <summary>
    ///     Set the Taskbar State option
    /// </summary>
    /// <param name="alwaysOnTop"></param>
    private void SetTaskbarState(bool alwaysOnTop)
    {
        var msgData = new APPBARDATA();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = HWnd;
        var option = alwaysOnTop ? PInvoke.ABS_ALWAYSONTOP : PInvoke.ABS_AUTOHIDE;

        msgData.lParam = (nint)option;
        PInvoke.SHAppBarMessage(PInvoke.ABM_SETSTATE, ref msgData);
    }

    /// <summary>
    ///     Gets the current Taskbar state
    /// </summary>
    /// <returns>current Taskbar state</returns>
    private uint GetTaskbarState()
    {
        var msgData = new APPBARDATA();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = HWnd;
        return PInvoke.SHAppBarMessage(PInvoke.ABM_GETSTATE, ref msgData).ToUInt32();
    }
} // class Taskbar
// namespace Taskbar_Hider