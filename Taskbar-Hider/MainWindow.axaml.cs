using System;
using System.Collections.Immutable;
using System.Linq;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Taskbar_Hider;

public partial class MainWindow : Window
{
    private readonly AutoRun _autorun;
    private readonly HotKeys _hk;
    private readonly Taskbar _tb;

    private readonly HWND _hWnd; // MainWindow句柄 不为HWND.Null
    private bool _showFirstTime = true;

    private readonly ImmutableSortedDictionary<uint, string> _vKeys;
    private readonly ImmutableSortedDictionary<uint, string> _modifiers;

    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            if (!_showFirstTime) return;
            Hide(); // 这会导致设计器中窗口隐藏
            _showFirstTime = false;
        };
        _hWnd = new HWND((nint)TryGetPlatformHandle()?.Handle!);
        _tb = new Taskbar();
        _hk = new HotKeys();
        Win32Properties.AddWndProcHookCallback(this, _hk.OnHotkey);
        _autorun = new AutoRun(App.ProgramName);
        AutoRunToggleSwitch.IsChecked = _autorun.RunOnBoot;

        _vKeys = Enum.GetValues<VIRTUAL_KEY>()
            .ToImmutableSortedDictionary(m => (uint)m, m => Enum.GetName(m)![3..]);

        VirtualKeyComboBox.ItemsSource = _vKeys.Values;
        VirtualKeyComboBox.SelectedValue = _vKeys[AppConfiguration.Instance.Config.VKey];

        _modifiers = Enum.GetValues<HOT_KEY_MODIFIERS>()
            .ToImmutableSortedDictionary(m => (uint)m, m => Enum.GetName(m)![4..]);
        ModifierComboBox.ItemsSource = _modifiers.Values;
        ModifierComboBox.SelectedValue = _modifiers[AppConfiguration.Instance.Config.Modifiers];
    }

    ~MainWindow()
    {
        _hk.Unregister(_hWnd, _tb.ChangeState);
    }

    private void AutoRunToggleSwitch_OnClick(object? sender, RoutedEventArgs e)
    {
        var cb = (ToggleSwitch)sender!;
        _autorun.SetStartupOnBoot(cb.IsChecked!.Value);
    }

    private void ModifierComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox ?? throw new NullReferenceException();

        AppConfiguration.Instance.Config.Modifiers = _modifiers.Where(v => v.Value == cb.SelectedItem!.ToString())
            .Select(v => v.Key).ToArray().First();
        AppConfiguration.Instance.Save();

        ChangeHotkey();
    }

    private void VirtualKeyComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox ?? throw new NullReferenceException();

        AppConfiguration.Instance.Config.VKey = _vKeys.Where(v => v.Value == cb.SelectedItem!.ToString())
            .Select(v => v.Key).ToArray().First();
        AppConfiguration.Instance.Save();

        ChangeHotkey();
    }

    private void ChangeHotkey()
    {
        // 取消注册热键
        _hk.Unregister(_hWnd, _tb.ChangeState);
        // 注册热键
        try
        {
            _hk.Register(_hWnd, (HOT_KEY_MODIFIERS)AppConfiguration.Instance.Config.Modifiers,
                (VIRTUAL_KEY)AppConfiguration.Instance.Config.VKey,
                _tb.ChangeState);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}