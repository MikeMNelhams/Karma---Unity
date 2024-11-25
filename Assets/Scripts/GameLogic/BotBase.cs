using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using System.Collections.Generic;

namespace KarmaLogic.Bots
{
    public abstract class BotBase
    {
        public BotBase(string name, float delay)
        {
            Name = name;
            DelaySeconds = delay;
        }

        public string Name { get; protected set; }
        public float DelaySeconds { get; protected set; }
        public abstract BoardPlayerAction SelectAction(IBoard board);
        public abstract FrozenMultiSet<CardValue> ComboToPlay(IBoard board);
        public abstract int CardGiveAwayIndex(IBoard board);
        public abstract int CardPlayerGiveAwayIndex(IBoard board, HashSet<int> excludedPlayerIndices);
        public abstract int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices);
        public abstract bool WantsToMulligan(IBoard board);
        public abstract int MulliganHandIndex(IBoard board);
        public abstract int MulliganKarmaUpIndex(IBoard board);
        public abstract BoardTurnOrder PreferredStartDirection(IBoard board);
        public abstract int VoteForWinnerIndex(IBoard board, HashSet<int> excludedPlayerIndices);
    }
}