using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Principal;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Services;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<SpamSlotViewModel> SpamSlotViews { get; }
    
    public ReactiveCommand<Unit, Unit> OnRemoveButtonClicked { get; init; }
    
    public ReactiveCommand<Unit, Unit> OnSettingsButtonClicked { get; init; }
    
    public ReactiveCommand<Unit, Unit> OnAboutButtonClicked { get; init; }

    public Interaction<SettingsWindowViewModel, Unit> SettingsWindowDialog { get; } = null!;
    
    public Interaction<AboutWindowViewModel, Unit> AboutWindowDialog { get; } = null!;

    private bool RemoveButtonActive
    {
        get => _removeButtonActive;
        set => this.RaiseAndSetIfChanged(ref _removeButtonActive, value);
    }

    private bool IsNotElevated
    {
        get => _isNotElevated;
        set => this.RaiseAndSetIfChanged(ref _isNotElevated, value);
    }

    private bool _removeButtonActive;
    
    private bool _isNotElevated;

    public MainWindowViewModel()
    {
        //yes, this only works on windows.
#pragma warning disable CA1416 
        using (var identity = WindowsIdentity.GetCurrent())
        {
            var principal = new WindowsPrincipal(identity);
            IsNotElevated = !principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
#pragma warning restore CA1416
        
        SpamSlotViews = new ObservableCollection<SpamSlotViewModel>();
        SpamSlotViews.CollectionChanged += OnSpamSlotViewsCollectionChanged;

        foreach (var slot in PersistenceManager.Instance.GetApplicationState().SpamSlots)
            SpamSlotViews.Add(new SpamSlotViewModel(SpamSlotViews.Count + 1, slot));

        OnRemoveButtonClicked = ReactiveCommand.Create(() =>
        {
            var last = SpamSlotViews.LastOrDefault();
            if (last == null)
                return;

            if (last.IsSpamming)
                last.SpamService?.Stop();
            
            last.UnregisterHotkey();

            SpamSlotViews.Remove(last);
            PersistenceManager.Instance.GetApplicationState().SpamSlots.Remove(last.SpamSlot);
        });
        
        SettingsWindowDialog = new Interaction<SettingsWindowViewModel, Unit>();
        
        OnSettingsButtonClicked = ReactiveCommand.CreateFromTask( async () =>
        {
            var settingsViewModel = new SettingsWindowViewModel(); 
            await SettingsWindowDialog.Handle(settingsViewModel);
        });
        
        AboutWindowDialog = new Interaction<AboutWindowViewModel, Unit>();
        
        OnAboutButtonClicked = ReactiveCommand.CreateFromTask( async () =>
        {
            var aboutViewModel = new AboutWindowViewModel(); 
            await AboutWindowDialog.Handle(aboutViewModel);
        });
    }

    public void OnAddButtonClicked() =>
        SpamSlotViews.Add(new SpamSlotViewModel(SpamSlotViews.Count + 1,
            new SpamSlot(SpamSlotViews.Count + 1, PersistenceManager.Instance.GetApplicationState())));

    private void OnSpamSlotViewsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) 
        => RemoveButtonActive = SpamSlotViews.Count > 1;
}