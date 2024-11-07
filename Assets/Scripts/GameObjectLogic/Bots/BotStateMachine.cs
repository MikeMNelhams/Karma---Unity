using System.Collections.Generic;

namespace StateMachineV2
{
    public class BotStateMachine : StateMachine
    {
        public BotStateMachine(PlayerProperties playerProperties)
        {
            Transitions = new Dictionary<StateTransition, StateTransitionResult> ();
        }
    }
}

