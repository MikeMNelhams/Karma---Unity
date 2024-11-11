using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StateMachines
{
    namespace CharacterStateMachines
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
            PotentialWinner,
            GameOver
        }
    }
}

