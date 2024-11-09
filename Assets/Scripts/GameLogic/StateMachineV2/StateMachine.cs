using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineV2
{
    public delegate Task OnEnterListener(State currentState, State nextState);
    public delegate Task OnExitListener(State currentState, State nextState);

    public abstract class StateMachine
    {
        protected Dictionary<StateTransition, StateTransitionResult> Transitions { get; set; }
        public State CurrentState { get; set; }

        public virtual bool IsValidCommand(Command command)
        {
            StateTransition transition = new(CurrentState, command);
            return Transitions.ContainsKey(transition);
        }

        public virtual StateTransitionResult GetNext(Command command)
        {
            StateTransition transition = new(CurrentState, command);
            if (!Transitions.TryGetValue(transition, out StateTransitionResult stateTransitionResult))
            {
                stateTransitionResult = StateTransitionResult.Failed;
            }
            return stateTransitionResult;
        }

        public virtual async ValueTask<State> MoveNext(Command command)
        {
            UnityEngine.Debug.Log("MoveNext: " + CurrentState + " + " + command);
            State startState = CurrentState;
            StateTransitionResult nextStateResult = GetNext(command);
            if (nextStateResult.HasFailed) { return startState; }
            CurrentState = nextStateResult.State;
            await nextStateResult.InvokeTransitionActions();
            return CurrentState;
        }

        public virtual async ValueTask<State> MoveNextStrict(Command command)
        {
            StateTransitionResult nextStateResult = GetNext(command);
            if (nextStateResult.HasFailed) { throw new StateTransitionFailedException(command); }
            CurrentState = nextStateResult.State;
            await nextStateResult.InvokeTransitionActions();
            return CurrentState;
        }
    }
}
