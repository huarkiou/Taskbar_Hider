using System;
using System.Text;
using System.IO;

namespace Taskbar_Hider
{
    class ConfigHelper
    {
        private static readonly string CONFIG_PATH = ".\\"+App.PROGRAM_NAME+ ".ini";
        public static HotKeys.HotkeyModifiers Modifiers = HotKeys.HotkeyModifiers.Control;
        public static HotKeys.EKey VKey = HotKeys.EKey.Oemtilde;


        public static void Load()
        {
            try
            {
                using (StreamReader fd = new StreamReader(CONFIG_PATH, Encoding.Default))
                {
                    while (!fd.EndOfStream)
                    {
                        string line = fd.ReadLine();
                        string[] strSplit = line.Split('=');
                        if (strSplit[0] == "Modifiers")
                        {
                            Modifiers = (HotKeys.HotkeyModifiers)Convert.ToInt32(strSplit[1]);
                        }
                        else if (strSplit[0] == "VKey")
                        {
                            VKey = (HotKeys.EKey)Convert.ToInt32(strSplit[1]);
                        }
                    }
                }
            }
            catch(FileNotFoundException)
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
            }
            return;
        }
    }
}
