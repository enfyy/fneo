using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Utils;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class ProcessSelectionViewModel : ViewModelBase
{
    public ObservableCollection<string> ProcessNames { get; }
    
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }
    
    public ReactiveCommand<Unit, Process?> OnApplyButtonClicked { get; init; }
    
    public ReactiveCommand<Unit, Unit> OnRefreshButtonClicked { get; init; }
    
    public ReactiveCommand<Unit, Unit> OnJumpButtonClicked { get; init; }

    private Dictionary<int, Process> _indexedProcesses;
    
    private int _selectedIndex;

    public ProcessSelectionViewModel()
    {
        _indexedProcesses = new Dictionary<int, Process>();
        ProcessNames = new ObservableCollection<string>();
        FetchProcessData();

        var applyAndJumpButtonEnabled = this.WhenAny(x => x.SelectedIndex, x => x.Value != -1);

        OnApplyButtonClicked = ReactiveCommand.Create(() => _indexedProcesses[SelectedIndex], applyAndJumpButtonEnabled)!;

        OnRefreshButtonClicked = ReactiveCommand.Create(FetchProcessData);

        OnJumpButtonClicked =
            ReactiveCommand.Create(() => JumpToProcess(_indexedProcesses[SelectedIndex]), applyAndJumpButtonEnabled);
    }

    /// <summary>
    /// Fetches process data and populates the name list and the dictionary.
    /// </summary>
    private void FetchProcessData()
    {
        var processes = Process.GetProcessesByName(ApplicationState.Instance.ApplicationSettings.ProcessName);
        var i = 0;

        _indexedProcesses.Clear();
        ProcessNames.Clear();
        foreach (var process in processes)
        {
            _indexedProcesses[i] = process;
            ProcessNames.Add(process.MainWindowTitle);
            i++;
        }
    }

    /// <summary>
    /// Brings the process to the foreground.
    /// </summary>
    /// <param name="processToJumpTo">The process that gets jumped to.</param>
    private void JumpToProcess(Process processToJumpTo)
    {
        var handle = processToJumpTo.MainWindowHandle;

        if (User32.IsIconic(handle))
        {
            User32.ShowWindow(handle, 9);
            User32.SetForegroundWindow(handle);
        }
        else
        {
            User32.ShowWindow(handle, 6); //minimize it first lmao. i hate this but it works :)
            User32.ShowWindow(handle, 9);
            User32.SetForegroundWindow(handle);
        }
    }
}