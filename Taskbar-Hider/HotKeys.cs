using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Taskbar_Hider_Avalonia;

public class HotKeys
{
    public delegate void HotKeyCallBackHandler();

    private readonly Dictionary<int, HotKeyCallBackHandler> _keymap = new();

    /// <summary>
    ///     注册快捷键
    /// </summary>
    /// <param name="hWnd">持有快捷键窗口的句柄</param>
    /// <param name="modifiers">组合键</param>
    /// <param name="vk">快捷键的虚拟键码</param>
    /// <param name="callBack">回调函数（按下快捷键时被调用的方法）</param>
    internal void Register(HWND hWnd, HOT_KEY_MODIFIERS modifiers, VIRTUAL_KEY vk,
        HotKeyCallBackHandler callBack)
    {
        int id = PInvoke.GlobalAddAtom("Taskbar-Hider");
        if (!PInvoke.RegisterHotKey(hWnd, id, modifiers, (uint)vk))
            throw new Exception("注册失败！");
        _keymap[id] = callBack;
    }

    /// <summary>
    ///     注销快捷键
    /// </summary>
    /// <param name="hWnd">持有快捷键窗口的句柄</param>
    /// <param name="callBack">回调函数</param>
    internal void Unregister(HWND hWnd, HotKeyCallBackHandler callBack)
    {
        foreach (var (key, _) in _keymap.Where(pair => pair.Value == callBack))
        {
            PInvoke.UnregisterHotKey(hWnd, key);
            PInvoke.GlobalDeleteAtom((ushort)key);
        }
    }

    /// <summary>
    ///     窗体回调函数，接收所有窗体消息的事件处理函数
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <param name="msg">消息</param>
    /// <param name="wideParam">附加参数1</param>
    /// <param name="longParam">附加参数2</param>
    /// <param name="handled">是否处理</param>
    /// <returns>返回句柄</returns>
    public IntPtr OnHotkey(IntPtr hWnd, uint msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
    {
        switch (msg)
        {
            case (int)PInvoke.WM_HOTKEY:
                var id = wideParam.ToInt32();
                if (_keymap.TryGetValue(id, out var callback))
                    callback();
                break;
        }

        return IntPtr.Zero;
    }
}