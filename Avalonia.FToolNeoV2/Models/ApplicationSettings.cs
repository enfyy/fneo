using System;

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
    /// Should previous settings be loaded again on the next application startup?
    /// </summary>
    public bool PersistSettings { get; set; } = true;
    
    /// <summary>
    /// Regex that converts the process name to a character name.
    /// </summary>
    public string ProcessNameRegex { get; set; }

    /// <summary>
    /// Should the application try to restore attached processes by using the character name?
    /// </summary>
    public bool TryRestoreProcesses { get; set; } = false;
}