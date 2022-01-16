using System;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Services;
using Avalonia.Input;
using Newtonsoft.Json;

namespace Avalonia.FToolNeoV2.Models;

[Serializable]
public class SpamSlot
{
    /// <summary>
    /// The minimum delay
    /// </summary>
    [JsonIgnore]
    public int MINIMUM_DELAY { get; } = 50;
    
    /// <summary>
    /// The slot index.
    /// </summary>
    public int Index { get; set; }
    
    /// <summary>
    /// The delay of the spammer in ms.
    /// </summary>
    public int Delay { get; set; }
    
    /// <summary>
    /// The Function key that is being spammed.
    /// </summary>
    public Keys? FKey { get; set; }
    
    /// <summary>
    /// The bar number that is being spammed.
    /// </summary>
    public Keys? BarKey { get; set; }
    
    /// <summary>
    /// The hotkey modifier keys.
    /// </summary>
    public KeyModifiers? HotkeyModifierKeys { get; set; }
    
    /// <summary>
    /// The hotkey.
    /// </summary>
    public Keys? Hotkey { get; set; }

    /// <summary>
    /// id of the attached Process.
    /// </summary>
    public int? ProcessId { get; set; }


    /// <summary>
    /// The json constructor
    /// </summary>
    [JsonConstructor]
    public SpamSlot(int index, int delay, Keys? fKey, Keys? barKey, KeyModifiers? hotkeyModifierKeys, Keys? hotkey)
    {
        Index = index;
        Delay = delay;
        FKey = fKey;
        BarKey = barKey;
        HotkeyModifierKeys = hotkeyModifierKeys;
        Hotkey = hotkey;
    }
    
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="index">The slot index.</param>
    /// <param name="applicationState">The application state.</param>
    public SpamSlot(int index, ApplicationState applicationState)
    {
        Index = index;
        Delay = MINIMUM_DELAY;
        FKey = null;
        BarKey = null;
        HotkeyModifierKeys = null;
        Hotkey = null;
        applicationState.SpamSlots.Add(this);
    }
}