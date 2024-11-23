using System;
using System.IO;
using System.Text.Json;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Tbh;

internal struct Configuration()
{
    public HOT_KEY_MODIFIERS Modifiers { get; set; } = HOT_KEY_MODIFIERS.MOD_ALT;
    public VIRTUAL_KEY VKey { get; set; } = VIRTUAL_KEY.VK_F2;
    public bool ShowTaskbarOnStartup { get; set; } = true;
}

internal static class ConfigHelper
{
    private static readonly string ConfigPath =
        Path.GetDirectoryName(Environment.ProcessPath) + @"/" + App.ProgramName + ".json";

    public static Configuration Config = new();

    public static void Load()
    {
        if (File.Exists(ConfigPath)) Config = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(ConfigPath));
    }

    public static void Save()
    {
        File.WriteAllText(ConfigPath,
            JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
    }
}