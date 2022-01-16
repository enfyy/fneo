using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Services;
using GlobalHotKeys.Native.Types;
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
            SpamSlot.FKey = IndexToFKey(value);
            ComboBoxesHaveValidValues = SpamSlot.BarKey != null || SpamSlot.FKey != null;
        }
    }

    public int SelectedBarIndex
    {
        get => _selectedBarIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBarIndex, value);
            SpamSlot.BarKey = IndexToBarKey(value);
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

    public Interaction<ProcessSelectionViewModel, Process?> ProcessSelectionDialog { get; } = null!;

    public ReactiveCommand<Unit, Unit> OnSelectProcessButtonClicked { get; } = null!;

    public ReactiveCommand<Unit, Unit> OnHotkeyButtonClicked { get; } = null!;

    public ReactiveCommand<Unit, Unit> OnStartButtonClicked { get; } = null!;

    public SpamService? SpamService { get; private set; }

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
        Index = $"#{index}";
        SpamSlot = spamSlot;
        DelayText = spamSlot.Delay.ToString();
        if (spamSlot.BarKey != null) SelectedBarIndex = BarKeyToIndex(spamSlot.BarKey.Value);
        if (spamSlot.FKey != null) SelectedFKeyIndex = FKeyToIndex(spamSlot.FKey.Value);
        
        // try to recover attached process  
        if (spamSlot.ProcessId != null)
        {
            try
            {
                var process = Process.GetProcessById(spamSlot.ProcessId.Value);
                AttachToProcess(process);
            }
            //process not found i guess...
            catch (InvalidOperationException) { }
            catch (ArgumentException) { }
        }
        
        //TODO: recover hotkey

        ProcessSelectionDialog = new Interaction<ProcessSelectionViewModel, Process?>();
        
        OnSelectProcessButtonClicked = ReactiveCommand.CreateFromTask( async () =>
        {
            var processSelection = new ProcessSelectionViewModel();
            
            var result = await ProcessSelectionDialog.Handle(processSelection);

            if (result != null) AttachToProcess(result);
        });
        
        OnHotkeyButtonClicked = ReactiveCommand.Create(() =>
        {
            if (SpamService == null) return;
            if (_hotkeyId == null)
                _hotkeyId = SpamHotkeyService.RegisterHotkey(VirtualKeyCode.KEY_1, Modifiers.Shift, this);
            else
                SpamHotkeyService.UnregisterHotkey(_hotkeyId.Value);
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
        if (SpamService == null) return;

        SpamService.Toggle();
        IsSpamming = SpamService.IsActive;
    }

    private void AttachToProcess(Process process)
    {
        AttachedTo = process.MainWindowTitle;
        SpamSlot.ProcessId = process.Id;
        SpamService = new SpamService(process, SpamSlot);
        ProcessSelected = true;
    }

    private static Keys? IndexToFKey(int index)
    {
        return index switch
        {
            1 => Keys.F1,
            2 => Keys.F2,
            3 => Keys.F3,
            4 => Keys.F4,
            5 => Keys.F5,
            6 => Keys.F6,
            7 => Keys.F7,
            8 => Keys.F8,
            9 => Keys.F9,
            _ => null
        };
    }

    private static Keys? IndexToBarKey(int index)
    {
        return index switch
        {
            1 => Keys.D1,
            2 => Keys.D2,
            3 => Keys.D3,
            4 => Keys.D4,
            5 => Keys.D5,
            6 => Keys.D6,
            7 => Keys.D7,
            8 => Keys.D8,
            9 => Keys.D9,
            _ => null
        };
    }
    
    private static int FKeyToIndex(Keys key)
    {
        return key switch
        {
            Keys.F1 => 1,
            Keys.F2 => 2,
            Keys.F3 => 3,
            Keys.F4 => 4,
            Keys.F5 => 5,
            Keys.F6 => 6,
            Keys.F7 => 7,
            Keys.F8 => 8,
            Keys.F9 => 9,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static int BarKeyToIndex(Keys key)
    {
        return key switch
        {
            Keys.D1 => 1,
            Keys.D2 => 2,
            Keys.D3 => 3,
            Keys.D4 => 4,
            Keys.D5 => 5,
            Keys.D6 => 6,
            Keys.D7 => 7,
            Keys.D8 => 8,
            Keys.D9 => 9,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}