using System;
using System.Drawing;
using System.Reflection;
using System.Windows;

namespace Tbh
{
    internal class NotifyIcon
    {
        private readonly System.Windows.Forms.NotifyIcon notifyicon;
        private readonly MainWindow mainwindow;

        public NotifyIcon(MainWindow win)
        {
            mainwindow = win;
            // 初始化托盘菜单
            this.notifyicon = new System.Windows.Forms.NotifyIcon();

            // 导入资源
            //Uri uri = new(@"Resources\Icon_MainWindow.ico", UriKind.Relative);
            //System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(uri);
            //notifyicon.Icon = new Icon(info.Stream);

            // 导入嵌入的资源
            //MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Icon_MainWindow.ico");
            string name = MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace + ".Resources.Icon_MainWindow.ico";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
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
                notifyicon.Icon = new Icon(stream);
            }

            notifyicon.Text = "Taskbar Hider";
            notifyicon.DoubleClick += new EventHandler(this.Show_Clicked);
            notifyicon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyicon.ContextMenuStrip.Items.Add("显示/隐藏", null, Show_Clicked);
            notifyicon.ContextMenuStrip.Items.Add("退出", null, Quit_Clicked);
            notifyicon.Visible = true;
        }

        ~NotifyIcon()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.notifyicon.Dispose();
        }

        public void Quit_Clicked(Object? sender, EventArgs e)
        {
            this.mainwindow.AboutToClose = true;
            mainwindow.Close();
        }

        public void Show_Clicked(Object? sender, EventArgs e)
        {
            if (mainwindow.IsVisible)
            {
                mainwindow.Hide();
            }
            else
            {
                mainwindow.Show();
                mainwindow.Activate();
            }
        }
    }
}
