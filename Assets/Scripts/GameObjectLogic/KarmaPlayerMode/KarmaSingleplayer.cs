using KarmaLogic.BasicBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaPlayMode
{
    public class KarmaSingleplayer : KarmaPlayMode
    {
        
        public KarmaSingleplayer(KarmaPlayerStartInfo[] playersStartInfo)
        {

        }

        public override int NumberOfActivePlayers { get => 1; }
        

        public override void SetupPlayerActionStateForBasicStart()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupPlayerActionStatesForVotingForWinner()
        {
            throw new System.NotImplementedException();
        }
    }
}
