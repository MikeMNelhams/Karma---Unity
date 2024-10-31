using UnityEngine;
using KarmaLogic.Controller;
using UnityEngine.UI;
using DataStructures;
using System.Threading.Tasks;

public interface ICharacterProperties
{
    public int Index { get; set; }
    public Controller Controller { get; set; }

    public Button PickupPlayPileButton { get; }

    public Button ConfirmSelectionButton { get; }

    public Button ClearSelectionButton { get; }

    public ListWithConstantContainsCheck<SelectableCard> SelectableCardObjects { get; }

    public void TryToggleCardSelect(SelectableCard cardObject);

    public void EnableCamera();

    public void DisableCamera();

    public void HideUI();

    public Task TriggerVoteForPlayer(int targetIndex);

    public Task EnterPickingActionUpdateUI();

    public Task ExitPickingActionUpdateUI();

    public Task EnterVotingForWinner();

    public Task ExitVotingForWinner();

    public Task EnterCardGiveAwaySelection();

    public Task ExitCardGiveAwaySelection();

    public Task EnterCardGiveAwayPlayerIndexSelection();

    public Task ExitCardGiveAwayPlayerIndexSelection();

    public Task EnterPlayPileGiveAwayPlayerIndexSelection();

    public Task ExitPlayPileGiveAwayPlayerIndexSelection();

    public Task TriggerTargetReceivePickedUpCard(int targetIndex);
}
