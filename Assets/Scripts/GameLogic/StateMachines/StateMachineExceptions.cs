using System.Collections;
using System.Collections.Generic;
using System;

namespace StateMachines
{
    public class StateTransitionFailedException<C> : Exception
    {
        public StateTransitionFailedException() { }

        public StateTransitionFailedException(C command) : base(command.ToString())
        {

        }
    }
}
