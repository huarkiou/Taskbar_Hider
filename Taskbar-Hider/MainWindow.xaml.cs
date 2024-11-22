using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Tbh;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly AutoRun _autorun;
    private readonly HotKeys _hk;
    private readonly List<Modifiers> _modifiers;

    private readonly Taskbar _tb;
    private readonly NotifyIcon _tray;

    private readonly List<Keys> _vKeys;

    // 主窗口句柄
    private HWND _hWnd;

    public MainWindow()
    {
        InitializeComponent();
        _tb = new Taskbar
        {
            Visibility = ConfigHelper.Config.ShowTaskbarOnStartup
        };

        _hk = new HotKeys();

        // 设置ComboBox
        _vKeys = [];
        var tmp = new Keys();
        foreach (VIRTUAL_KEY i in Enum.GetValues(typeof(VIRTUAL_KEY)))
        {
            tmp.Index = (int)i;
            var key = Enum.GetName(typeof(VIRTUAL_KEY), i);
            if (key == null) continue;

            tmp.Name = key;
            if (!_vKeys.Contains(tmp)) _vKeys.Add(tmp);
        }

        VKeyComboBox.ItemsSource = _vKeys;
        VKeyComboBox.SelectedIndex = _vKeys.FindIndex(k => k.Index == (int)ConfigHelper.Config.VKey);

        _modifiers = [];
        var tmp2 = new Modifiers();
        foreach (HOT_KEY_MODIFIERS i in Enum.GetValues(typeof(HOT_KEY_MODIFIERS)))
        {
            tmp2.Index = (int)i;
            var modifier = Enum.GetName(typeof(HOT_KEY_MODIFIERS), i);
            if (modifier == null) continue;

            tmp2.Name = modifier;
            if (!_modifiers.Contains(tmp2)) _modifiers.Add(tmp2);
        }

        ModifiersComboBox.ItemsSource = _modifiers;
        ModifiersComboBox.SelectedIndex = _modifiers.FindIndex(k => k.Index == (int)ConfigHelper.Config.Modifiers);

        _autorun = new AutoRun("Taskbar Hider");
        CheckBoxAutoRun.IsChecked = _autorun.RunOnBoot;
        _tray = new NotifyIcon(this);
    }

    public bool AboutToClose { set; get; }

    ~MainWindow()
    {
        // 取消注册热键
        _hk.Unregister(_hWnd, _tb.ChangeState);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        // 获取窗体句柄
        _hWnd = new HWND(new WindowInteropHelper(this).Handle);
        var hWndSource = HwndSource.FromHwnd(_hWnd);
        // 添加处理程序
        hWndSource?.AddHook(_hk.OnHotkey);
        // 注册热键
        _hk.Register(_hWnd, ConfigHelper.Config.Modifiers, ConfigHelper.Config.VKey, _tb.ChangeState);
    }

    private void CheckBoxAutoRun_Click(object sender, RoutedEventArgs e)
    {
        var cb = (CheckBox)sender;
        if (cb.IsChecked == true)
            _autorun.SetStartupOnBoot(true);
        else if (cb.IsChecked == false) _autorun.SetStartupOnBoot(false);
    }


    private void Modifiers_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    private void VKey_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (!AboutToClose) e.Cancel = true;

        Hide();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _tray.Dispose();
    }

    private void ButtonReset_Click(object sender, RoutedEventArgs e)
    {
        _tb.ResetHandle();
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