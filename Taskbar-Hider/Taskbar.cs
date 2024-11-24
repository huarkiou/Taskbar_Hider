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
    private const string ClassName = "Shell_TrayWnd";
    private readonly bool _defaultAutoHide;
    private readonly DispatcherTimer _timer;

    private HWND _hWnd = HWND.Null;


    public Taskbar()
    {
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

    public bool Visibility { get; set; }

    private HWND HWnd
    {
        get
        {
            if (_hWnd != HWND.Null && PInvoke.IsWindow(_hWnd)) return _hWnd;
            _hWnd = PInvoke.FindWindow(ClassName, null);
            if (_hWnd != HWND.Null) return _hWnd;
            Stopwatch sw = new();
            sw.Start();
            while (_hWnd == HWND.Null)
            {
                _hWnd = PInvoke.FindWindow(ClassName, null);
                if (sw.Elapsed > new TimeSpan(0, 0, 5)) throw new Exception("找不到任务栏句柄");
            }

            return _hWnd;
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