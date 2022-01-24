using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.Views;

public class CompactDockWindow : ReactiveWindow<CompactDockViewModel>
{
    
    public static IntPtr Handle { get; private set; }
    
    public CompactDockWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        Handle = PlatformImpl.Handle.Handle;
        Topmost = true;
        PointerPressed += DragMove;
        this.WhenActivated(d => ViewModel!.SpamSlotViewModels.CollectionChanged += SpamSlotCollectionChanged);
        this.WhenActivated(d => CalculateAndSetHeight(ViewModel!.SpamSlotViewModels.Count));
    }

    private void CalculateAndSetHeight(int count)
    {
        Height = 20 + (29 * count); // the height of one button is 27 and the top-bar height is 15 + margins.
    }

    private void SpamSlotCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (sender is IEnumerable<SpamSlotViewModel> list)
            CalculateAndSetHeight(list.Count());
    }

    private void DragMove(object? sender, PointerPressedEventArgs e) => BeginMoveDrag(e);
}