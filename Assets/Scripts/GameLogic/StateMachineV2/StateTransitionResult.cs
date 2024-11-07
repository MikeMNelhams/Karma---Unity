using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineV2
{
    public delegate Task StateTransitionListener();

    public class StateTransitionResult
    {
        public State State { get; protected set; }
        public bool HasFailed { get; private set; }

        protected List<StateTransitionListener> _transitionListeners;

        public StateTransitionResult(State resultState, IEnumerable<StateTransitionListener> transitionActions)
        {
            State = resultState;
            _transitionListeners = new List<StateTransitionListener>(transitionActions);
            HasFailed = false;
        }

        public StateTransitionResult(State resultState)
        {
            State = resultState;
            _transitionListeners = new List<StateTransitionListener>();
            HasFailed = false;
        }

        public async Task InvokeTransitionActions()
        {
            foreach (StateTransitionListener listener in _transitionListeners)
            {
                await listener?.Invoke();
            }
        }

        public static StateTransitionResult Failed
        {
            get
            {
                StateTransitionResult failedResult = new(State.Null)
                {
                    HasFailed = true
                };
                return failedResult;
            }
        }
    }
}

