using DataStructures;
using KarmaLogic.Controller;
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

    public void EnableCamera()
    {
        throw new System.NotImplementedException();
    }

    public void DisableCamera()
    {
        throw new System.NotImplementedException();
    }

    public void HideUI()
    {
        throw new System.NotImplementedException();
    }

    public void EnterPickingActionUpdateUI()
    {
        throw new System.NotImplementedException();
    }

    public void ExitPickingActionUpdateUI()
    {
        throw new System.NotImplementedException();
    }

    public void EnterVotingForWinner()
    {
        throw new System.NotImplementedException();
    }

    public void ExitVotingForWinner()
    {
        throw new System.NotImplementedException();
    }

    public void EnterCardGiveAwaySelection()
    {
        throw new System.NotImplementedException();
    }

    public void ExitCardGiveAwaySelection()
    {
        throw new System.NotImplementedException();
    }

    public void EnterCardGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public void ExitCardGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public void EnterPlayPileGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public void ExitPlayPileGiveAwayPlayerIndexSelection()
    {
        throw new System.NotImplementedException();
    }

    public void TryToggleCardSelect(SelectableCard cardObject)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerVoteForPlayer(int targetIndex)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerTargetReceivePickedUpCard(int targetIndex)
    {
        throw new System.NotImplementedException();
    }
}
