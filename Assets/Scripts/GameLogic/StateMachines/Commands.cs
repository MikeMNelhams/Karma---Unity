using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachines
{
    namespace CharacterStateMachines
    {
        public enum Command : byte
        {
            TurnStarted,
            TurnEnded,
            Mulligan,
            GameEnded,
            CardGiveAwayComboPlayed,
            CardGiveAwayIndexSelected,
            CardGiveAwayUnfinished,
            PlayPileGiveAwayComboPlayed,
            VotingStarted,
            HasNoCards,
            GotJokered
        }
    }
}
