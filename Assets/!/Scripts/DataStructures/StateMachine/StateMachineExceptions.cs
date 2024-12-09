using System;

namespace StateMachine
{
    public class StateTransitionFailedException<C> : Exception
    {
        public StateTransitionFailedException(C command) : base(command.ToString())
        {

        }
    }
}
