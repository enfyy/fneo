using System;

namespace Avalonia.FToolNeoV2.Models;

public class Constants
{
    /// <summary>
    /// Message that is sent to the process that represents a key being pressed down.
    /// </summary>
    public const UInt32 WM_KEYDOWN = 0x0100;

    /// <summary>
    /// Name of the file 
    /// </summary>
    public const string PERSISTENT_FILE_NAME = "ftool-settings.json";
}