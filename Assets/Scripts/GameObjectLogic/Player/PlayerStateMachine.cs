using KarmaLogic.Board;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineV2
{
    public class PlayerStateMachine : StateMachine
    {
        protected PlayerProperties _playerProperties;
        protected IBoard _board;

        public PlayerStateMachine(PlayerProperties playerProperties, IBoard board)
        {
            _playerProperties = playerProperties;
            _board = board;
            Transitions = new Dictionary<StateTransition, StateTransitionResult>
            {
                {
                    new StateTransition(State.Null, Command.Mulligan),
                    new StateTransitionResult(State.Mulligan, new List<StateTransitionListener>{ playerProperties.EnterMulligan })
                },
                {
                    new StateTransition(State.Null, Command.TurnStarted),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener>{ playerProperties.EnterPickingActionUpdateUI, playerProperties.EnableCamera })
                },
                {
                    new StateTransition(State.Null, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener> { playerProperties.DisableCamera })
                },
                {
                    new StateTransition(State.Null, Command.VotingStarted),
                    new StateTransitionResult(State.VotingForWinner, new List<StateTransitionListener> { playerProperties.EnableCamera, playerProperties.EnterVotingForWinner})
                },
                {
                    new StateTransition(State.Null, Command.HasNoCards),
                    new StateTransitionResult(State.PotentialWinner, new List<StateTransitionListener> { playerProperties.DisableCamera })
                },
                {
                    new StateTransition(State.Mulligan, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener>{ playerProperties.ExitMulligan, playerProperties.HideUI })
                },
                {
                    new StateTransition(State.Mulligan, Command.TurnStarted),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener>{ playerProperties.ExitMulligan, playerProperties.EnterPickingActionUpdateUI })
                },
                {
                    new StateTransition(State.PickingAction, Command.PlayPileGiveAwayComboPlayed),
                    new StateTransitionResult(State.SelectingPlayPileGiveAwayPlayerIndex, new List<StateTransitionListener>{ playerProperties.EnterPlayPileGiveAwayPlayerIndexSelection })
                },
                {
                    new StateTransition(State.PickingAction, Command.CardGiveAwayComboPlayed),
                    new StateTransitionResult(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { playerProperties.EnterCardGiveAwaySelection })
                },
                {
                    new StateTransition(State.PickingAction, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener>{ playerProperties.ExitPickingActionUpdateUI, playerProperties.DisableCamera})
                },
                {
                    new StateTransition(State.PickingAction, Command.HasNoCards),
                    new StateTransitionResult(State.PotentialWinner, new List<StateTransitionListener> {playerProperties.ExitPickingActionUpdateUI, playerProperties.DisableCamera})
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayIndex, Command.CardGiveAwayIndexSelected),
                    new StateTransitionResult(State.SelectingCardGiveAwayPlayerIndex, new List<StateTransitionListener>{ playerProperties.EnterCardGiveAwayPlayerIndexSelection, playerProperties.UpdateDisplayedDebugInfo })
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayPlayerIndex, Command.CardGiveAwayUnfinished),
                    new StateTransitionResult(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { playerProperties.EnterCardGiveAwaySelection })
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayPlayerIndex, Command.Burned),
                    new StateTransitionResult(State.PickingAction)
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayPlayerIndex, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener>{ playerProperties.ExitCardGiveAwayPlayerIndexSelection, playerProperties.DisableCamera})
                },
                {
                    new StateTransition(State.SelectingPlayPileGiveAwayPlayerIndex, Command.Burned),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener> { playerProperties.ExitPlayPileGiveAwayPlayerIndexSelection })
                },
                {
                    new StateTransition(State.SelectingPlayPileGiveAwayPlayerIndex, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener> { playerProperties.ExitPlayPileGiveAwayPlayerIndexSelection })
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.VotingStarted),
                    new StateTransitionResult(State.VotingForWinner, new List<StateTransitionListener> { playerProperties.EnterVotingForWinner })
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.TurnStarted),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener>{ playerProperties.EnterPickingActionUpdateUI, playerProperties.EnableCamera })
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.HasNoCards),
                    new StateTransitionResult(State.PotentialWinner)
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.GameEnded),
                    new StateTransitionResult(State.GameOver)
                },
                {
                    new StateTransition(State.VotingForWinner, Command.GameEnded),
                    new StateTransitionResult(State.GameOver, new List<StateTransitionListener> { playerProperties.DisableCamera, playerProperties.ExitVotingForWinner })
                },
                {
                    new StateTransition(State.PotentialWinner, Command.GotJokered),
                    new StateTransitionResult(State.WaitingForTurn)
                },
                {
                    new StateTransition(State.PotentialWinner, Command.TurnEnded),
                    new StateTransitionResult(State.GameOver, new List<StateTransitionListener> {playerProperties.DisableCamera, playerProperties.ExitVotingForWinner})
                }
            };
            CurrentState = State.Null;
        }

        public PlayerStateMachine(State startState, PlayerProperties playerProperties, IBoard board) : this(playerProperties, board) { CurrentState = startState; }
    }
}