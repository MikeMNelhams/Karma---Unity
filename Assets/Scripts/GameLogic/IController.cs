using DataStructures;
using Karma.Board;
using Karma.Cards;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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
            public PlayerProperties _playerProperties;
            protected ControllerState(IBoard board, PlayerProperties playerProperties)
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
            public WaitForTurn(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) {}
            public override void OnEnter()
            {
                _playerProperties.DisableCamera();
                _playerProperties.HideUI();
            }

            public override void OnExit()
            {
                _playerProperties.EnableCamera();  
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class PickingAction : ControllerState
        {
            public PickingAction(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) { }

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
                Debug.Log("Current playable cards: " + _board.CurrentPlayer.PlayableCards);
                _playerProperties.EnterPickingActionMode();
            }

            public override void OnExit()
            {
                _playerProperties.ExitPickingActionMode();
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        public class VotingForWinner : ControllerState
        {
            public VotingForWinner(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterVotingForWinnerMode();
            }

            public override void OnExit()
            {
                _playerProperties.ExitVotingForWinnerMode();
            }

            public override int GetHashCode()
            {
                return 2;
            }
        }

        public class SelectingCardGiveAwaySelectionIndex : ControllerState
        {
            public SelectingCardGiveAwaySelectionIndex(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterCardGiveAwaySelectionMode();
            }

            public override void OnExit()
            {
                _playerProperties.ExitCardGiveAwaySelectionMode();
            }

            public override int GetHashCode()
            {
                return 3;
            }
        }

        public class SelectingCardGiveAwayPlayerIndex : ControllerState
        {
            public SelectingCardGiveAwayPlayerIndex(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterCardGiveAwayPlayerSelectionMode();
            }

            public override void OnExit()
            {
                _playerProperties.ExitCardGiveAwayPlayerSelectionMode();
            }

            public override int GetHashCode()
            {
                return 4;
            }
        }
    }
}
