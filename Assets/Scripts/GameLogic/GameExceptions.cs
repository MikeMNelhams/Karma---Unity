using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karma
{
    namespace Game
    {
        class GameWonException : Exception
        {
            public GameWonException(string message) : base(message) { }
            public GameWonException(Dictionary<int, int> gameRanks)
            {
                string message = "Overall Rankings: " + gameRanks;
                throw new GameWonException(message);
            }
        }

        class GameTurnLimitExceededException : Exception
        {
            public GameTurnLimitExceededException(string message) : base(message) { }
            public GameTurnLimitExceededException(Dictionary<int, int> gameRanks, int turnLimit)
            {
                string message = "Max turn limit of " + turnLimit + " has been hit!\nGame rankings: " + gameRanks;
                throw new GameTurnLimitExceededException(message);
            }
        }
    }
}