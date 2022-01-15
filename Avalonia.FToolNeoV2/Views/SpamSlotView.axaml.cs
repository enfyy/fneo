using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.Views;

public class SpamSlotView : ReactiveUserControl<SpamSlotViewModel>
{
    public SpamSlotView()
    {
        InitializeComponent();
    }
    
    private async Task ShowProcessSelectionDialogAsync(InteractionContext<ProcessSelectionViewModel, Process?> interaction)
    {
        var dialog = new ProcessSelectionWindow
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<Process?>((MainWindow) VisualRoot!);
        interaction.SetOutput(result);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(d => d(ViewModel!.ProcessSelectionDialog.RegisterHandler(ShowProcessSelectionDialogAsync)));
    }
}