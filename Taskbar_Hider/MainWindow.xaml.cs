using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Taskbar_Hider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 主窗口句柄
        public IntPtr m_Hwnd;

        private readonly Taskbar tb;
        private readonly HotKeys hk;

        private readonly List<Keys> vkeys;
        private readonly List<Modifiers> modifiers;

        private readonly AutoRun autorun;
        private readonly NotifyIcon tray;

        private bool _close = false;

        public bool AboutToClose
        {
            set { _close = value; }
            get { return _close; }
        }

        public MainWindow()
        {
            InitializeComponent();
            tb = new Taskbar();
            hk = new HotKeys();

            // 设置ComboBox
            vkeys = new List<Keys>();
            Keys tmp = new Keys();
            foreach (HotKeys.EKey i in Enum.GetValues(typeof(HotKeys.EKey)))
            {
                tmp.index = (int)i;
                tmp.key = Enum.GetName(typeof(HotKeys.EKey), i);
                if (!vkeys.Contains(tmp))
                {
                    vkeys.Add(tmp);
                }
            }
            this.VKey_comboBox.ItemsSource = vkeys;
            this.VKey_comboBox.SelectedIndex = vkeys.FindIndex((Keys k) =>
                {
                    if (k.index == (int)ConfigHelper.VKey)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            modifiers = new List<Modifiers>();
            Modifiers tmp2 = new Modifiers();
            foreach (HotKeys.EKey i in Enum.GetValues(typeof(HotKeys.HotkeyModifiers)))
            {
                tmp2.index = (int)i;
                tmp2.modifier = Enum.GetName(typeof(HotKeys.HotkeyModifiers), i);
                if (!modifiers.Contains(tmp2))
                {
                    modifiers.Add(tmp2);
                }
            }
            this.Modifiers_comboBox.ItemsSource = modifiers;
            this.Modifiers_comboBox.SelectedIndex = modifiers.FindIndex((Modifiers k) =>
                {
                    if (k.index == (int)ConfigHelper.Modifiers)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            autorun = new AutoRun("Taskbar Hider");
            this.CheckBoxAutoRun.IsChecked = autorun.RunOnBoot;
            tray = new NotifyIcon(this);
        }

        ~MainWindow()
        {
            // 取消注册热键
            hk.UnRegist(m_Hwnd, tb.ChangeState);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // 获取窗体句柄
            m_Hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            System.Windows.Interop.HwndSource hWndSource = System.Windows.Interop.HwndSource.FromHwnd(m_Hwnd);
            // 添加处理程序
            if (hWndSource != null) hWndSource.AddHook(hk.OnHotkey);
            // 注册热键
            hk.Regist(this.m_Hwnd, (int)ConfigHelper.Modifiers, ConfigHelper.VKey, tb.ChangeState);
        }

        /// <summary>
        /// 所有控件初始化完成后调用
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
        }

        private void CheckBoxAutoRun_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                autorun.SetStartupOnBoot(true);
            }
            else if (cb.IsChecked == false)
            {
                autorun.SetStartupOnBoot(false);
            }
        }


        private void Modifiers_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex == -1)
            {
                cb.SelectedIndex = modifiers.FindIndex((Modifiers k) =>
                {
                    if (k.index == (int)ConfigHelper.Modifiers)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            ConfigHelper.Modifiers = (HotKeys.HotkeyModifiers)modifiers[cb.SelectedIndex].index;

            if (m_Hwnd != IntPtr.Zero)
            {
                // 取消注册热键
                hk.UnRegist(m_Hwnd, tb.ChangeState);
                // 注册热键
                hk.Regist(this.m_Hwnd, (int)ConfigHelper.Modifiers, ConfigHelper.VKey, tb.ChangeState);
            }
        }

        private void VKey_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex == -1)
            {
                cb.SelectedIndex = vkeys.FindIndex((Keys k) =>
                {
                    if (k.index == (int)ConfigHelper.VKey)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            ConfigHelper.VKey = (HotKeys.EKey)vkeys[cb.SelectedIndex].index;

            if (m_Hwnd != IntPtr.Zero)
            {
                // 取消注册热键
                hk.UnRegist(m_Hwnd, tb.ChangeState);
                // 注册热键
                hk.Regist(this.m_Hwnd, (int)ConfigHelper.Modifiers, ConfigHelper.VKey, tb.ChangeState);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.AboutToClose)
            {
                e.Cancel = true;
            }
            this.Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            tray.Dispose();
        }
    }
    public struct Keys
    {
        public int index
        {
            get;
            set;
        }
        public string key
        {
            get;
            set;
        }
        public override string ToString()
        {
            return this.key;
        }
        public override bool Equals(object obj)
        {
            return index == ((Keys)obj).index;
        }
        public override int GetHashCode()
        {
            return index.GetHashCode();
        }
    }

    public struct Modifiers
    {
        public int index
        {
            get;
            set;
        }
        public string modifier
        {
            get;
            set;
        }
        public override string ToString()
        {
            return this.modifier;
        }
        public override bool Equals(object obj)
        {
            return index == ((Modifiers)obj).index;
        }
        public override int GetHashCode()
        {
            return index.GetHashCode();
        }
    }
}
