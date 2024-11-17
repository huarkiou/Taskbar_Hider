namespace Tbh
{
    internal class AutoRun
    {
        private readonly string PROGRAM_NAME;
        private readonly string fileDir;
        private const string KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly Microsoft.Win32.RegistryKey? asKey;
        public bool RunOnBoot { get; set; }

        public AutoRun(string programname)
        {
            PROGRAM_NAME = programname;
            string? processPath = System.Environment.ProcessPath;
            if (processPath != null)
            {
                fileDir = processPath;
            }
            else
            {
                fileDir = "";
            }
            asKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(KEY_PATH, true);
            if (asKey == null)
            {
                System.Windows.MessageBox.Show("asKey is null!");
            }
            CheckStartupOnBoot();
        }

        ~AutoRun()
        {
            asKey?.Close();
        }

        public void CheckStartupOnBoot()
        {
            RunOnBoot = (asKey?.GetValue(PROGRAM_NAME) != null);
        }

        public void SetStartupOnBoot(bool enable)
        {
            CheckStartupOnBoot();
            if (enable == RunOnBoot)
            {
                return;
            }

            if (enable == true)
            {
                asKey?.SetValue(PROGRAM_NAME, fileDir);
            }
            else
            {
                asKey?.DeleteValue(PROGRAM_NAME);
            }
            RunOnBoot = enable;
        }
    }
}
