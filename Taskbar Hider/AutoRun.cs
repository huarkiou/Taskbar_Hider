namespace Tbh
{
    internal class AutoRun
    {
        private readonly string _programName;
        private readonly string _fileDir;
        private const string KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly Microsoft.Win32.RegistryKey? _asKey;
        public bool RunOnBoot { get; private set; }

        public AutoRun(string programName)
        {
            _programName = programName;
            _fileDir = System.Environment.ProcessPath ?? "";

            _asKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(KeyPath, true);
            if (_asKey == null)
            {
                System.Windows.MessageBox.Show("asKey is null!");
            }

            CheckStartupOnBoot();
        }

        ~AutoRun()
        {
            _asKey?.Close();
        }

        private void CheckStartupOnBoot()
        {
            RunOnBoot = (_asKey?.GetValue(_programName) != null);
        }

        public void SetStartupOnBoot(bool enable)
        {
            CheckStartupOnBoot();
            if (enable == RunOnBoot)
            {
                return;
            }

            if (enable)
            {
                _asKey?.SetValue(_programName, _fileDir);
            }
            else
            {
                _asKey?.DeleteValue(_programName);
            }

            RunOnBoot = enable;
        }
    }
}