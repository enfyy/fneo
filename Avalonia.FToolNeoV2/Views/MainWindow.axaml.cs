using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using System.ComponentModel;

namespace Avalonia.FToolNeoV2.Views;

public class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Closing += OnClosing;
    }
    
    private void OnClosing(object? sender, CancelEventArgs e) => ViewModel?.OnWindowClosing(sender, e);

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}