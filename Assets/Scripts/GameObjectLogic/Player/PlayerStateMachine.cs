using KarmaLogic.Board;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachine
{
    namespace CharacterStateMachines
    {
        public class PlayerStateMachine : StateMachine<State, Command>
        {
            protected PlayerHandler _playerProperties;

            public PlayerStateMachine(PlayerHandler playerProperties)
            {
                _playerProperties = playerProperties;
                Transitions = new Dictionary<StateTransition<State, Command>, StateTransitionResult<State>>
            {
                {
                    new StateTransition<State, Command>(State.Null, Command.MulliganStarted),
                    new StateTransitionResult<State>(State.Mulligan, new List<StateTransitionListener>{ playerProperties.DisconnectCamera, playerProperties.EnterMulligan })
                },
                {
                    new StateTransition<State, Command>(State.Null, Command.TurnStarted),
                    new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ playerProperties.EnterPickingActionUpdateUI, playerProperties.EnableCamera })
                },
                {
                    new StateTransition<State, Command>(State.Null, Command.TurnEnded),
                    new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener> { playerProperties.DisconnectCamera })
                },
                {
                    new StateTransition<State, Command>(State.Null, Command.VotingStarted),
                    new StateTransitionResult<State>(State.VotingForWinner, new List<StateTransitionListener> { playerProperties.EnableCamera, playerProperties.EnterVotingForWinner})
                },
                {
                    new StateTransition<State, Command>(State.Null, Command.HasNoCards),
                    new StateTransitionResult<State>(State.PotentialWinner, new List<StateTransitionListener> { playerProperties.DisconnectCamera })
                },
                {
                    new StateTransition<State, Command>(State.Mulligan, Command.TurnStarted),
                    new StateTransitionResult<State>(State.Mulligan, new List<StateTransitionListener> { playerProperties.EnableCamera })
                },
                {
                    new StateTransition<State, Command>(State.Mulligan, Command.TurnEnded),
                    new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener>{ playerProperties.DisconnectCamera, playerProperties.HideUI })
                },
                {
                    new StateTransition<State, Command>(State.Mulligan, Command.MulliganEnded),
                    new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ playerProperties.ExitMulligan, playerProperties.EnterPickingActionUpdateUI })
                },
                {
                    new StateTransition<State, Command>(State.PickingAction, Command.PlayPileGiveAwayComboPlayed),
                    new StateTransitionResult<State>(State.SelectingPlayPileGiveAwayPlayerIndex, new List<StateTransitionListener>{ playerProperties.EnterPlayPileGiveAwayPlayerIndexSelection })
                },
                {
                    new StateTransition<State, Command>(State.PickingAction, Command.CardGiveAwayComboPlayed),
                    new StateTransitionResult<State>(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { playerProperties.EnterCardGiveAwaySelection })
                },
                {
                    new StateTransition<State, Command>(State.PickingAction, Command.TurnEnded),
                    new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener>{ playerProperties.ExitPickingActionUpdateUI, playerProperties.DisconnectCamera})
                },
                {
                    new StateTransition<State, Command>(State.PickingAction, Command.HasNoCards),
                    new StateTransitionResult<State>(State.PotentialWinner, new List<StateTransitionListener> {playerProperties.ExitPickingActionUpdateUI, playerProperties.DisconnectCamera})
                },
                {
                    new StateTransition<State, Command>(State.SelectingCardGiveAwayIndex, Command.CardGiveAwayIndexSelected),
                    new StateTransitionResult<State>(State.SelectingCardGiveAwayPlayerIndex, new List<StateTransitionListener>{ playerProperties.EnterCardGiveAwayPlayerIndexSelection, playerProperties.UpdateDisplayedDebugInfo })
                },
                {
                    new StateTransition<State, Command>(State.SelectingCardGiveAwayPlayerIndex, Command.CardGiveAwayUnfinished),
                    new StateTransitionResult<State>(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { playerProperties.EnterCardGiveAwaySelection })
                },
                {
                    new StateTransition<State, Command>(State.SelectingCardGiveAwayPlayerIndex, Command.TurnEnded),
                    new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener>{ playerProperties.ExitCardGiveAwayPlayerIndexSelection, playerProperties.DisconnectCamera})
                },
                {
                    new StateTransition<State, Command>(State.SelectingPlayPileGiveAwayPlayerIndex, Command.TurnEnded),
                    new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener> { playerProperties.ExitPlayPileGiveAwayPlayerIndexSelection })
                },
                {
                    new StateTransition<State, Command>(State.WaitingForTurn, Command.VotingStarted),
                    new StateTransitionResult<State>(State.VotingForWinner, new List<StateTransitionListener> { playerProperties.EnterVotingForWinner })
                },
                {
                    new StateTransition<State, Command>(State.WaitingForTurn, Command.TurnStarted),
                    new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ playerProperties.EnterPickingActionUpdateUI, playerProperties.EnableCamera })
                },
                {
                    new StateTransition<State, Command>(State.WaitingForTurn, Command.HasNoCards),
                    new StateTransitionResult<State>(State.PotentialWinner)
                },
                {
                    new StateTransition<State, Command>(State.WaitingForTurn, Command.GameEnded),
                    new StateTransitionResult<State>(State.GameOver)
                },
                {
                    new StateTransition<State, Command>(State.WaitingForTurn, Command.MulliganStarted),
                    new StateTransitionResult<State>(State.Mulligan, new List<StateTransitionListener> { playerProperties.EnterMulligan, playerProperties.EnableCamera })
                },
                {
                    new StateTransition<State, Command>(State.VotingForWinner, Command.GameEnded),
                    new StateTransitionResult<State>(State.GameOver, new List<StateTransitionListener> { playerProperties.DisconnectCamera, playerProperties.ExitVotingForWinner })
                },
                {
                    new StateTransition<State, Command>(State.PotentialWinner, Command.GotJokered),
                    new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener> { playerProperties.HideUI })
                },
                {
                    new StateTransition<State, Command>(State.PotentialWinner, Command.GameEnded),
                    new StateTransitionResult<State>(State.GameOver, new List<StateTransitionListener> {playerProperties.DisconnectCamera, playerProperties.ExitVotingForWinner})
                }
            };
                CurrentState = State.Null;
            }

            public PlayerStateMachine(State startState, PlayerHandler playerProperties) : this(playerProperties) { CurrentState = startState; }
        }
    }
}