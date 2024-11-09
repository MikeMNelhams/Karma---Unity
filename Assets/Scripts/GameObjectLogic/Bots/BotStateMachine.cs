using System.Collections.Generic;
using System.Threading.Tasks;
using KarmaLogic.Board;
using KarmaLogic.Bots;
using KarmaLogic.BasicBoard;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using DataStructures;

namespace StateMachineV2
{
    public class BotStateMachine : StateMachine
    {
        protected IBoard _board;
        protected IBot _bot;
        protected PlayerProperties _playerProperties;

        public BotStateMachine(IBot bot, PlayerProperties playerProperties, IBoard board)
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
                    new StateTransition(State.PickingAction, Command.CardGiveAwayComboPlayed),
                    new StateTransitionResult(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { Delay, EnterCardGiveAwaySelection})
                },
                {
                    new StateTransition(State.PickingAction, Command.PlayPileGiveAwayComboPlayed),
                    new StateTransitionResult(State.SelectingPlayPileGiveAwayPlayerIndex, new List<StateTransitionListener> { Delay })
                },
                {
                    new StateTransition(State.PickingAction, Command.GameEnded),
                    new StateTransitionResult(State.Null)
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.TurnStarted),
                    new StateTransitionResult(State.PickingAction, new List<StateTransitionListener>{ Delay, EnterPickingAction })
                },
                {
                    new StateTransition(State.WaitingForTurn, Command.GameEnded),
                    new StateTransitionResult(State.Null)
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayIndex, Command.CardGiveAwayIndexSelected),
                    new StateTransitionResult(State.SelectingCardGiveAwayPlayerIndex, new List<StateTransitionListener> { Delay, EnterCardGiveAwayPlayerIndexSelection})
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayPlayerIndex, Command.TurnEnded),
                    new StateTransitionResult(State.WaitingForTurn)
                },
                {
                    new StateTransition(State.SelectingCardGiveAwayPlayerIndex, Command.Burned),
                    new StateTransitionResult(State.PickingAction)
                },
                {
                    new StateTransition(State.SelectingPlayPileGiveAwayPlayerIndex, Command.Burned),
                    new StateTransitionResult(State.PickingAction)
                },
                {
                    new StateTransition(State.VotingForWinner, Command.GameEnded),
                    new StateTransitionResult(State.Null)
                }
            };
        }

        async Task Delay()
        {
            await Task.Delay((int)(_bot.DelaySeconds * 1000));
        }
        
        async Task EnterPickingAction()
        {
            BoardPlayerAction selectedAction = _bot.SelectAction(_board);
            if (!_board.CurrentLegalActions.Contains(selectedAction))
            {
                throw new InvalidBoardPlayerActionException(selectedAction);
            }
            UnityEngine.Debug.Log("Bot selected action: " + selectedAction);
            // The button methods NEED to be awaited. May have to create an entirely custom button class :(
            if (selectedAction is PickupPlayPile) { await _playerProperties.PickupPlayPileButton.onClick?.Invoke(); return; }
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

            await _playerProperties.ConfirmSelectionButton.onClick?.Invoke(); // The button methods NEED to be awaited. May have to create an entirely custom button class :(
        }

        async Task EnterCardGiveAwaySelection()
        {
            int cardGiveAwayIndex = _bot.CardGiveAwayIndex(_board);
            SelectableCard selectedGiveawayCard = _playerProperties.SelectableCardObjects[cardGiveAwayIndex];
            _playerProperties.TryToggleCardSelect(selectedGiveawayCard);
            await _playerProperties.ConfirmSelectionButton.onClick?.Invoke();
        }

        Task EnterCardGiveAwayPlayerIndexSelection()
        {
            HashSet<int> invalidTargetIndices = new() { _playerProperties.Index };
            for (int i = 0; i < _board.Players.Count; i++)
            {
                Player player = _board.Players[i];
                if (!player.HasCards)
                {
                    invalidTargetIndices.Add(i);
                }
            }

            int targetIndex = _bot.CardPlayerGiveAwayIndex(_board, invalidTargetIndices);
            _playerProperties.TriggerTargetReceivePickedUpCard(targetIndex);
            return Task.CompletedTask;
        }
    }
}

