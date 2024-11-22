using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Tbh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // 主窗口句柄
        private IntPtr _hWnd;

        private readonly Taskbar _tb;
        private readonly HotKeys _hk;

        private readonly List<Keys> _vkeys;
        private readonly List<Modifiers> _modifiers;

        private readonly AutoRun _autorun;
        private readonly NotifyIcon _tray;

        public bool AboutToClose { set; get; } = false;

        public MainWindow()
        {
            InitializeComponent();
            _tb = new Taskbar
            {
                Visibility = ConfigHelper.ShowTaskbarOnStartup
            };

            _hk = new HotKeys();

            // 设置ComboBox
            _vkeys = [];
            var tmp = new Keys();
            foreach (HotKeys.EKey i in Enum.GetValues(typeof(HotKeys.EKey)))
            {
                tmp.Index = (int)i;
                var key = Enum.GetName(typeof(HotKeys.EKey), i);
                if (key == null)
                {
                    continue;
                }

                tmp.Key = key;
                if (!_vkeys.Contains(tmp))
                {
                    _vkeys.Add(tmp);
                }
            }

            VKeyComboBox.ItemsSource = _vkeys;
            VKeyComboBox.SelectedIndex = _vkeys.FindIndex(k => k.Index == (int)ConfigHelper.VKey);

            _modifiers = [];
            var tmp2 = new Modifiers();
            foreach (HotKeys.EKey i in Enum.GetValues(typeof(HotKeys.HotkeyModifiers)))
            {
                tmp2.Index = (int)i;
                var modifier = Enum.GetName(typeof(HotKeys.HotkeyModifiers), i);
                if (modifier == null)
                {
                    continue;
                }

                tmp2.Modifier = modifier;
                if (!_modifiers.Contains(tmp2))
                {
                    _modifiers.Add(tmp2);
                }
            }

            ModifiersComboBox.ItemsSource = _modifiers;
            ModifiersComboBox.SelectedIndex = _modifiers.FindIndex(k =>
            {
                if (k.Index == (int)ConfigHelper.Modifiers)
                {
                    return true;
                }

                return false;
            });

            _autorun = new AutoRun("Taskbar Hider");
            CheckBoxAutoRun.IsChecked = _autorun.RunOnBoot;
            _tray = new NotifyIcon(this);
        }

        ~MainWindow()
        {
            // 取消注册热键
            _hk.UnRegist(_hWnd, _tb.ChangeState);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // 获取窗体句柄
            _hWnd = new WindowInteropHelper(this).Handle;
            var hWndSource = HwndSource.FromHwnd(_hWnd);
            // 添加处理程序
            hWndSource?.AddHook(_hk.OnHotkey);
            // 注册热键
            _hk.Regist(_hWnd, (int)ConfigHelper.Modifiers, ConfigHelper.VKey, _tb.ChangeState);
        }

        private void CheckBoxAutoRun_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                _autorun.SetStartupOnBoot(true);
            }
            else if (cb.IsChecked == false)
            {
                _autorun.SetStartupOnBoot(false);
            }
        }


        private void Modifiers_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex == -1)
            {
                cb.SelectedIndex = _modifiers.FindIndex(k =>
                {
                    if (k.Index == (int)ConfigHelper.Modifiers)
                    {
                        return true;
                    }

                    return false;
                });
            }

            ConfigHelper.Modifiers = (HotKeys.HotkeyModifiers)_modifiers[cb.SelectedIndex].Index;
            ConfigHelper.Save();

            if (_hWnd != IntPtr.Zero)
            {
                // 取消注册热键
                _hk.UnRegist(_hWnd, _tb.ChangeState);
                // 注册热键
                _hk.Regist(_hWnd, (int)ConfigHelper.Modifiers, ConfigHelper.VKey, _tb.ChangeState);
            }
        }

        private void VKey_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex == -1)
            {
                cb.SelectedIndex = _vkeys.FindIndex(k =>
                {
                    if (k.Index == (int)ConfigHelper.VKey)
                    {
                        return true;
                    }

                    return false;
                });
            }

            ConfigHelper.VKey = (HotKeys.EKey)_vkeys[cb.SelectedIndex].Index;
            ConfigHelper.Save();

            if (_hWnd != IntPtr.Zero)
            {
                // 取消注册热键
                _hk.UnRegist(_hWnd, _tb.ChangeState);
                // 注册热键
                _hk.Regist(_hWnd, (int)ConfigHelper.Modifiers, ConfigHelper.VKey, _tb.ChangeState);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!AboutToClose)
            {
                e.Cancel = true;
            }

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
        public string Key { get; set; }

        public override string ToString()
        {
            return Key;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Index == ((Keys)obj).Index;
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public bool Equals(Keys other)
        {
            return Index == other.Index && Key == other.Key;
        }
    }

    public struct Modifiers : IEquatable<Modifiers>
    {
        public int Index { get; set; }
        public string Modifier { get; set; }

        public override string ToString()
        {
            return this.Modifier;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Index == ((Modifiers)obj).Index;
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public bool Equals(Modifiers other)
        {
            return Index == other.Index && Modifier == other.Modifier;
        }
    }
}