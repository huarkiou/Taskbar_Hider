using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Tbh;

namespace Taskbar_Hider_Avalonia;

public partial class App : Application
{
    public const string ProgramName = "Taskbar-Hider";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mutex = new Mutex(true, "Global\\" + ProgramName, out var createdNew);
            if (createdNew)
            {
                mutex.ReleaseMutex();
                ConfigHelper.Load();
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.Closing += CancelAndHideWindow;
            }
            else
            {
                // MessageBox.Show("已启动一个" + ProgramName + "实例,不必重复启动。");
            }
        }
    }

    private static void CancelAndHideWindow(object? sender, WindowClosingEventArgs eventArgs)
    {
        ((Window)sender!).Hide();
        eventArgs.Cancel = true;
    }

    private void Quit_OnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        desktop.MainWindow!.Closing -= CancelAndHideWindow;
        desktop.MainWindow!.Close();
    }

    private void ChangeVisible_OnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        if (desktop.MainWindow!.IsVisible)
        {
            desktop.MainWindow!.Hide();
        }
        else
        {
            desktop.MainWindow!.Show();
        }
    }
}