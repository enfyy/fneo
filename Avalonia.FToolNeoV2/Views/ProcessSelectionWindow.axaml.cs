using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;

namespace Avalonia.FToolNeoV2.Views;

public class ProcessSelectionWindow : ReactiveWindow<ProcessSelectionViewModel>
{
    public ProcessSelectionWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d => d(ViewModel!.OnApplyButtonClicked.Subscribe(Close)));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}