using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerProperties : MonoBehaviour
{
    public abstract void EnterWaitingForTurn();
    public abstract void ExitWaitingForTurn();

    public abstract void EnterPickingAction();
    public abstract void ExitPickingAction();

    public abstract void EnterVotingForWinner();
    public abstract void ExitVotingForWinner();

    public abstract void EnterCardGiveAwaySelection();
    public abstract void ExitCardGiveAwaySelection();

    public abstract void EnterCardGiveAwayPlayerIndexSelection();
    public abstract void ExitCardGiveAwayPlayerIndexSelection();

    public abstract void EnterPlayPileGiveAwayPlayerIndexSelection();
    public abstract void ExitPlayPileGiveAwayPlayerIndexSelection();
}
