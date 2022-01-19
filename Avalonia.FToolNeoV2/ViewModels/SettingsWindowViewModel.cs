using System.Reactive;
using System.Text.RegularExpressions;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Services;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class SettingsWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> OnApplyButtonClicked { get; init; }
    
    public ReactiveCommand<Unit, Unit> OnCancelButtonClicked { get; init; }

    public string ProcessName
    {
        get => _processName;
        set
        {
            this.RaiseAndSetIfChanged(ref _processName, value);
            _appSettings.ProcessName = value;
        }
    }

    public string ProcessWindowTitleRegex
    {
        get => _processWindowTitleRegex;
        set
        {
            this.RaiseAndSetIfChanged(ref _processWindowTitleRegex, value);
            _appSettings.ProcessWindowTitleRegex = new Regex(value);
        }
    }

    public bool TryRestoreAttachedProcesses
    {
        get => _tryRestoreAttachedProcesses;
        set
        {
            this.RaiseAndSetIfChanged(ref _tryRestoreAttachedProcesses, value);
            _appSettings.TryRestoreProcesses = value;
        }
    }

    public bool IsAudioMuted
    {
        get => _isAudioMuted;
        set
        {
            this.RaiseAndSetIfChanged(ref _isAudioMuted, value);
            _appSettings.IsAudioMuted = value;
        }
    }

    public bool UseDefaultHotkeys
    {
        get => _useDefaultHotkeys;
        set
        {
            this.RaiseAndSetIfChanged(ref _useDefaultHotkeys, value);
            _appSettings.UseDefaultHotkeys = value;
        }
    }

    private bool _useDefaultHotkeys;

    private bool _isAudioMuted;

    private string _processName = string.Empty;

    private bool _tryRestoreAttachedProcesses;

    private string _processWindowTitleRegex = string.Empty;
    
    private readonly ApplicationSettings _appSettings;

    private ApplicationState _applicationState;

    public SettingsWindowViewModel()
    {
        _applicationState = PersistenceManager.Instance.GetApplicationState().SemiDeepCopy();
        _appSettings = _applicationState.ApplicationSettings;
        ProcessName = _appSettings.ProcessName;
        TryRestoreAttachedProcesses = _appSettings.TryRestoreProcesses;
        IsAudioMuted = _appSettings.IsAudioMuted;
        UseDefaultHotkeys = _appSettings.UseDefaultHotkeys;
        ProcessWindowTitleRegex = _appSettings.ProcessWindowTitleRegex.ToString();
        
        OnApplyButtonClicked = ReactiveCommand.CreateFromTask(async () => await PersistenceManager.Instance.SaveApplicationStateAsync(_applicationState));
        OnCancelButtonClicked = ReactiveCommand.Create(() => {});
    }
}