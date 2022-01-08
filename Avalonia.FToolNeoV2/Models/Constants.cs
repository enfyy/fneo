using System;

namespace Avalonia.FToolNeoV2.Models;

public class Constants
{
    /// <summary>
    /// The minimum delay of the spammer.
    /// </summary>
    public const int MinimumDelay = 50;
    
    /// <summary>
    /// Message that is sent to the process that represents a key being pressed down.
    /// </summary>
    public const UInt32 WM_KEYDOWN = 0x0100;
}