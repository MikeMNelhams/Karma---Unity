using DataStructures;
using Karma.Board;
using Karma.Cards;
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

        public abstract class ControllerState
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
        }

        public class PickingAction : ControllerState
        {
            public PickingAction(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _board.StartTurn();
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
        }

        public class SelectingCardGiveAwayCardIndex : ControllerState
        {
            public SelectingCardGiveAwayCardIndex(IBoard board, PlayerProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                throw new System.NotImplementedException();
            }

            public override void OnExit()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
