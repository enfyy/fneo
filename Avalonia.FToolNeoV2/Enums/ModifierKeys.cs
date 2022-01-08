using System;

namespace Avalonia.FToolNeoV2.Enums;

/// <summary>
/// The ModifierKeys enumeration describes a set of common keys used to modify other input operations.
/// Originally taken from System.Input.Controls.ModifierKeys
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>
    ///    No modifiers are pressed.
    /// </summary>
    None = 0,

    /// <summary>
    ///    An alt key.
    /// </summary>
    Alt = 1,
        
    /// <summary>
    ///    A control key.
    /// </summary>
    Control = 2,

    /// <summary>
    ///    A shift key.
    /// </summary>
    Shift = 4,
        
    /// <summary>
    ///    A windows key.
    /// </summary>
    Windows = 8
}