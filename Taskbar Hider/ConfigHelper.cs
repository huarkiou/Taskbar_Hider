using System;
using System.Text;
using System.IO;

namespace Tbh
{
    class ConfigHelper
    {
        private static readonly string CONFIG_PATH = System.IO.Path.GetDirectoryName(Environment.ProcessPath) + @"\" + App.PROGRAM_NAME + ".ini";
        public static HotKeys.HotkeyModifiers Modifiers = HotKeys.HotkeyModifiers.Alt;
        public static HotKeys.EKey VKey = HotKeys.EKey.F2;
        public static bool ShowTaskbarOnStartup = true;


        public static void Load()
        {
            try
            {
                using (StreamReader fd = new StreamReader(CONFIG_PATH, Encoding.Default))
                {
                    while (!fd.EndOfStream)
                    {
                        string? line = fd.ReadLine();
                        if(line == null)
                        {
                            continue;
                        }
                        string[] strSplit = line.Split('=');
                        if (strSplit[0] == "Modifiers")
                        {
                            Modifiers = (HotKeys.HotkeyModifiers)Convert.ToInt32(strSplit[1]);
                        }
                        else if (strSplit[0] == "VKey")
                        {
                            VKey = (HotKeys.EKey)Convert.ToInt32(strSplit[1]);
                        }
                        else if (strSplit[0] == "ShowTaskbarOnStartup")
                        {
                            ShowTaskbarOnStartup = Convert.ToBoolean(strSplit[1]);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {

            }
            return;
        }

        public static void Save()
        {
            using (StreamWriter fd = new StreamWriter(CONFIG_PATH))
            {
                string line = "Modifiers=" + ((int)Modifiers).ToString();
                fd.WriteLine(line);
                line = "VKey=" + ((int)VKey).ToString();
                fd.WriteLine(line);
                line = "ShowTaskbarOnStartup=" + ShowTaskbarOnStartup.ToString();
                fd.WriteLine(line);
            }
            return;
        }
    }
}
