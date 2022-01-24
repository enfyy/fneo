using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;

namespace Avalonia.FToolNeoV2.Views;

public class HotkeyDialogWindow : ReactiveWindow<HotkeyDialogWindowViewModel>
{
    public HotkeyDialogWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(d => d(ViewModel!.OnApplyButtonClicked.Subscribe((result) => Close(result))));
        this.WhenActivated(d => d(ViewModel!.OnCancelButtonClicked.Subscribe((result) => Close(result))));
    }
    private void OnHotkeyUp(object? sender, KeyEventArgs args) => ViewModel?.OnHotkeyUp();

    private void OnHotkeyDown(object? _, KeyEventArgs args) => ViewModel?.OnHotkeyDown(args);
}