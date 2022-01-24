using System.Reactive;
using Avalonia.FToolNeoV2.Extensions;
using ReactiveUI;

namespace Avalonia.FToolNeoV2.ViewModels;

public class CompactDockButtonViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> OnButtonToggled { get; init; }

    public bool ButtonIsToggled
    {
        get => _isButtonToggled;
        set => this.RaiseAndSetIfChanged(ref _isButtonToggled, value);
    }

    public string IndexText
    {
        get => _indexText;
        set => this.RaiseAndSetIfChanged(ref _indexText, value);
    }

    public string NameText
    {
        get => _nameText;
        set => this.RaiseAndSetIfChanged(ref _nameText, value);
    }

    public SpamSlotViewModel SpamSlotViewModel { get; }

    private bool _isButtonToggled;

    private string _indexText = "";

    private string _nameText = "";
    

    public CompactDockButtonViewModel(SpamSlotViewModel spamSlotViewModel)
    {
        SpamSlotViewModel = spamSlotViewModel;

        IndexText = SpamSlotViewModel.Index;

        if (!SpamSlotViewModel.SpamSlot.CharacterName.IsNullOrWhiteSpace())
            NameText = SpamSlotViewModel.SpamSlot.CharacterName!;

        ButtonIsToggled = SpamSlotViewModel.IsSpamming;

        var buttonEnabled = this.WhenAny(x => x.SpamSlotViewModel.StartButtonIsEnabled, x => x.Value);

        OnButtonToggled = ReactiveCommand.Create(() => { SpamSlotViewModel.ToggleSpammer(); }, buttonEnabled);
    }
    
    
}