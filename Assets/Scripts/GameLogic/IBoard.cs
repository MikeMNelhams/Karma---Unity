using System.Collections;
using System.Collections.Generic;
using DataStructures;
using Karma.Cards;
using Karma.Players;
using Karma.Controller;
using System;
using Karma.Board.BoardEvents;

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

        public abstract class BoardPlayerAction : IEquatable<BoardPlayerAction>
        {
            public abstract bool IsValid(IBoard board);
            public abstract void Apply(IBoard board, IController controller, CardsList selectedCards);
            public abstract BoardPlayerAction Copy();
            public abstract string Name { get; }
            public override int GetHashCode() 
            { 
                return Name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null) { return false; }
                if (ReferenceEquals(this, obj)) { return true; }
                if (!(obj is BoardPlayerAction)) { return false; }
                return Equals((BoardPlayerAction)obj);
            }

            public bool Equals(BoardPlayerAction other)
            {
                return Name == other.Name;
            }

            public static bool operator ==(BoardPlayerAction x, BoardPlayerAction y) => x.Equals(y);
            public static bool operator !=(BoardPlayerAction x, BoardPlayerAction y) => !x.Equals(y);

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
            public void RotateHands(int numberOfRotations, Deque<Hand> hands);
            public void StartGivingAwayCards(int numberOfCards);
            public void StartGivingAwayPlayPile(int giverIndex);
            public void StartTurn();
            public void EndTurn();
            public bool PlayCards(CardsList cards, IController controller);
            public bool PlayCards(CardsList cards, IController controller, bool addToPlayPile);
            public void Burn(int jokerCount);

            public List<Player> Players { get; }
            public CardPile DrawPile { get; }
            public CardPile BurnPile { get; }
            public PlayCardPile PlayPile { get; }
            public BoardEventSystem BoardEventSystem {get; }
            public BoardPlayOrder PlayOrder { get; }
            public BoardTurnOrder TurnOrder { get; }
            public bool HandsAreFlipped { get; set; }
            public int EffectMultiplier { get; set; }
            public int CurrentPlayerIndex { get; set; }
            public int NumberOfJokersInPlay { get; set; }
            public bool HasBurnedThisTurn { get; }
            public int TurnsPlayed { get; }
            public int NumberOfCardsDrawnThisTurn { get; }
            public HashSet<FrozenMultiSet<CardValue>> CurrentLegalCombos { get; }
            public HashSet<BoardPlayerAction> CurrentLegalActions { get; }
            public int TotalJokers { get; }
            public HashSet<int> PotentialWinnerIndices { get; }
            public int NumberOfCombosPlayedThisTurn { get; }
            public int PlayerIndexWhoStartedTurn { get; }
            public List<CardCombo> ComboHistory { get; }
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