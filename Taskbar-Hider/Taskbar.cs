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
    public bool Visibility { get; set; }

    private readonly bool _defaultAutoHide;
    private readonly DispatcherTimer _timer;
    private HWND HWnd { get; set; }
    private const string ClassName = "Shell_TrayWnd";

    public Taskbar()
    {
        ResetHandle();

        // 检查并保存当前任务栏状态
        _defaultAutoHide = (GetTaskbarState() & PInvoke.ABS_AUTOHIDE) == PInvoke.ABS_AUTOHIDE;
        Visibility = PInvoke.IsWindowVisible(HWnd);

        // 设置定时器用于定时保证任务栏的隐藏情况
        _timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 1000)
        };
        _timer.Tick += (_, _) =>
        {
            if (PInvoke.IsWindowVisible(HWnd) == Visibility) Show(Visibility);
        };
        _timer.Start();
    }

    public void ResetHandle()
    {
        HWnd = PInvoke.FindWindow(ClassName, null);
        if (HWnd != HWND.Null) return;
        Stopwatch sw = new();
        sw.Start();
        while (HWnd == HWND.Null)
        {
            HWnd = PInvoke.FindWindow(ClassName, null);
            if (sw.Elapsed > new TimeSpan(0, 0, 5)) throw new Exception("找不到任务栏句柄");
        }
    }

    public void ChangeState()
    {
        Visibility = !Visibility;

        if (!_defaultAutoHide) SetTaskbarState(Visibility);

        Thread.Sleep(80);
        Show(Visibility);

        AppConfiguration.Instance.Config.ShowTaskbarOnStartup = Visibility;
    }

    private void Show(bool show)
    {
        if (!PInvoke.IsWindow(HWnd))
        {
            ResetHandle();
        }

        PInvoke.ShowWindow(HWnd,
            show
                ? SHOW_WINDOW_CMD.SW_SHOW
                : SHOW_WINDOW_CMD.SW_HIDE);
    }

    private void SetTaskbarState(bool alwaysOnTop)
    {
        var msgData = new APPBARDATA();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = HWnd;
        var option = alwaysOnTop ? PInvoke.ABS_ALWAYSONTOP : PInvoke.ABS_AUTOHIDE;

        msgData.lParam = (nint)option;
        PInvoke.SHAppBarMessage(PInvoke.ABM_SETSTATE, ref msgData);
    }

    private uint GetTaskbarState()
    {
        var msgData = new APPBARDATA();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = HWnd;
        return PInvoke.SHAppBarMessage(PInvoke.ABM_GETSTATE, ref msgData).ToUInt32();
    }
}