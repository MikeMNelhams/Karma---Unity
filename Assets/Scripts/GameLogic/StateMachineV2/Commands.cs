using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineV2
{
    public enum Command : byte
    {
        TurnStarted,
        TurnEnded,
        Burned,
        Mulligan,
        GameEnded,
        CardGiveAwayComboPlayed,
        CardGiveAwayIndexSelected,
        CardGiveAwayUnfinished,
        PlayPileGiveAwayComboPlayed, 
        VotingStarted, 
        HasNoCardsLeft,
        GotJokered
    }
}
