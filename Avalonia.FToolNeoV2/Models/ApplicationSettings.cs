using System;

namespace Avalonia.FToolNeoV2.Models;

/// <summary>
/// Settings of the application.
/// </summary>
[Serializable]
public struct ApplicationSettings
{
    /// <summary>
    /// The name of the Flyff process.
    /// </summary>
    public string ProcessName { get; set; } = "Neuz";

    /// <summary>
    /// Should previous settings be loaded again on the next application startup?
    /// </summary>
    public bool PersistSettings { get; set; } = true;
}