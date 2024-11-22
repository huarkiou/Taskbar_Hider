using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Tbh;

internal class NotifyIcon
{
    private readonly MainWindow _mainWindow;
    private readonly System.Windows.Forms.NotifyIcon _notifyIcon;

    public NotifyIcon(MainWindow win)
    {
        _mainWindow = win;
        // 初始化托盘菜单
        _notifyIcon = new System.Windows.Forms.NotifyIcon();

        // 导入嵌入的资源
        var name = MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace + ".Resources.Icon_MainWindow.ico";
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        if (stream == null)
        {
            MessageBox.Show("Cannot file notifyicon", "Error", MessageBoxButton.OKCancel);
            Environment.Exit(-1);
        }
        else
        {
            _notifyIcon.Icon = new Icon(stream);
        }

        _notifyIcon.Text = "Taskbar Hider";
        _notifyIcon.DoubleClick += Show_Clicked;
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("显示/隐藏", null, Show_Clicked);
        _notifyIcon.ContextMenuStrip.Items.Add("退出", null, Quit_Clicked);
        _notifyIcon.Visible = true;
    }

    ~NotifyIcon()
    {
        Dispose();
    }

    public void Dispose()
    {
        _notifyIcon.Dispose();
    }

    private void Quit_Clicked(object? sender, EventArgs e)
    {
        _mainWindow.AboutToClose = true;
        _mainWindow.Close();
    }

    private void Show_Clicked(object? sender, EventArgs e)
    {
        if (_mainWindow.IsVisible)
        {
            _mainWindow.Hide();
        }
        else
        {
            _mainWindow.Show();
            _mainWindow.Activate();
        }
    }
}