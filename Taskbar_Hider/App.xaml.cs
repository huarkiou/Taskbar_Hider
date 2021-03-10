using System.Windows;

namespace Taskbar_Hider
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string PROGRAM_NAME = "Taskbar Hider";
        private MainWindow wnd;
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            var mutex = new System.Threading.Mutex(true, "Global\\" + PROGRAM_NAME, out bool runone);
            if (runone)
            {
                mutex.ReleaseMutex();
                ConfigHelper.Load();
                wnd = new MainWindow();
                wnd.ShowActivated = false;
                wnd.Show();
                wnd.Hide();
            }
            else
            {
                MessageBox.Show("已启动一个"+ PROGRAM_NAME + "实例,不必重复启动。");
                if (wnd != null)
                {
                    wnd.AboutToClose = true;
                    wnd.Close();
                }
            }

            return;
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
}
