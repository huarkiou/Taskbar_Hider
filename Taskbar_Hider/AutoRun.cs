namespace Taskbar_Hider
{
    internal class AutoRun
    {
        private readonly string PROGRAM_NAME;
        private readonly string fileDir;
        private const string KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly Microsoft.Win32.RegistryKey asKey;
        public bool RunOnBoot { get; set; }

        public AutoRun(string programname)
        {
            PROGRAM_NAME = programname;
            fileDir = GetType().Assembly.Location;
            asKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(KEY_PATH, true);
            CheckStartupOnBoot();
        }

        ~AutoRun()
        {
            asKey.Close();
        }

        public void CheckStartupOnBoot()
        {
            RunOnBoot = (asKey.GetValue(PROGRAM_NAME + '_' + fileDir.GetHashCode()) != null);
        }

        public void SetStartupOnBoot(bool enable)
        {
            CheckStartupOnBoot();
            if(enable == RunOnBoot)
            {
                return;
            }

            if(enable == true)
            {
                asKey.SetValue(PROGRAM_NAME+'_'+fileDir.GetHashCode(), fileDir);
            }
            else
            {
                asKey.DeleteValue(PROGRAM_NAME+'_'+fileDir.GetHashCode());
            }
            RunOnBoot = enable;
        }
    }
}
