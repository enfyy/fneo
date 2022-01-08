using System;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Utils;

namespace Avalonia.FToolNeoV2.Models;

/// <summary>
/// Represents a registered hotkey
/// </summary>
public class RegisteredHotkey
{
    public IntPtr Handle { get; init; }
    
    public int Id { get; init; }
    
    public ModifierKeys Modifiers { get; init; }
    
    public Keys Keys { get; init; }
    
    public RegisteredHotkey(IntPtr handle, ModifierKeys modifiers, Keys keys)
    {
        Handle = handle;
        Id = Guid.NewGuid().GetHashCode();
        Modifiers = modifiers;
        Keys = keys;
        User32.RegisterHotKey(handle, Id, modifiers, keys);
    }

    public void Unregister() => User32.UnregisterHotKey(Handle, Id);
}