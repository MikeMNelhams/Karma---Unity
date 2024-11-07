using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StateMachineV2
{
    public class StateTransition
    {
        readonly State _currentState;
        readonly Command _command;

        public StateTransition(State currentState, Command command)
        {
            _currentState = currentState;
            _command = command;
        }

        public override int GetHashCode()
        {
            return 17 + 31 * _currentState.GetHashCode() + 31 * _command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is StateTransition other && this._currentState == other._currentState && this._command == other._command;
        }

        public override string ToString()
        {
            return "StateTransition[" + _currentState + ", " + _command + "]";
        }
    }
}
