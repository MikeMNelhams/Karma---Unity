using DataStructures;
using Karma.Board;
using Karma.Cards;
using System;
using UnityEngine;

namespace Karma
{
    namespace Controller
    {
        public class IController
        {
            public ControllerState State { get; protected set; }
            public virtual void SetState(ControllerState newState)
            {
                State?.OnExit();
                newState.OnEnter();
                State = newState;
            } 
        }

        public abstract class ControllerState : IEquatable<ControllerState>
        {
            protected IBoard _board;
            public BasePlayerProperties _playerProperties;

            protected ControllerState(IBoard board, BasePlayerProperties playerProperties)
            {
                _board = board;
                _playerProperties = playerProperties;
            }

            public abstract void OnEnter();
            public abstract void OnExit();
            public abstract override int GetHashCode();

            public bool Equals(ControllerState other)
            {
                if (ReferenceEquals(this, other)) { return true; }
                if (GetType() == other.GetType()) { return true; }
                return false;
            }
        }

        public class WaitForTurn : ControllerState
        {
            public WaitForTurn(IBoard board, BasePlayerProperties playerProperties) : base(board, playerProperties) {}
            public override void OnEnter()
            {
                _playerProperties.EnterWaitingForTurn();
            }

            public override void OnExit()
            {
                _playerProperties.ExitWaitingForTurn();
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class PickingAction : ControllerState
        {
            public PickingAction(IBoard board, BasePlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                string currentLegalCombos = "Currently legal combos ON \'"; ;

                Card topCard = _board.PlayPile.VisibleTopCard;
                if (topCard is not null)
                {
                    currentLegalCombos += _board.PlayPile.VisibleTopCard + "\' ";
                }
                else
                {
                    currentLegalCombos += "Nothing! ";
                }

                foreach (FrozenMultiSet<CardValue> combo in _board.CurrentLegalCombos)
                {
                    currentLegalCombos += combo + ", ";
                }

                currentLegalCombos += ": HashSet[";

                Debug.Log(currentLegalCombos + "]");
                Debug.Log("Starting picking actions for player: " + _board.CurrentPlayerIndex);
                Debug.Log("Current selectable cards BOARD: " + _board.CurrentPlayer.PlayableCards);
                _playerProperties.EnterPickingAction();
            }

            public override void OnExit()
            {
                _playerProperties.ExitPickingAction();
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        public class VotingForWinner : ControllerState
        {
            public VotingForWinner(IBoard board, BasePlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterVotingForWinner();
            }

            public override void OnExit()
            {
                _playerProperties.ExitVotingForWinner();
            }

            public override int GetHashCode()
            {
                return 2;
            }
        }

        public class SelectingCardGiveAwaySelectionIndex : ControllerState
        {
            public SelectingCardGiveAwaySelectionIndex(IBoard board, BasePlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterCardGiveAwaySelection();
            }

            public override void OnExit()
            {
                _playerProperties.ExitCardGiveAwaySelection();
            }

            public override int GetHashCode()
            {
                return 3;
            }
        }

        public class SelectingCardGiveAwayPlayerIndex : ControllerState
        {
            public SelectingCardGiveAwayPlayerIndex(IBoard board, BasePlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterCardGiveAwayPlayerIndexSelection();
            }

            public override void OnExit()
            {
                _playerProperties.ExitCardGiveAwayPlayerIndexSelection();
            }

            public override int GetHashCode()
            {
                return 4;
            }
        }

        public class SelectingPlayPileGiveAwayPlayerIndex : ControllerState
        {
            public SelectingPlayPileGiveAwayPlayerIndex(IBoard board, BasePlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterPlayPileGiveAwayPlayerIndexSelection();
            }

            public override void OnExit()
            {
                _playerProperties.ExitPlayPileGiveAwayPlayerIndexSelection();
            }

            public override int GetHashCode()
            {
                return 5;
            }
        }
    }
}
