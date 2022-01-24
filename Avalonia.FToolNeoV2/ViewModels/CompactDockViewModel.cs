using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Avalonia.FToolNeoV2.ViewModels;

public class CompactDockViewModel : ViewModelBase
{
    public ObservableCollection<SpamSlotViewModel> SpamSlotViewModels { get; init; } = null!;

    public ObservableCollection<CompactDockButtonViewModel> SpamButtons { get; init; } = null!;

    private int _buttonCount;

    public CompactDockViewModel(ObservableCollection<SpamSlotViewModel> spamSlotViewModels)
    {
        SpamSlotViewModels = spamSlotViewModels;
        SpamSlotViewModels.CollectionChanged += OnSpamSlotCollectionChanged;
        SpamButtons = new ObservableCollection<CompactDockButtonViewModel>();
        _buttonCount = SpamSlotViewModels.Count; 

        foreach (var spamSlotViewModel in spamSlotViewModels)
            SpamButtons.Add(new CompactDockButtonViewModel(spamSlotViewModel));
    }

    public CompactDockViewModel()
    {
       // ** nothing **
    }

    private void OnSpamSlotCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (SpamSlotViewModels.Count == 0) 
            return;

        if (_buttonCount > SpamSlotViewModels.Count)
            SpamButtons.Remove(SpamButtons.Last());
        else
            SpamButtons.Add(new CompactDockButtonViewModel(SpamSlotViewModels.Last()));

        _buttonCount = SpamSlotViewModels.Count;
    }
}