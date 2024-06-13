using System.Collections;
using System.Collections.Generic;
using DataStructures;
using Karma.Cards;
using Karma.Players;
using Karma.Controller;
using System;

namespace Karma
{
    namespace Board
    {
        public enum BoardPlayOrder : byte
        {
            UP = 0,
            DOWN = 1
        }

        public enum BoardTurnOrder : short
        {
            LEFT = -1,
            RIGHT = 1
        }

        public abstract class BoardPlayerAction{
            public abstract bool IsValid(IBoard board);
            public abstract void Apply(IBoard board, IController controller);
            public abstract BoardPlayerAction Copy();
            public abstract string Name { get; }
            public override int GetHashCode() 
            {
                return Name.GetHashCode();
            }
        }

        public interface IBoard
        {
            public Player CurrentPlayer { get; }
            public void FlipTurnOrder();
            public void SetTurnOrder(BoardTurnOrder turnOrder);
            public void StepPlayerIndex(int numberOfRepeats);
            public void FlipPlayOrder();
            public void ResetPlayOrder();
            public void FlipHands();
            public void StartTurn();
            public void EndTurn();
            public bool PlayCards(CardsList cards, IController controller);
            public bool PlayCards(CardsList cards, IController controller, bool addToPlayPile);
            public void Burn(int jokerCount);

            public List<Player> Players { get; }
            public CardPile DrawPile { get;}
            public CardPile BurnPile { get; }
            public PlayCardPile PlayPile { get; }
            public BoardPlayOrder PlayOrder { get; }
            public BoardTurnOrder TurnOrder { get; }
            public bool HandsAreFlipped { get; set; }
            public int EffectMultiplier { get; set; }
            public int CurrentPlayerIndex { get; set; }
            public int NumberOfJokersInPlay { get; set; }
            public bool HasBurnedThisTurn { get; }
            public int TurnsPlayed { get; }
            public HashSet<FrozenMultiSet<CardValue>> CurrentLegalCombos { get; }
            public HashSet<BoardPlayerAction> CurrentLegalActions { get; }
            public int TotalJokers { get; }
            public HashSet<int> PotentialWinnerIndices { get; }
            public int NumberOfCombosPlayedThisTurn { get; }
            public int PlayerIndexWhoStartedTurn { get; }
            public List<CardCombo> ComboHistory { get; }

            public delegate void OnTurnEndEventHandler(IBoard board);
            public void RegisterOnTurnEndEvent(OnTurnEndEventHandler newEventHandler);
        }

        public class InvalidBoardPlayerActionException : Exception
        {
            public InvalidBoardPlayerActionException(string message) : base(message) { }

            public InvalidBoardPlayerActionException(BoardPlayerAction boardPlayerAction)
            {
                string message = "The action: \'" + boardPlayerAction.Name + "\' is invalid.";
                throw new InvalidBoardPlayerActionException(message);
            }
        }
    }
}