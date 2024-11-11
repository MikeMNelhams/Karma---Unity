using System;

namespace StateMachines
{
    public class StateTransition<S, C> : IEquatable<StateTransition<S, C>>
    {
        readonly S _currentState;
        readonly C _command;

        public StateTransition(S currentState, C command)
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
            return obj != null && obj is StateTransition<S, C> other && Equals(other as StateTransition<S, C>);
        }

        public override string ToString()
        {
            return "StateTransition[" + _currentState + ", " + _command + "]";
        }

        public bool Equals(StateTransition<S, C> other)
        {
            if (this._currentState is not IEquatable<S> && this._currentState is not Enum) 
            { 
                throw new InvalidCastException("Invalid implementation of StateMachine with generic: " + typeof(S)); 
            }
            if (this._command is not IEquatable<C> && this._currentState is not Enum) 
            { 
                throw new InvalidCastException("Invalid implementation of StateMachine with generic: " + typeof(C)); 
            }
            
            return this._currentState.Equals(other._currentState) && this._command.Equals(other._command);
        }
    }

}
