using System;
using System.Text.RegularExpressions;

namespace Avalonia.FToolNeoV2.Models;

/// <summary>
/// Settings of the application.
/// </summary>
[Serializable]
public class ApplicationSettings
{
    /// <summary>
    /// The name of the Flyff process.
    /// </summary>
    public string ProcessName { get; set; } = "Neuz";

    /// <summary>
    /// Regex that converts the process window tittle to a character name.
    /// </summary>
    public Regex ProcessWindowTitleRegex { get; set; } = new (@"^\S+");

    /// <summary>
    /// Should the application try to restore attached processes by using the character name?
    /// </summary>
    public bool TryRestoreProcesses { get; set; } = false;

    /// <summary>
    /// Use Shift + F1 - F11 for the first 11 spam slots unless something else is set.
    /// </summary>
    public bool UseDefaultHotkeys { get; set; } = true;

    /// <summary>
    /// Is audio playback muted?
    /// </summary>
    public bool IsAudioMuted { get; set; } = false;

    public ApplicationSettings ShallowCopy() => (ApplicationSettings) MemberwiseClone();
}