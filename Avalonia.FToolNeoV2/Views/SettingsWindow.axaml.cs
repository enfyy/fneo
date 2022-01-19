using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.Views;

public class SettingsWindow : ReactiveWindow<SettingsWindowViewModel>
{
    public SettingsWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(d => d(ViewModel!.OnApplyButtonClicked.Subscribe((_) => Close())));
        this.WhenActivated(d => d(ViewModel!.OnCancelButtonClicked.Subscribe((_) => Close())));
    }
}