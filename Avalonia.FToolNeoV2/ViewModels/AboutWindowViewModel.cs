using System.Diagnostics;
using System.Reactive;
using Avalonia.FToolNeoV2.Models;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class AboutWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> OnCloseButtonClicked { get; init; }

    public ReactiveCommand<Unit, Unit> OnGithubButtonClicked { get; init; }

    public ReactiveCommand<Unit, Unit> OnKofiButtonClicked { get; init; }

    public AboutWindowViewModel()
    {
        OnCloseButtonClicked = ReactiveCommand.Create(() => { });

        OnGithubButtonClicked = ReactiveCommand.Create(() => { OpenLinkInBrowser(Constants.GITHUB_LINK); });

        OnKofiButtonClicked = ReactiveCommand.Create(() => { OpenLinkInBrowser(Constants.KOFI_LINK); });
    }

    /// <summary>
    /// Opens a browser with the url
    /// </summary>
    /// <param name="url">The url.</param>
    private static void OpenLinkInBrowser(string url)
    {
        var info = new ProcessStartInfo
        {
            FileName = url,
            CreateNoWindow = true,
            UseShellExecute = true
        };
        Process.Start(info);
    }
}