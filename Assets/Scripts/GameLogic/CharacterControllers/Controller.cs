using KarmaLogic.Board;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KarmaLogic
{
    namespace Controller
    {
        public abstract class Controller
        {
            public ControllerState State { get; protected set; }

            public delegate void OnFinishStateTransitionListener();

            protected event OnFinishStateTransitionListener OnFinishStateTransition;
            protected readonly List<OnFinishStateTransitionListener> _onFinishStateTransitionListeners = new();

            public virtual void SetState(ControllerState newState)
            {
                State?.OnExit();
                State = newState;
                newState.OnEnter();
                //UnityEngine.Debug.Log("number of listeners: " + _onFinishStateTransitionListeners.Count);
                //OnFinishStateTransition?.Invoke();
                //UnregisterOnFinishTransitionListeners();
            }

            public abstract void EnterWaitingForTurn(IBoard board, ICharacterProperties characterProperties);
            public abstract void ExitWaitingForTurn(IBoard board, ICharacterProperties characterProperties);

            public abstract void EnterPickingAction(IBoard board, ICharacterProperties characterProperties);
            public abstract void ExitPickingAction(IBoard board, ICharacterProperties characterProperties);

            public abstract void EnterVotingForWinner(IBoard board, ICharacterProperties characterProperties);
            public abstract void ExitVotingForWinner(IBoard board, ICharacterProperties characterProperties);

            public abstract void EnterCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties);
            public abstract void ExitCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties);

            public abstract void EnterCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);
            public abstract void ExitCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);

            public abstract void EnterPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);
            public abstract void ExitPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);

            public virtual void RegisterOnFinishTransitionListener(OnFinishStateTransitionListener listener)
            {
                OnFinishStateTransition += listener;
            }

            void UnregisterOnFinishTransitionListeners()
            {
                foreach (OnFinishStateTransitionListener listener in _onFinishStateTransitionListeners)
                {
                    OnFinishStateTransition -= listener;
                }

                _onFinishStateTransitionListeners.Clear();
            }
        }

        public abstract class ControllerState : IEquatable<ControllerState>
        {
            protected IBoard _board;
            protected ICharacterProperties _characterProperties;

            protected ControllerState(IBoard board, ICharacterProperties characterProperties)
            {
                _board = board;
                _characterProperties = characterProperties;
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

            public override string ToString()
            {
                return "State ID: " + GetHashCode();
            }
        }
    }
}
