using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachines
{
    public abstract class StateMachine<S, C>
    {
        public delegate Task OnEnterListener(S currentState, S nextState);
        public delegate Task OnExitListener(S currentState, S nextState);

        protected Dictionary<StateTransition<S, C>, StateTransitionResult<S>> Transitions { get; set; }
        public S CurrentState { get; protected set; }

        public virtual bool IsValidCommand(C command)
        {
            StateTransition<S, C> transition = new(CurrentState, command);
            return Transitions.ContainsKey(transition);
        }

        public virtual StateTransitionResult<S> GetNext(C command)
        {
            StateTransition<S, C> transition = new(CurrentState, command);
            if (!Transitions.TryGetValue(transition, out StateTransitionResult<S> stateTransitionResult))
            {
                stateTransitionResult = StateTransitionResult<S>.Failed;
            }
            return stateTransitionResult;
        }

        public virtual async ValueTask<S> MoveNext(C command)
        {
            UnityEngine.Debug.Log("MoveNext: " + CurrentState + " + " + command);
            S startState = CurrentState;
            StateTransitionResult<S> nextStateResult = GetNext(command);
            if (nextStateResult.HasFailed) { return startState; }
            CurrentState = nextStateResult.State;
            await nextStateResult.InvokeTransitionActions();
            return CurrentState;
        }

        public virtual async ValueTask<S> MoveNextStrict(C command)
        {
            StateTransitionResult<S> nextStateResult = GetNext(command);
            if (nextStateResult.HasFailed) { throw new StateTransitionFailedException<C>(command); }
            CurrentState = nextStateResult.State;
            await nextStateResult.InvokeTransitionActions();
            return CurrentState;
        }
    }
}
