using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Taskbar_Hider;

public partial class MessageBox : Window
{
    public MessageBox(string msg)
    {
        InitializeComponent();
        MessageLabel.Content = msg;
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}