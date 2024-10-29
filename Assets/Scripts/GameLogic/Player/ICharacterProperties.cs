using UnityEngine;
using KarmaLogic.Controller;
using UnityEngine.UI;
using DataStructures;

public interface ICharacterProperties
{
    public Controller Controller { get; set; }

    public Button PickupPlayPileButton { get; }

    public Button ConfirmSelectionButton { get; }

    public Button ClearSelectionButton { get; }

    public ListWithConstantContainsCheck<SelectableCard> SelectableCardObjects { get; }

    public void TryToggleCardSelect(SelectableCard cardObject);

    public void EnableCamera();

    public void DisableCamera();

    public void HideUI();

    public void EnterPickingActionUpdateUI();

    public void ExitPickingActionUpdateUI();

    public void EnterVotingForWinner();

    public void ExitVotingForWinner();

    public void EnterCardGiveAwaySelection();

    public void ExitCardGiveAwaySelection();

    public void EnterCardGiveAwayPlayerIndexSelection();

    public void ExitCardGiveAwayPlayerIndexSelection();

    public void EnterPlayPileGiveAwayPlayerIndexSelection();

    public void ExitPlayPileGiveAwayPlayerIndexSelection();
}
