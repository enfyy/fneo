using System;
using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Win32;

namespace Avalonia.FToolNeoV2.Views;

public class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public static IntPtr Handle { get; private set; }
    
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        Handle = ((WindowImpl) ((TopLevel) this.GetVisualRoot()).PlatformImpl).Handle.Handle;
    }
}