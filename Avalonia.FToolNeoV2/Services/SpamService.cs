using System.Diagnostics;
using System.Threading;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Utils;

namespace Avalonia.FToolNeoV2.Services;

/// <summary>
/// Service that spams its process with key press messages.
/// </summary>
public class SpamService
{
    /// <summary>
    /// Is the spammer currently actively spamming ?
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// The process that gets spammed.
    /// </summary>
    private readonly Process _process;

    /// <summary>
    /// The data of the spam slot.
    /// </summary>
    private readonly SpamSlot _spamSlot;

    /// <summary>
    /// The thread that the spammer is using.
    /// </summary>
    private Thread? _thread;


    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="process">The process that gets spammed.</param>
    /// <param name="spamSlot">The data of the spam slot.</param>
    public SpamService(Process process, SpamSlot spamSlot)
    {
        _process = process;
        _spamSlot = spamSlot;
    }

    /// <summary>
    /// The spam loop.
    /// </summary>
    private void Spam()
    {
        while (IsActive)
        {
            if (_spamSlot.BarKey != null)
                User32.PostMessage(_process.MainWindowHandle, Constants.WM_KEYDOWN, (int) _spamSlot.BarKey, 0);

            if (_spamSlot.FKey != null)
                User32.PostMessage(_process.MainWindowHandle, Constants.WM_KEYDOWN, (int) _spamSlot.FKey, 0);
            
            Thread.Sleep(_spamSlot.Delay);
        }
    }

    /// <summary>
    /// Stops the spam loop.
    /// </summary>
    public void Stop()
    {
        IsActive = false;
        _thread?.Join();
    }

    /// <summary>
    /// Is the service ready to start spamming?
    /// </summary>
    /// <returns>True when ready, else false.</returns>
    public bool IsReady() => !IsActive && (_spamSlot.BarKey != null || _spamSlot.FKey != null);

    /// <summary>
    /// Starts a new thread that spams.
    /// </summary>
    public void Start()
    {
        IsActive = true;
        _thread = new Thread(Spam);
        _thread.Start();
    }
}