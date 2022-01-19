using System;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Services;
using Avalonia.FToolNeoV2.Utils;
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
    /// The hotkey combination that activates this slot if enabled.
    /// </summary>
    public HotkeyCombination HotkeyCombination { get; set; }

    /// <summary>
    /// The id of the attached Process.
    /// </summary>
    public int? ProcessId { get; set; }

    /// <summary>
    /// The name of the character at the attached process.
    /// </summary>
    public string? CharacterName { get; set; }


    /// <summary>
    /// The json constructor
    /// </summary>
    [JsonConstructor]
    public SpamSlot(int index, int delay, Keys? fKey, Keys? barKey, HotkeyCombination hotkeyCombination)
    {
        Index = index;
        Delay = delay;
        FKey = fKey;
        BarKey = barKey;
        HotkeyCombination = hotkeyCombination;
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

        if (applicationState.ApplicationSettings.UseDefaultHotkeys && index < 12)
        {
            HotkeyCombination = new HotkeyCombination()
            {
                IsEnabled = true,
                Key = KeyInterop.IndexToFKey(index)!.Value,
                Modifiers = KeyModifiers.Shift
            };
        }
        else
            HotkeyCombination = new HotkeyCombination();

        applicationState.SpamSlots.Add(this);
    }
}