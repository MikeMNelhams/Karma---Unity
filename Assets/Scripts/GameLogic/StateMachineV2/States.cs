using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StateMachineV2
{
    public enum State : byte
    {
        Null,
        WaitingForTurn,
        PickingAction,
        SelectingCardGiveAwayIndex,
        SelectingCardGiveAwayPlayerIndex,
        SelectingPlayPileGiveAwayPlayerIndex,
        VotingForWinner,
        Mulligan, 
        PotentialWinner
    }
}

