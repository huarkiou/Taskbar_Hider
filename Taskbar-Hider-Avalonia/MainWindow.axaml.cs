using System;
using System.Collections.Generic;
using Windows.Win32.Foundation;
using Avalonia.Controls;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Avalonia.Interactivity;
using Tbh;

namespace Taskbar_Hider_Avalonia;

public partial class MainWindow : Window
{
    private readonly List<Modifiers> _modifiers;
    private readonly List<Keys> _vKeys;
    private readonly AutoRun _autorun;
    private readonly HotKeys _hk;
    private readonly Taskbar _tb;

    private readonly HWND _hWnd; // MainWindow句柄

    public MainWindow()
    {
        InitializeComponent();

        _tb = new Taskbar
        {
            Visibility = ConfigHelper.Config.ShowTaskbarOnStartup
        };
        _hk = new HotKeys();
        _autorun = new AutoRun(App.ProgramName);

        // 注册热键
        Win32Properties.AddWndProcHookCallback(this, _hk.OnHotkey);
        _hWnd = new HWND(TryGetPlatformHandle()?.Handle ?? IntPtr.Zero);
        // _hk.Register(_hWnd, ConfigHelper.Config.Modifiers, ConfigHelper.Config.VKey, _tb.ChangeState);

        _vKeys = [];
        var tmp = new Keys();
        foreach (var virtualKey in Enum.GetValues<VIRTUAL_KEY>())
        {
            tmp.Index = (int)virtualKey;
            var name = Enum.GetName(virtualKey);
            if (name == null) continue;

            tmp.Name = name;
            if (!_vKeys.Contains(tmp)) _vKeys.Add(tmp);
        }

        VirtualKeyComboBox.ItemsSource = _vKeys;
        VirtualKeyComboBox.SelectedIndex = _vKeys.FindIndex(k => k.Index == (int)ConfigHelper.Config.VKey);

        _modifiers = [];
        var tmp2 = new Modifiers();
        foreach (var modifier in Enum.GetValues<HOT_KEY_MODIFIERS>())
        {
            tmp2.Index = (int)modifier;
            var name = Enum.GetName(modifier);
            if (name == null) continue;

            tmp2.Name = name;
            if (!_modifiers.Contains(tmp2)) _modifiers.Add(tmp2);
        }

        ModifierComboBox.ItemsSource = _modifiers;
        ModifierComboBox.SelectedIndex = _modifiers.FindIndex(k => k.Index == (int)ConfigHelper.Config.Modifiers);

        AutoRunCheckBox.IsChecked = _autorun.RunOnBoot;
    }

    ~MainWindow()
    {
        _hk.Unregister(_hWnd, _tb.ChangeState);
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _tb.ResetHandle();
    }

    private void AutoRunCheckBox_OnClick(object? sender, RoutedEventArgs e)
    {
        var cb = (CheckBox)sender!;
        _autorun.SetStartupOnBoot(cb.IsChecked!.Value);
    }

    private void ModifierComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox ?? throw new NullReferenceException();
        if (cb.SelectedIndex == -1)
            cb.SelectedIndex = _modifiers.FindIndex(k => k.Index == (int)ConfigHelper.Config.Modifiers);

        ConfigHelper.Config.Modifiers = (HOT_KEY_MODIFIERS)_modifiers[cb.SelectedIndex].Index;
        ConfigHelper.Save();

        if (_hWnd == HWND.Null) return;
        // 取消注册热键
        _hk.Unregister(_hWnd, _tb.ChangeState);
        // 注册热键
        _hk.Register(_hWnd, ConfigHelper.Config.Modifiers, ConfigHelper.Config.VKey, _tb.ChangeState);
    }

    private void VirtualKeyComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox ?? throw new NullReferenceException();
        if (cb.SelectedIndex == -1) cb.SelectedIndex = _vKeys.FindIndex(k => k.Index == (int)ConfigHelper.Config.VKey);

        ConfigHelper.Config.VKey = (VIRTUAL_KEY)_vKeys[cb.SelectedIndex].Index;
        ConfigHelper.Save();

        if (_hWnd != HWND.Null)
        {
            // 取消注册热键
            _hk.Unregister(_hWnd, _tb.ChangeState);
            // 注册热键
            _hk.Register(_hWnd, ConfigHelper.Config.Modifiers, ConfigHelper.Config.VKey, _tb.ChangeState);
        }
    }
}

public struct Keys : IEquatable<Keys>
{
    public int Index { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return Name[3..];
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        return Index == ((Keys)obj).Index;
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public bool Equals(Keys other)
    {
        return Index == other.Index && Name == other.Name;
    }

    public static bool operator ==(Keys left, Keys right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Keys left, Keys right)
    {
        return !(left == right);
    }
}

public struct Modifiers : IEquatable<Modifiers>
{
    public int Index { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return Name[4..];
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        return Index == ((Modifiers)obj).Index;
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public bool Equals(Modifiers other)
    {
        return Index == other.Index && Name == other.Name;
    }

    public static bool operator ==(Modifiers left, Modifiers right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Modifiers left, Modifiers right)
    {
        return !(left == right);
    }
}