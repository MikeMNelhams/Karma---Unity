using KarmaLogic.Board;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KarmaLogic
{
    namespace Controller
    {
        public abstract class Controller
        {
            public ControllerState State { get; protected set; }

            public delegate void OnFinishStateTransitionListener();

            protected readonly Queue<OnFinishStateTransitionListener> _onFinishStateTransitionListeners = new();

            // TODO MAKE ASYNC! await on exit, await on enter, remove the listeners callback!!
            public virtual async Task SetState(ControllerState newState)
            {
                await State?.OnExit();
                State = newState;
                await newState.OnEnter();
                if (_onFinishStateTransitionListeners == null) { throw new NullReferenceException("On Finish State Transition failed. Null ref"); }
                TriggerFinishStateTransitionListeners();
            }

            public abstract Task EnterWaitingForTurn(IBoard board, ICharacterProperties characterProperties);
            public abstract Task ExitWaitingForTurn(IBoard board, ICharacterProperties characterProperties);

            public abstract Task EnterPickingAction(IBoard board, ICharacterProperties characterProperties);
            public abstract Task ExitPickingAction(IBoard board, ICharacterProperties characterProperties);

            public abstract Task EnterVotingForWinner(IBoard board, ICharacterProperties characterProperties);
            public abstract Task ExitVotingForWinner(IBoard board, ICharacterProperties characterProperties);

            public abstract Task EnterCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties);
            public abstract Task ExitCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties);

            public abstract Task EnterCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);
            public abstract Task ExitCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);

            public abstract Task EnterPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);
            public abstract Task ExitPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties);

            public void RegisterOnFinishTransitionListener(OnFinishStateTransitionListener listener)
            {
                _onFinishStateTransitionListeners.Enqueue(listener);
            }

            void TriggerFinishStateTransitionListeners()
            {
                while (_onFinishStateTransitionListeners.Count > 0)
                {
                    OnFinishStateTransitionListener listener = _onFinishStateTransitionListeners.Dequeue() ?? throw new NullReferenceException("NULL LISTENER!!");
                    listener.Invoke();
                }
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

            public abstract Task OnEnter();
            public abstract Task OnExit();
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
