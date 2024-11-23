using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Taskbar_Hider_Avalonia;

public class App : Application
{
    public const string ProgramName = "Taskbar-Hider";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var mutex = new Mutex(true, "Global\\" + ProgramName, out var createdNew);
        if (!createdNew) return;
        mutex.ReleaseMutex();
        desktop.MainWindow = new MainWindow();
        desktop.MainWindow.Closing += CancelAndHideWindow;
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
            desktop.MainWindow!.Hide();
        else
            desktop.MainWindow!.Show();
    }
}