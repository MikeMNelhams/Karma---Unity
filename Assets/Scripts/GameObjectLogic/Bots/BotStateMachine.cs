using System.Collections.Generic;
using System.Threading.Tasks;
using KarmaLogic.Board;
using KarmaLogic.Bots;
using KarmaLogic.BasicBoard;
using KarmaLogic.Cards;
using DataStructures;

namespace StateMachineV2
{
    public class BotStateMachine : StateMachine
    {
        protected IBoard _board;
        protected IBot _bot;
        protected PlayerProperties _playerProperties;

        public BotStateMachine(IBot bot, IBoard board, PlayerProperties playerProperties)
        {
            _board = board;
            _bot = bot;
            _playerProperties = playerProperties;

            Transitions = new Dictionary<StateTransition, StateTransitionResult>()
            {
                {
                    new StateTransition(State.Null, Command.Mulligan),
                    new StateTransitionResult(State.Mulligan)
                },
                {
                    new StateTransition(State.Null, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener> { })
                },
                {
                    new StateTransition(State.Null, Command.TurnStarted),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener>{ Delay, EnterPickingAction })
                },
                {
                    new StateTransition(State.PickingAction, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn, new List<StateTransitionListener> { })
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.TurnStarted),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener>{ Delay, EnterPickingAction })
                }
            };
        }

        async Task Delay()
        {
            await Task.Delay((int)(_bot.DelaySeconds * 1000));
        }
        
        Task EnterPickingAction()
        {
            BoardPlayerAction selectedAction = _bot.SelectAction(_board);
            if (!_board.CurrentLegalActions.Contains(selectedAction))
            {
                throw new InvalidBoardPlayerActionException(selectedAction);
            }
            UnityEngine.Debug.Log("Bot selected action: " + selectedAction);
            // The button methods NEED to be awaited. May have to create an entirely custom button class :(
            if (selectedAction is PickupPlayPile) { _playerProperties.PickupPlayPileButton.onClick?.Invoke(); return Task.CompletedTask; }
            if (selectedAction is not PlayCardsCombo) { throw new InvalidBoardPlayerActionException(selectedAction); }
            FrozenMultiSet<CardValue> selectedCombo = _bot.ComboToPlay(_board);
            MultiSet<CardValue> combo = new();

            foreach (SelectableCard cardObject in _playerProperties.SelectableCardObjects)
            {
                CardValue cardValue = cardObject.CurrentCard.Value;
                if (!selectedCombo.Contains(cardValue)) { continue; }
                if (combo.Contains(cardValue) && combo[cardValue] >= selectedCombo[cardValue]) { continue; }

                combo.Add(cardValue, 1);
                _playerProperties.TryToggleCardSelect(cardObject);
            }

            _playerProperties.ConfirmSelectionButton.onClick?.Invoke(); // The button methods NEED to be awaited. May have to create an entirely custom button class :(
            return Task.CompletedTask;
        }
    }
}

