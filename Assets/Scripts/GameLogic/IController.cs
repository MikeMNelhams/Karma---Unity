using DataStructures;
using Karma.Board;
using Karma.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace Karma
{
    namespace Controller
    {
        public interface IController
        {
            public bool IsAwaitingInput { get; set; }
            public FrozenMultiSet<CardValue> SelectedCardValues { get; set; }
            public BoardPlayerAction SelectedAction { get; set; }
            public int GiveAwayCardIndex(IBoard board, HashSet<int> excludedCardsIndices);
            public int GiveAwayPlayerIndex(IBoard board, HashSet<int> excludedPlayerIndices);
            public int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices);
            public bool WantsToMulligan(IBoard board);
            public int MulliganHandIndex(IBoard board);
            public int MulliganKarmaUpIndex(IBoard board);
            public BoardTurnOrder ChooseStartDirection(IBoard board);
            public BoardPlayerAction SelectAction(IBoard board);
            public FrozenMultiSet<CardValue> SelectCardsToPlay(IBoard board);
            public int VoteForWinner(IBoard board, HashSet<int> excludedPlayerIndices);
        }

        public abstract class ControllerState
        {
            public abstract void OnEnter(IBoard board);
            public abstract void OnExit(IBoard board);
            public GameObject CurrentPlayer(IBoard board) { return KarmaGameManager.Instance.Players[board.CurrentPlayerIndex]; }
        }

        public class WaitForTurnState : ControllerState
        {
            public override void OnEnter(IBoard board)
            {
                throw new System.NotImplementedException();
            }

            public override void OnExit(IBoard board)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
