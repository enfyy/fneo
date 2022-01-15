using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.FToolNeoV2.ViewModels;
using GlobalHotKeys;
using GlobalHotKeys.Native.Types;

namespace Avalonia.FToolNeoV2.Services;

/// <summary>
/// Manages hotkeys that toggle instances of <see cref="SpamService"/>.
/// </summary>
public static class SpamHotkeyService
{
    private static readonly HotKeyManager _hotKeyManager = new();

    private static readonly Dictionary<int, SpamSlotViewModel> _registrationIdToSpamService = new();

    private static readonly Dictionary<int, IRegistration> _registrations = new();

    private static bool _initialized;
    

    /// <summary>
    /// Registers a hotkey for a spammer that gets toggled on button press.
    /// </summary>
    /// <param name="key">The key that triggers the hotkey.</param>
    /// <param name="modifiers">The modifer keys.</param>
    /// <param name="spammer">The viewmodel of the spammer.</param>
    public static int RegisterHotkey(VirtualKeyCode key, Modifiers modifiers, SpamSlotViewModel spammer)
    {
        var registration = _hotKeyManager.Register(key, modifiers);
        _registrationIdToSpamService[registration.Id] = spammer;
        _registrations[registration.Id] = registration;

        if (_initialized) return registration.Id;
        // initialize on first registration:
        _hotKeyManager.HotKeyPressed
            .ObserveOn(Threading.AvaloniaScheduler.Instance)
            .Subscribe(OnHotkeyPressed);
        
        _initialized = true;
        return registration.Id;
    }

    /// <summary>
    /// Unregisters a hotkey.
    /// </summary>
    public static void UnregisterHotkey(int registrationId)
    {
        var removed = _registrations.Remove(registrationId, out var registration);
        if (!removed) return;

        registration!.Dispose();
    }

    /// <summary>
    /// Unregisters all hotkeys & cleans up in preparation for application exit.
    /// </summary>
    public static void CleanUp()
    {
        foreach (var registration in _registrations.Values) registration.Dispose();
        _hotKeyManager.Dispose();
    }

    private static void OnHotkeyPressed(HotKey hotkey)
    {
        var found = _registrationIdToSpamService.TryGetValue(hotkey.Id, out var spammer);
        if (!found) return;
        spammer!.ToggleSpammer();
    }
}