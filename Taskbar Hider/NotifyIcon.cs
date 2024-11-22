using System;
using System.Drawing;
using System.Reflection;
using System.Windows;

namespace Tbh
{
    internal class NotifyIcon
    {
        private readonly System.Windows.Forms.NotifyIcon _notifyicon;
        private readonly MainWindow _mainwindow;

        public NotifyIcon(MainWindow win)
        {
            _mainwindow = win;
            // 初始化托盘菜单
            _notifyicon = new System.Windows.Forms.NotifyIcon();

            // 导入资源
            //Uri uri = new(@"Resources\Icon_MainWindow.ico", UriKind.Relative);
            //System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(uri);
            //notifyicon.Icon = new Icon(info.Stream);

            // 导入嵌入的资源
            //MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Icon_MainWindow.ico");
            var name = MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace + ".Resources.Icon_MainWindow.ico";
            Assembly assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream? stream = assembly.GetManifestResourceStream(name);
            //var list = assembly.GetManifestResourceNames();
            //foreach (string item in list) {
            //    MessageBox.Show(item);
            //}
            if (stream == null)
            {
                MessageBox.Show("Cannot file notifyicon", "Error", MessageBoxButton.OKCancel);
                Environment.Exit(-1);
            }
            else
            {
                _notifyicon.Icon = new Icon(stream);
            }

            _notifyicon.Text = "Taskbar Hider";
            _notifyicon.DoubleClick += Show_Clicked;
            _notifyicon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyicon.ContextMenuStrip.Items.Add("显示/隐藏", null, Show_Clicked);
            _notifyicon.ContextMenuStrip.Items.Add("退出", null, Quit_Clicked);
            _notifyicon.Visible = true;
        }

        ~NotifyIcon()
        {
            Dispose();
        }

        public void Dispose()
        {
            _notifyicon.Dispose();
        }

        private void Quit_Clicked(Object? sender, EventArgs e)
        {
            _mainwindow.AboutToClose = true;
            _mainwindow.Close();
        }

        private void Show_Clicked(Object? sender, EventArgs e)
        {
            if (_mainwindow.IsVisible)
            {
                _mainwindow.Hide();
            }
            else
            {
                _mainwindow.Show();
                _mainwindow.Activate();
            }
        }
    }
}