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
            public Dictionary<int, int> GameRanks { get; protected set; }
                
            protected GameWonException(string message) : base(message) { }
            public GameWonException(Dictionary<int, int> gameRanks)
            {
                GameRanks = gameRanks;
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
            public Dictionary<int, int> GameRanks { get; protected set; }
            public int TurnLimit { get; protected set; }

            protected GameTurnLimitExceededException(string message) : base(message) { }
            public GameTurnLimitExceededException(Dictionary<int, int> gameRanks, int turnLimit)
            {
                GameRanks = gameRanks;
                TurnLimit = turnLimit;
                string message = "Max turn limit of " + turnLimit + " has been hit!\nGame rankings: ";

                foreach (KeyValuePair<int, int> kvp in gameRanks)
                {
                    message += kvp.ToString() + ", ";
                }

                throw new GameTurnLimitExceededException(message);
            }
        }
    }
}