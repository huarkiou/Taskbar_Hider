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

            bool runone;
            var mutex = new System.Threading.Mutex(true, "Global\\" + PROGRAM_NAME, out runone);
            if(runone)
            {
                mutex.ReleaseMutex();
                ConfigHelper.Load();
                wnd = new MainWindow();
                //wnd.Height = 0;
                //wnd.Width = 0;
                wnd.ShowActivated = false;
                wnd.Show();
                wnd.Hide();
                //wnd.Height = 330;
                //wnd.Width = 440;
                //wnd.MinHeight = 300;
                //wnd.MinWidth = 400;
            }
            else
            {
                MessageBox.Show("已启动一个"+ PROGRAM_NAME + "实例,不必重复启动。");
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
