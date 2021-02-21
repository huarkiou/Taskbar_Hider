using System;
using System.Drawing;

namespace Taskbar_Hider
{
    internal class NotifyIcon
    {
        private readonly System.Windows.Forms.NotifyIcon notifyicon;
        private readonly System.Windows.Forms.ContextMenu contextmenu;
        private readonly System.Windows.Forms.MenuItem quitItem;
        private readonly System.Windows.Forms.MenuItem showItem;
        private readonly System.ComponentModel.IContainer components;
        private readonly MainWindow mainwindow;

        public NotifyIcon(MainWindow win)
        {
            mainwindow = win;
            // 初始化托盘菜单
            this.contextmenu = new System.Windows.Forms.ContextMenu();
            this.quitItem = new System.Windows.Forms.MenuItem();
            this.showItem = new System.Windows.Forms.MenuItem();
            this.components = new System.ComponentModel.Container();

            this.contextmenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.quitItem, this.showItem });

            this.quitItem.Index = 0;
            this.quitItem.Text = "退出";
            this.quitItem.Click += new EventHandler(this.QuitItem_Clicked);

            this.showItem.Index = 1;
            this.showItem.Text = "显示/隐藏";
            this.showItem.Click += new EventHandler(this.ShowItem_Clicked);

            this.notifyicon = new System.Windows.Forms.NotifyIcon(this.components);

            // 导入资源
            //Uri uri = new Uri(@"Resources\Icon_MainWindow.ico", UriKind.Relative);
            //System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(uri);
            //notifyicon.Icon = new Icon(info.Stream);

            // 导入嵌入的资源
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Icon_MainWindow.ico";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(name);
            notifyicon.Icon = new Icon(stream);

            notifyicon.ContextMenu = contextmenu;
            notifyicon.Text = "Taskbar Hider";
            notifyicon.Visible = true;
            notifyicon.DoubleClick += new EventHandler(this.ShowItem_Clicked);
        }

        ~NotifyIcon()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.components.Dispose();
            this.showItem.Dispose();
            this.quitItem.Dispose();
            this.contextmenu.Dispose();
            this.notifyicon.Dispose();
        }

        public void QuitItem_Clicked(Object sender, EventArgs e)
        {
            this.mainwindow.AboutToClose = true;
            mainwindow.Close();
        }

        public void ShowItem_Clicked(Object sender, EventArgs e)
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
