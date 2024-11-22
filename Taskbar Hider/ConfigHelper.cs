using System;
using System.IO;
using System.Text;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Tbh;

internal static class ConfigHelper
{
    private static readonly string ConfigPath =
        Path.GetDirectoryName(Environment.ProcessPath) + @"\" + App.ProgramName + ".ini";

    public static HOT_KEY_MODIFIERS Modifiers =
        HOT_KEY_MODIFIERS.MOD_ALT;

    public static VIRTUAL_KEY VKey = VIRTUAL_KEY.VK_F2;
    public static bool ShowTaskbarOnStartup = true;

    public static void Load()
    {
        try
        {
            using var fd = new StreamReader(ConfigPath, Encoding.Default);
            while (!fd.EndOfStream)
            {
                var line = fd.ReadLine();
                if (line == null) continue;

                string[] strSplit = line.Split('=');
                switch (strSplit[0])
                {
                    case "Modifiers":
                        Modifiers = (HOT_KEY_MODIFIERS)Convert.ToInt32(strSplit[1]);
                        break;
                    case "VKey":
                        VKey = (VIRTUAL_KEY)Convert.ToInt32(strSplit[1]);
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
        var line = "Modifiers=" + (uint)Modifiers;
        fd.WriteLine(line);
        line = "VKey=" + (ushort)VKey;
        fd.WriteLine(line);
        line = "ShowTaskbarOnStartup=" + ShowTaskbarOnStartup;
        fd.WriteLine(line);
    }
}