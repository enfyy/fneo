using Avalonia.FToolNeoV2.Enums;
using Avalonia.Input;

namespace Avalonia.FToolNeoV2.Models;

public struct HotkeyCombination
{
    /// <summary>
    /// Is this combination enabled?
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// The key.
    /// </summary>
    public Keys Key { get; set; }
    
    /// <summary>
    /// The modifer keys.
    /// </summary>
    public KeyModifiers Modifiers { get; set; }
}