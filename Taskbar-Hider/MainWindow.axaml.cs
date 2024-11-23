using System;
using System.Collections.Immutable;
using System.Linq;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Taskbar_Hider_Avalonia;

public partial class MainWindow : Window
{
    private readonly AutoRun _autorun;
    private readonly HotKeys _hk;
    private readonly Taskbar _tb;

    private readonly ImmutableList<Modifiers> _modifiers;
    private readonly ImmutableList<Keys> _vKeys;

    private readonly HWND _hWnd; // MainWindow句柄

    private bool _showFirstTime = true;

    public MainWindow()
    {
        Opened += (_, _) =>
        {
            if (_showFirstTime)
            {
                Hide();
                _showFirstTime = false;
            }
        };
        InitializeComponent();
        _hWnd = new HWND(TryGetPlatformHandle()?.Handle ?? IntPtr.Zero);
        _tb = new Taskbar
        {
            Visibility = AppConfiguration.Instance.Config.ShowTaskbarOnStartup
        };
        _hk = new HotKeys();
        Win32Properties.AddWndProcHookCallback(this, _hk.OnHotkey);
        _autorun = new AutoRun(App.ProgramName);

        AutoRunCheckBox.IsChecked = _autorun.RunOnBoot;

        _vKeys = Enum.GetValues<VIRTUAL_KEY>()
            .Select(m => new Keys() { Index = (int)m, Name = Enum.GetName(m)! })
            .ToImmutableList();
        VirtualKeyComboBox.ItemsSource = _vKeys;
        VirtualKeyComboBox.SelectedIndex = _vKeys.FindIndex(k => k.Index == (int)AppConfiguration.Instance.Config.VKey);

        _modifiers = Enum.GetValues<HOT_KEY_MODIFIERS>()
            .Select(m => new Modifiers() { Index = (int)m, Name = Enum.GetName(m)! })
            .ToImmutableList();
        ModifierComboBox.ItemsSource = _modifiers;
        ModifierComboBox.SelectedIndex =
            _modifiers.FindIndex(k => k.Index == (int)AppConfiguration.Instance.Config.Modifiers);
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
            cb.SelectedIndex = _modifiers.FindIndex(k => k.Index == (int)AppConfiguration.Instance.Config.Modifiers);

        AppConfiguration.Instance.Config.Modifiers = (uint)_modifiers[cb.SelectedIndex].Index;
        AppConfiguration.Instance.Save();

        if (_hWnd == HWND.Null) return;
        // 取消注册热键
        _hk.Unregister(_hWnd, _tb.ChangeState);
        // 注册热键
        _hk.Register(_hWnd, (HOT_KEY_MODIFIERS)AppConfiguration.Instance.Config.Modifiers,
            (VIRTUAL_KEY)AppConfiguration.Instance.Config.VKey,
            _tb.ChangeState);
    }

    private void VirtualKeyComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox ?? throw new NullReferenceException();
        if (cb.SelectedIndex == -1)
            cb.SelectedIndex = _vKeys.FindIndex(k => k.Index == (int)AppConfiguration.Instance.Config.VKey);

        AppConfiguration.Instance.Config.VKey = (uint)_vKeys[cb.SelectedIndex].Index;
        AppConfiguration.Instance.Save();

        if (_hWnd != HWND.Null)
        {
            // 取消注册热键
            _hk.Unregister(_hWnd, _tb.ChangeState);
            // 注册热键
            _hk.Register(_hWnd, (HOT_KEY_MODIFIERS)AppConfiguration.Instance.Config.Modifiers,
                (VIRTUAL_KEY)AppConfiguration.Instance.Config.VKey,
                _tb.ChangeState);
        }
    }
}

public readonly struct Keys : IEquatable<Keys>
{
    public int Index { get; init; }
    public string Name { get; init; }

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

public readonly struct Modifiers : IEquatable<Modifiers>
{
    public int Index { get; init; }
    public string Name { get; init; }

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