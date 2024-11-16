using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachine
{
    public delegate Task StateTransitionListener();

    public class StateTransitionResult<S>
    {
        public S State { get; protected set; }
        public bool HasFailed { get; private set; }

        protected List<StateTransitionListener> _transitionListeners;

        public StateTransitionResult(S resultState, IEnumerable<StateTransitionListener> transitionActions)
        {
            State = resultState;
            _transitionListeners = new List<StateTransitionListener>(transitionActions);
            HasFailed = false;
        }

        public StateTransitionResult(S resultState)
        {
            State = resultState;
            _transitionListeners = new List<StateTransitionListener>();
            HasFailed = false;
        }

        protected StateTransitionResult()
        {
            HasFailed = true;
        }

        public async Task InvokeTransitionActions()
        {
            foreach (StateTransitionListener listener in _transitionListeners)
            {
                await listener?.Invoke();
            }
        }

        public static StateTransitionResult<S> Failed
        {
            get
            {
                StateTransitionResult<S> failedResult = new();
                return failedResult;
            }
        }
    }
}

