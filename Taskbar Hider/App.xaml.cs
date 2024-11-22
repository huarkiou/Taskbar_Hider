using System.Threading;
using System.Windows;

namespace Tbh;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public const string ProgramName = "Taskbar-Hider";
    private MainWindow? _wnd;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mutex = new Mutex(true, "Global\\" + ProgramName, out var runone);
        if (runone)
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

    protected override void OnExit(ExitEventArgs e)
    {
        ConfigHelper.Save();
        base.OnExit(e);
    }

    private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        ConfigHelper.Save();
        //wnd.AboutToClose = true;
        //wnd.Close();
    }
}