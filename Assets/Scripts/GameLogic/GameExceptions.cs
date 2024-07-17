using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic
{
    namespace GameExceptions
    {
        public class GameWonException : Exception
        {
            public GameWonException(string message) : base(message) { }
            public GameWonException(Dictionary<int, int> gameRanks)
            {
                string message = "Overall Rankings: ";

                foreach (KeyValuePair<int, int> kvp in gameRanks)
                {
                    message += kvp.ToString() + ", ";
                }
                throw new GameWonException(message);
            }
        }

        public class GameTurnLimitExceededException : Exception
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