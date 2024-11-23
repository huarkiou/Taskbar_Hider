using System;
using System.IO;
using System.Text.Json;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Taskbar_Hider_Avalonia;

internal struct Configuration
{
    public Configuration()
    {
    }

    public HOT_KEY_MODIFIERS Modifiers { get; set; } = HOT_KEY_MODIFIERS.MOD_ALT;
    public VIRTUAL_KEY VKey { get; set; } = VIRTUAL_KEY.VK_F2;
    public bool ShowTaskbarOnStartup { get; set; } = true;
}

internal class AppConfiguration
{
    private static readonly string ConfigPath =
        Path.GetDirectoryName(Environment.ProcessPath) + @"/" + App.ProgramName + ".json";

    private static readonly JsonSerializerOptions Option = new() { WriteIndented = true };

    public Configuration Config = new();

    private AppConfiguration()
    {
        if (!File.Exists(ConfigPath)) return;
        Config = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(ConfigPath));
    }

    public static AppConfiguration Instance { get; } = new();

    public void Save()
    {
        File.WriteAllText(ConfigPath,
            JsonSerializer.Serialize(this, Option));
    }
}