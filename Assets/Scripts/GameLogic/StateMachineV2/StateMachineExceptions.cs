using System.Collections;
using System.Collections.Generic;
using System;

namespace StateMachineV2
{
    public class StateTransitionFailedException : Exception
    {
        public StateTransitionFailedException() { }

        public StateTransitionFailedException(Command command) : base (command.ToString())
        {

        }
    }
}
