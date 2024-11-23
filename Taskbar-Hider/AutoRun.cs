using System;
using Microsoft.Win32;

namespace Taskbar_Hider_Avalonia;

internal class AutoRun
{
    private const string KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private readonly RegistryKey? _asKey;
    private readonly string _fileDir;
    private readonly string _programName;

    public AutoRun(string programName)
    {
        _programName = programName;
        _fileDir = Environment.ProcessPath ?? "";

        _asKey = Registry.CurrentUser.OpenSubKey(KeyPath, true);
        if (_asKey == null) throw new InvalidOperationException();

        CheckStartupOnBoot();
    }

    public bool RunOnBoot { get; private set; }

    ~AutoRun()
    {
        _asKey?.Close();
    }

    private void CheckStartupOnBoot()
    {
        RunOnBoot = _asKey?.GetValue(_programName) != null;
    }

    public void SetStartupOnBoot(bool enable)
    {
        CheckStartupOnBoot();
        if (enable == RunOnBoot) return;

        if (enable)
            _asKey?.SetValue(_programName, _fileDir);
        else
            _asKey?.DeleteValue(_programName);

        RunOnBoot = enable;
    }
}