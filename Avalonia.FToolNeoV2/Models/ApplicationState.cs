using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Avalonia.FToolNeoV2.Models;

/// <summary>
/// Holds the state of the application.
/// </summary>
[Serializable]
public class ApplicationState
{
    /// <summary>
    /// List of the spammer slots.
    /// </summary>
    public List<SpamSlot> SpamSlots { get; set; }
    
    /// <summary>
    /// The settings of the application.
    /// </summary>
    public ApplicationSettings ApplicationSettings { get; set; }

    /// <summary>
    /// The json constructor.
    /// </summary>
    /// <param name="spamSlots">The spam slots.</param>
    /// <param name="settings">The application settings.</param>
    [JsonConstructor]
    public ApplicationState(List<SpamSlot> spamSlots, ApplicationSettings settings)
    {
        SpamSlots = spamSlots;
        ApplicationSettings = settings;
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public ApplicationState()
    {
        SpamSlots = new List<SpamSlot>();
        _ = new SpamSlot(1, this);
        _ = new SpamSlot(2, this);
        ApplicationSettings = new ApplicationSettings();
    }


    /// <summary>
    /// Create a copy that also references a new copy of the settings.
    /// </summary>
    /// <returns>The copy of the application state and settings.</returns>
    public ApplicationState SemiDeepCopy()
    {
        var settingsCopy = ApplicationSettings.ShallowCopy();
        var copy = (ApplicationState) MemberwiseClone();
        copy.ApplicationSettings = settingsCopy;
        return copy;
    }
}