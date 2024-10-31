using DataStructures;
using KarmaLogic.Controller;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DummyPlayerProperties : MonoBehaviour, ICharacterProperties
{
    public DummyPlayerProperties()
    {

    }

    public int Index { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public Controller Controller { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Button PickupPlayPileButton => throw new System.NotImplementedException();

    public Button ConfirmSelectionButton => throw new System.NotImplementedException();

    public Button ClearSelectionButton => throw new System.NotImplementedException();

    public ListWithConstantContainsCheck<SelectableCard> SelectableCardObjects => throw new System.NotImplementedException();

    public void DisableCamera()
    {
        throw new System.NotImplementedException();
    }

    public void EnableCamera()
    {
        throw new System.NotImplementedException();
    }

    public Task EnterCardGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public Task EnterCardGiveAwaySelection()
    {
        throw new System.NotImplementedException();
    }

    public Task EnterPickingActionUpdateUI()
    {
        throw new System.NotImplementedException();
    }

    public Task EnterPlayPileGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public Task EnterVotingForWinner()
    {
        throw new System.NotImplementedException();
    }

    public Task ExitCardGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public Task ExitCardGiveAwaySelection()
    {
        throw new System.NotImplementedException();
    }

    public Task ExitPickingActionUpdateUI()
    {
        throw new System.NotImplementedException();
    }

    public Task ExitPlayPileGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public Task ExitVotingForWinner()
    {
        throw new System.NotImplementedException();
    }

    public void HideUI()
    {
        throw new System.NotImplementedException();
    }

    public Task TriggerTargetReceivePickedUpCard(int targetIndex)
    {
        throw new System.NotImplementedException();
    }

    public Task TriggerVoteForPlayer(int targetIndex)
    {
        throw new System.NotImplementedException();
    }

    public void TryToggleCardSelect(SelectableCard cardObject)
    {
        throw new System.NotImplementedException();
    }
}
