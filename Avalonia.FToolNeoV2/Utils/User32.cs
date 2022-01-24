using System;
using System.Runtime.InteropServices;

namespace Avalonia.FToolNeoV2.Utils;

public class User32
{
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    // This helper static method is required because the 32-bit version of user32.dll does not contain this API
    // (on any versions of Windows), so linking the method will fail at run-time. The bridge dispatches the request
    // to the correct function (GetWindowLong in 32-bit mode and GetWindowLongPtr in 64-bit mode)
    public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
    {
        return IntPtr.Size == 8 
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) 
            : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    [DllImport("user32.dll", EntryPoint="SetWindowLong")]
    private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint="SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
}
