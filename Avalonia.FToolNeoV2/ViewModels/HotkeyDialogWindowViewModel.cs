using System;
using System.Reactive;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Extensions;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Utils;
using Avalonia.Input;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class HotkeyDialogWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, HotkeyDialogResult> OnApplyButtonClicked { get; init; }

    public ReactiveCommand<Unit, HotkeyDialogResult> OnCancelButtonClicked { get; init; }

    public bool HotkeyButtonToggled
    {
        get => _hotkeyButtonToggled;
        set => this.RaiseAndSetIfChanged(ref _hotkeyButtonToggled, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref _isEnabled, value);
            _hotkeyCombination.IsEnabled = value;
        }
    }

    public string HotkeyToggleText
    {
        get => _hotkeyToggleText;
        set => this.RaiseAndSetIfChanged(ref _hotkeyToggleText, value);
    }

    private HotkeyCombination _hotkeyCombination;

    private bool _hotkeyButtonToggled;

    private bool _isEnabled = true;

    private string _hotkeyToggleText = string.Empty;

    private KeyEventArgs? _previousKeyDownEvent;

    public HotkeyDialogWindowViewModel(HotkeyCombination hotkeyCombination)
    {
        _hotkeyCombination = hotkeyCombination;

        IsEnabled = hotkeyCombination.IsEnabled;
        SetHotkeyText(hotkeyCombination);

        OnApplyButtonClicked = ReactiveCommand.Create(() =>
            new HotkeyDialogResult() {HotkeyCombination = _hotkeyCombination, Cancelled = false});

        OnCancelButtonClicked = ReactiveCommand.Create(() =>
            new HotkeyDialogResult() {HotkeyCombination = _hotkeyCombination, Cancelled = true});
    }

    /// <summary>
    /// Set the text of the toggle button to the combination.
    /// </summary>
    /// <param name="hotkeyCombination">The combination.</param>
    private void SetHotkeyText(HotkeyCombination hotkeyCombination)
    {
        string text;
        var additionalText = string.Empty;
        var modifiersText = hotkeyCombination.Modifiers == KeyModifiers.None
            ? string.Empty
            : $"{hotkeyCombination.Modifiers.ToString()} + ";
        
        var keyText = hotkeyCombination.Key == Keys.None
            ? string.Empty
            : hotkeyCombination.Key.ToString();

        if (hotkeyCombination.Key == Keys.F12)
        {
            additionalText = $"{Environment.NewLine}F12 is reserved and thus not allowed.";
            keyText = string.Empty;
        }

        if (modifiersText.IsNullOrWhiteSpace() && keyText.IsNullOrWhiteSpace())
            text = "Click button to set a hotkey.";
        else
            text = $"{modifiersText}{keyText}";

        HotkeyToggleText = $"{text}{additionalText}";
    }

    public void OnHotkeyDown(KeyEventArgs args) => _previousKeyDownEvent = args;

    public void OnHotkeyUp()
    {
        if (!HotkeyButtonToggled)
            return;

        HotkeyButtonToggled = false;

        if (_previousKeyDownEvent == null)
            return;

        var key = _previousKeyDownEvent.Key;
        var modifiers = _previousKeyDownEvent.KeyModifiers;

        if (key == Key.None)
            return;
        
        if (key != Key.F12) // f-12 is reserved and not allowed.
        {
            _hotkeyCombination.Key = KeyInterop.VirtualKeyFromKey(key);
            _hotkeyCombination.Modifiers = modifiers;
        }

        SetHotkeyText(_hotkeyCombination);
    }
}