using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Avalonia.FToolNeoV2.Views;

public class CompactDockButtonView : ReactiveUserControl<CompactDockButtonViewModel>
{
    public CompactDockButtonView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}