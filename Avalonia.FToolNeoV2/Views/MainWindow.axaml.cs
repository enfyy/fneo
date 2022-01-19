using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Win32;
using ReactiveUI;

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
        this.WhenActivated(d => d(ViewModel!.SettingsWindowDialog.RegisterHandler(ShowSettingsSelectionDialogAsync)));
    }

    private async Task ShowSettingsSelectionDialogAsync(InteractionContext<SettingsWindowViewModel, Unit> interaction)
    {
        var dialog = new SettingsWindow()
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<Unit>(this);
        interaction.SetOutput(result);
    }
}