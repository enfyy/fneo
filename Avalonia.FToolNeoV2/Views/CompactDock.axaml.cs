using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Avalonia.FToolNeoV2.Views;

public class CompactDock : Window
{
    public CompactDock()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.FindControl<Button>("DragMoveButton").PointerPressed += DragMove;
    }

    private void DragMove(object? sender, PointerPressedEventArgs e) => BeginMoveDrag(e);
}