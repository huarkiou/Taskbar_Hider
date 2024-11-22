using System;
using System.Text;
using System.IO;

namespace Tbh
{
    internal static class ConfigHelper
    {
        private static readonly string ConfigPath =
            Path.GetDirectoryName(Environment.ProcessPath) + @"\" + App.ProgramName + ".ini";

        public static HotKeys.HotkeyModifiers Modifiers = HotKeys.HotkeyModifiers.Alt;
        public static HotKeys.EKey VKey = HotKeys.EKey.F2;
        public static bool ShowTaskbarOnStartup = true;

        public static void Load()
        {
            try
            {
                using var fd = new StreamReader(ConfigPath, Encoding.Default);
                while (!fd.EndOfStream)
                {
                    string? line = fd.ReadLine();
                    if (line == null)
                    {
                        continue;
                    }

                    string[] strSplit = line.Split('=');
                    switch (strSplit[0])
                    {
                        case "Modifiers":
                            Modifiers = (HotKeys.HotkeyModifiers)Convert.ToInt32(strSplit[1]);
                            break;
                        case "VKey":
                            VKey = (HotKeys.EKey)Convert.ToInt32(strSplit[1]);
                            break;
                        case "ShowTaskbarOnStartup":
                            ShowTaskbarOnStartup = Convert.ToBoolean(strSplit[1]);
                            break;
                    }
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        public static void Save()
        {
            using var fd = new StreamWriter(ConfigPath);
            string line = "Modifiers=" + ((int)Modifiers).ToString();
            fd.WriteLine(line);
            line = "VKey=" + ((int)VKey).ToString();
            fd.WriteLine(line);
            line = "ShowTaskbarOnStartup=" + ShowTaskbarOnStartup.ToString();
            fd.WriteLine(line);
        }
    }
}