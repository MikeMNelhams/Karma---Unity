using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic
{
    namespace Bots
    {
        public interface IBot
        {
            public string Name { get; }
            public int DelaySeconds { get; }
            public bool IsReady(IBoard board);
            public BoardPlayerAction SelectAction(IBoard board);
            public FrozenMultiSet<CardValue> ComboToPlay(IBoard board);
            public int CardGiveAwayIndex(IBoard board);
            public int CardPlayerGiveAwayIndex(IBoard board, HashSet<int> excludedPlayerIndices);
            public int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices);
            public bool WantsToMulligan(IBoard board);
            public int MulliganHandIndex(IBoard board);
            public int MulliganKarmaUpIndex(IBoard board);
            public BoardTurnOrder PreferredStartDirection(IBoard board);
            public int VoteForWinnerIndex(IBoard board, HashSet<int> excludedPlayerIndices);
        }
    }
}