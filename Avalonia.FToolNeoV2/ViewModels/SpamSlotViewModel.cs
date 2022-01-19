using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Extensions;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Services;
using Avalonia.FToolNeoV2.Utils;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class SpamSlotViewModel : ViewModelBase
{
    public string Index { get; set; } = null!;

    public int SelectedFKeyIndex
    {
        get => _selectedFKeyIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFKeyIndex, value);
            SpamSlot.FKey = KeyInterop.IndexToFKey(value);
            ComboBoxesHaveValidValues = SpamSlot.BarKey != null || SpamSlot.FKey != null;
        }
    }

    public int SelectedBarIndex
    {
        get => _selectedBarIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBarIndex, value);
            SpamSlot.BarKey = KeyInterop.IndexToBarKey(value);
            ComboBoxesHaveValidValues = SpamSlot.BarKey != null || SpamSlot.FKey != null;
        }
    }

    public string AttachedTo
    {
        get => _attachedTo ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _attachedTo, value);
    }

    public bool IsSpamming
    {
        get => _isSpamming;
        set => this.RaiseAndSetIfChanged(ref _isSpamming, value);
    }

    public string DelayText
    {
        get => _delayText ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _delayText, value);
            if (!int.TryParse(value, out var parsed)) return;
            SpamSlot.Delay = parsed;
        }
    }

    public bool ComboBoxesHaveValidValues
    {
        get => _comboBoxesHaveValidValues;
        set
        {
            _comboBoxesHaveValidValues = value;
            StartButtonIsEnabled = ProcessSelected && _comboBoxesHaveValidValues;
        }
    }
    
    public bool ProcessSelected
    {
        get => _processSelected;
        set
        {
            _processSelected = value;
            StartButtonIsEnabled = _processSelected && ComboBoxesHaveValidValues;
        }
    }

    public bool StartButtonIsEnabled
    {
        get => _startButtonIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _startButtonIsEnabled, value);
    }

    public SpamSlot SpamSlot { get; } = null!;

    public Interaction<ProcessSelectionWindowViewModel, Process?> ProcessSelectionDialog { get; } = null!;
    
    public Interaction<HotkeyDialogWindowViewModel, HotkeyCombination> HotkeyDialog { get; } = null!;

    public ReactiveCommand<Unit, Unit> OnSelectProcessButtonClicked { get; } = null!;

    public ReactiveCommand<Unit, Unit> OnHotkeyButtonClicked { get; } = null!;

    public ReactiveCommand<Unit, Unit> OnStartButtonClicked { get; } = null!;

    public SpamService? SpamService { get; private set; }

    private ApplicationState _appState = null!;

    private bool _processSelected;

    private string? _attachedTo;

    private string? _delayText;

    private int _selectedFKeyIndex;

    private int _selectedBarIndex;

    private bool _isSpamming;

    private bool _comboBoxesHaveValidValues;

    private bool _startButtonIsEnabled;
    
    private int? _hotkeyId;

    public SpamSlotViewModel(int index, SpamSlot spamSlot)
    {
        _appState = PersistenceManager.Instance.GetApplicationState();
        Index = $"#{index}";
        SpamSlot = spamSlot;
        DelayText = spamSlot.Delay.ToString();
        if (spamSlot.BarKey != null) SelectedBarIndex = KeyInterop.BarKeyToIndex(spamSlot.BarKey.Value);
        if (spamSlot.FKey != null) SelectedFKeyIndex = KeyInterop.FKeyToIndex(spamSlot.FKey.Value);
        
        AttemptProcessRecovery(spamSlot);

        HandleHotkeyRegistry(spamSlot.HotkeyCombination);

        ProcessSelectionDialog = new Interaction<ProcessSelectionWindowViewModel, Process?>();
        
        OnSelectProcessButtonClicked = ReactiveCommand.CreateFromTask( async () =>
        {
            var viewModel = new ProcessSelectionWindowViewModel();
            
            var result = await ProcessSelectionDialog.Handle(viewModel);

            if (result != null) AttachToProcess(result);
        });

        HotkeyDialog = new Interaction<HotkeyDialogWindowViewModel, HotkeyCombination>();
        
        OnHotkeyButtonClicked = ReactiveCommand.CreateFromTask(async () =>
        {
            var viewModel = new HotkeyDialogWindowViewModel(SpamSlot.HotkeyCombination);

            SpamSlot.HotkeyCombination = await HotkeyDialog.Handle(viewModel);
            HandleHotkeyRegistry(SpamSlot.HotkeyCombination);
        });

        OnStartButtonClicked = ReactiveCommand.Create(ToggleSpammer);
    }

    public SpamSlotViewModel()
    {
        // ** nothing **
    }

    /// <summary>
    /// Toggle the spammer.
    /// </summary>
    public void ToggleSpammer()
    {
        if (SpamService == null || !StartButtonIsEnabled) return;
        
        AudioService.Instance.PlaySound(IsSpamming ? AudioAsset.Pause : AudioAsset.Play);
        
        SpamService.Toggle();
        IsSpamming = SpamService.IsActive;
    }

    /// <summary>
    /// Unregisters the hotkey of this slot if it has one registered.
    /// </summary>
    public void UnregisterHotkey()
    {
        if (_hotkeyId == null) 
            return;
            
        SpamHotkeyService.UnregisterHotkey(_hotkeyId.Value);
        _hotkeyId = null;
    }

    /// <summary>
    /// Attempt to recover the previously attached process by id first, then by character name.
    /// </summary>
    /// <param name="spamSlot"> </param>
    private void AttemptProcessRecovery(SpamSlot spamSlot)
    {
        // try to recover attached process  
        if (spamSlot.ProcessId != null)
        {
            try
            {
                var process = Process.GetProcessById(spamSlot.ProcessId.Value);
                AttachToProcess(process);
                return;
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                spamSlot.ProcessId = null;
            }
        }

        if (spamSlot.CharacterName.IsNullOrWhiteSpace()) return;

        try
        {
            var processName = _appState.ApplicationSettings.ProcessName;
            var processes = Process.GetProcessesByName(processName);
            
            var process = processes.FirstOrDefault(p =>
            {
                var name = _appState.ApplicationSettings.ProcessWindowTitleRegex.Match(p.MainWindowTitle).Value;
                return !name.IsNullOrWhiteSpace() && name == spamSlot.CharacterName;
            });
            
            if (process != null)
                AttachToProcess(process);
            else
                SpamSlot.CharacterName = null;
        }
        catch (InvalidOperationException)
        {
            SpamSlot.CharacterName = null;
        }
        
    }

    /// <summary>
    /// Attach the spammer to a process.
    /// </summary>
    /// <param name="process">The process to attach to.</param>
    private void AttachToProcess(Process process)
    {
        var windowTitle = process.MainWindowTitle;
        AttachedTo = windowTitle;
        SpamSlot.ProcessId = process.Id;
        
        var regex = _appState.ApplicationSettings.ProcessWindowTitleRegex;
        var name = regex.Match(windowTitle).Value;
        if (!name.IsNullOrWhiteSpace()) SpamSlot.CharacterName = name; 
        
        SpamService = new SpamService(process, SpamSlot);
        ProcessSelected = true;
    }

    /// <summary>
    /// Register or unregister a hotkey.
    /// </summary>
    /// <param name="hotkeyCombination">The hotkey combination that gets registered if its enabled.</param>
    private void HandleHotkeyRegistry(HotkeyCombination hotkeyCombination)
    {
        if (hotkeyCombination.IsEnabled) 
            _hotkeyId = SpamHotkeyService.RegisterHotkey(hotkeyCombination, this);
        else
            UnregisterHotkey();
    }
}