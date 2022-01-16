using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.FToolNeoV2.Models;
using Newtonsoft.Json;

namespace Avalonia.FToolNeoV2.Services;

/// <summary>
/// Manages persistence in the application.
/// </summary>
public class PersistenceManager
{
    private static PersistenceManager? _instance;
    
    private static readonly object singletonLock = new();
    
    public static PersistenceManager Instance
    {
        get
        {
            lock (singletonLock)
                return _instance ??= new PersistenceManager();
        }
    }
    
    private ApplicationState? _applicationState;
    
    private PersistenceManager(){}

    /// <summary>
    /// Try to load <see cref="ApplicationState"/> from the persistent file.
    /// If no file exists, a new one with a default <see cref="ApplicationState"/> is created.
    /// </summary>
    /// <returns>The <see cref="ApplicationState"/>.</returns>
    public ApplicationState GetApplicationState()
    {
        if (_applicationState != null) return _applicationState;

        if (!File.Exists(Constants.PERSISTENT_FILE_NAME))
        {
            _applicationState = new ApplicationState();
        }
        else
        {
            var content = File.ReadAllText(Constants.PERSISTENT_FILE_NAME);
            var state = JsonConvert.DeserializeObject<ApplicationState>(content);
            if (state == null) _applicationState = new ApplicationState();

            _applicationState = state ?? new ApplicationState();
        }

        _ = SaveApplicationStateAsync();
        return _applicationState;
    }

    /// <summary>
    /// Save the application state to the persistent file.
    /// </summary>
    /// <param name="state">The state that gets saved. defaults to the most recently loaded state.</param>
    /// <exception cref="ArgumentNullException">Thrown when no state was loaded yet and no state provided as parameter.stants.</exception>
    public async Task SaveApplicationStateAsync(ApplicationState? state = null)
    {
        if (_applicationState == null && state == null)
            throw new ArgumentNullException(nameof(state), "No ApplicationState loaded yet or and parameter is null.");
        
        var applicationState = state ?? _applicationState;
        var contentText = JsonConvert.SerializeObject(applicationState);

        await using var streamWriter = new StreamWriter(File.Create(Constants.PERSISTENT_FILE_NAME), Encoding.UTF8);
        await streamWriter.WriteAsync(contentText);
    }
}