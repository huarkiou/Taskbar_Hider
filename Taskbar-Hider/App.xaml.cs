using System.Threading;
using System.Windows;

namespace Tbh;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public const string ProgramName = "Taskbar-Hider";
    private MainWindow? _wnd;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mutex = new Mutex(true, "Global\\" + ProgramName, out var createdNew);
        if (createdNew)
        {
            mutex.ReleaseMutex();
            ConfigHelper.Load();
            _wnd = new MainWindow
            {
                ShowActivated = false
            };
            _wnd.Show();
            _wnd.Hide();
        }
        else
        {
            MessageBox.Show("已启动一个" + ProgramName + "实例,不必重复启动。");
            if (_wnd != null)
            {
                _wnd.AboutToClose = true;
                _wnd.Close();
            }
        }
    }


    private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        ConfigHelper.Save();
    }
}