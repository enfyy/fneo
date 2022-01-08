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
    private static ApplicationState? _instance;
    
    private static readonly object singletonLock = new();
    
    public static ApplicationState Instance
    {
        get
        {
            lock (singletonLock)
                return _instance ??= Load();
        }
    }
    
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
    private ApplicationState(List<SpamSlot> spamSlots, ApplicationSettings settings)
    {
        SpamSlots = spamSlots;
        ApplicationSettings = settings;
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    private ApplicationState()
    {
        SpamSlots = new List<SpamSlot>
        {
            new (),
            new ()
        };
        ApplicationSettings = new ApplicationSettings();
    }

    /// <summary>
    /// Tries to load application state from json.
    /// Creates a new state instead if loading failed.
    /// </summary>
    /// <returns>The <see cref="ApplicationState"/></returns>
    public static ApplicationState Load()
    {
        //TODO: try load from json 
        return new ApplicationState();
    }
}