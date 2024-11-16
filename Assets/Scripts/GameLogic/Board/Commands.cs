using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    namespace CharacterStateMachines
    {
        public enum Command : byte
        {
            TurnStarted,
            TurnEnded,
            MulliganStarted,
            MulliganEnded,
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
