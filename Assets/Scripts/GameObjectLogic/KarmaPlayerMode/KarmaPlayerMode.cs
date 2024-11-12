using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.BasicBoard;

namespace KarmaPlayMode
{
    [System.Serializable]
    public abstract class KarmaPlayMode
    {
        public abstract void SetupPlayerActionStateForBasicStart();

        public abstract void SetupPlayerActionStatesForVotingForWinner();


        public abstract int NumberOfActivePlayers { get; }

    }
}
