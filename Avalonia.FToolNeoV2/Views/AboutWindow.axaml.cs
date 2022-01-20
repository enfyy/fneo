using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.Views;

public class AboutWindow : ReactiveWindow<AboutWindowViewModel>
{
    public AboutWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(d => d(ViewModel!.OnCloseButtonClicked.Subscribe((_) => Close())));
    }
}