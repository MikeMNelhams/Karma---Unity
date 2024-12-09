using System.Collections.Generic;
using System.Threading.Tasks;
using KarmaLogic.Board;
using KarmaLogic.Bots;
using KarmaLogic.BasicBoard;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using DataStructures;

namespace StateMachine.CharacterStateMachines
{
    public class BotStateMachine : StateMachine<State, Command>
    {
        protected IBoard _board;
        protected BotBase _bot;
        protected PlayerHandler _playerHandler;

        public BotStateMachine(BotBase bot, PlayerHandler playerHandler, IBoard board)
        {
            _board = board;
            _bot = bot;
            _playerHandler = playerHandler;

            Transitions = new Dictionary<StateTransition<State, Command>, StateTransitionResult<State>>()
        {
            {
                new StateTransition<State, Command>(State.Null, Command.MulliganStarted),
                new StateTransitionResult<State>(State.Mulligan)
            },
            {
                new StateTransition<State, Command>(State.Null, Command.TurnEnded),
                new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener> { })
            },
            {
                new StateTransition<State, Command>(State.Null, Command.TurnStarted),
                new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ Delay, EnterPickingAction })
            },
            {
                new StateTransition<State, Command>(State.Null, Command.VotingStarted),
                new StateTransitionResult<State>(State.GameOver, new List<StateTransitionListener> { Delay, EnterVotingForWinner })
            },
            {
                new StateTransition<State, Command>(State.Null, Command.HasNoCards),
                new StateTransitionResult<State>(State.PotentialWinner)
            },
            {
                new StateTransition<State, Command>(State.Mulligan, Command.TurnStarted),
                new StateTransitionResult<State>(State.Mulligan, new List<StateTransitionListener> { EnterMulligan })
            },
            {
                new StateTransition<State, Command>(State.Mulligan, Command.TurnEnded),
                new StateTransitionResult<State>(State.WaitingForTurn, new List<StateTransitionListener>{ playerHandler.ExitMulligan, playerHandler.HideUI })
            },
            {
                new StateTransition<State, Command>(State.Mulligan, Command.MulliganEnded),
                new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ playerHandler.ExitMulligan, playerHandler.EnterPickingActionUpdateUI })
            },
            { 
                new StateTransition<State, Command>(State.PickingAction, Command.TurnStarted),
                new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ Delay, EnterPickingAction })
            },
            {
                new StateTransition<State, Command>(State.PickingAction, Command.TurnEnded),
                new StateTransitionResult<State>(State.WaitingForTurn)
            },
            {
                new StateTransition<State, Command>(State.PickingAction, Command.CardGiveAwayComboPlayed),
                new StateTransitionResult<State>(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { Delay, EnterCardGiveAwaySelection})
            },
            {
                new StateTransition<State, Command>(State.PickingAction, Command.PlayPileGiveAwayComboPlayed),
                new StateTransitionResult<State>(State.SelectingPlayPileGiveAwayPlayerIndex, new List<StateTransitionListener> { Delay, EnterPlayPileGiveAwayPlayerIndexSelection })
            },
            {
                new StateTransition<State, Command>(State.PickingAction, Command.GameEnded),
                new StateTransitionResult<State>(State.Null)
            },
            {
                new StateTransition<State, Command>(State.PickingAction, Command.HasNoCards),
                new StateTransitionResult<State>(State.PotentialWinner, new List<StateTransitionListener> { })
            },
            {
                new StateTransition<State, Command>(State.WaitingForTurn, Command.TurnStarted),
                new StateTransitionResult<State>(State.PickingAction, new List<StateTransitionListener>{ Delay, EnterPickingAction })
            },
            {
                new StateTransition<State, Command>(State.WaitingForTurn, Command.GameEnded),
                new StateTransitionResult<State>(State.Null)
            },
            {
                new StateTransition<State, Command>(State.WaitingForTurn, Command.HasNoCards),
                new StateTransitionResult<State>(State.PotentialWinner)
            },
            {
                new StateTransition<State, Command>(State.WaitingForTurn, Command.VotingStarted),
                new StateTransitionResult<State>(State.GameOver, new List<StateTransitionListener> { Delay, EnterVotingForWinner })
            },
            {
                new StateTransition<State, Command>(State.SelectingCardGiveAwayIndex, Command.CardGiveAwayIndexSelected),
                new StateTransitionResult<State>(State.SelectingCardGiveAwayPlayerIndex, new List<StateTransitionListener> { Delay, EnterCardGiveAwayPlayerIndexSelection})
            },
            {
                new StateTransition<State, Command>(State.SelectingCardGiveAwayPlayerIndex, Command.CardGiveAwayUnfinished),
                new StateTransitionResult<State>(State.SelectingCardGiveAwayIndex, new List<StateTransitionListener> { Delay, EnterCardGiveAwaySelection })
            },
            {
                new StateTransition<State, Command>(State.SelectingCardGiveAwayPlayerIndex, Command.TurnEnded),
                new StateTransitionResult<State>(State.WaitingForTurn)
            },
            {
                new StateTransition<State, Command>(State.SelectingPlayPileGiveAwayPlayerIndex, Command.TurnEnded),
                new StateTransitionResult<State>(State.WaitingForTurn)
            },
            {
                new StateTransition<State, Command>(State.VotingForWinner, Command.GameEnded),
                new StateTransitionResult<State>(State.GameOver)
            },
            {
                new StateTransition<State, Command>(State.PotentialWinner, Command.GotJokered),
                new StateTransitionResult<State>(State.WaitingForTurn)
            },
            {
                new StateTransition<State, Command>(State.PotentialWinner, Command.GameEnded),
                new StateTransitionResult<State>(State.GameOver)
            }
        };
        }

        async Task Delay()
        {
            await Task.Delay((int)(_bot.DelaySeconds * 1000));
        }

        async Task EnterMulligan()
        {
            for (int i = 0; i < 100; i++)
            {
                if (!_bot.WantsToMulligan(_board))
                {
                    await _playerHandler.FinishMulliganButton.onClick?.Invoke();
                    return;
                }

                int handIndex = _bot.MulliganHandIndex(_board);
                int karmaUpIndex = _bot.MulliganKarmaUpIndex(_board);

                SelectableCardObject handCard = _playerHandler.CardsInHand[handIndex];
                SelectableCardObject karmaUpCard = _playerHandler.CardsInKarmaUp[karmaUpIndex];

                _playerHandler.TryToggleCardSelect(handCard);
                _playerHandler.TryToggleCardSelect(karmaUpCard);

                await _playerHandler.AttemptMulliganSwap(_board);
                await _playerHandler.ConfirmSelectionButton.onClick?.Invoke();
            }
        }

        async Task EnterPickingAction()
        {
            BoardPlayerAction selectedAction = _bot.SelectAction(_board);
            if (!_board.CurrentLegalActions.Contains(selectedAction))
            {
                throw new InvalidBoardPlayerActionException(selectedAction);
            }
            UnityEngine.Debug.Log("Bot selected action: " + selectedAction);

            if (selectedAction is PickupPlayPile) { await _playerHandler.PickupPlayPileButton.onClick?.Invoke(); return; }
            if (selectedAction is not PlayCardsCombo) { throw new InvalidBoardPlayerActionException(selectedAction); }
            FrozenMultiSet<CardValue> selectedCombo = _bot.ComboToPlay(_board);
            MultiSet<CardValue> combo = new();
            foreach (SelectableCardObject cardObject in _playerHandler.SelectableCardObjects)
            {
                CardValue cardValue = cardObject.CurrentCard.Value;
                if (!selectedCombo.Contains(cardValue)) { continue; }
                if (combo.Contains(cardValue) && combo[cardValue] >= selectedCombo[cardValue]) { continue; }

                combo.Add(cardValue, 1);
                _playerHandler.TryToggleCardSelect(cardObject);
            }

            await _playerHandler.ConfirmSelectionButton.onClick?.Invoke();
        }

        async Task EnterCardGiveAwaySelection()
        {
            int cardGiveAwayIndex = _bot.CardGiveAwayIndex(_board);
            SelectableCardObject selectedGiveawayCard = _playerHandler.SelectableCardObjects[cardGiveAwayIndex];
            _playerHandler.TryToggleCardSelect(selectedGiveawayCard);
            await _playerHandler.ConfirmSelectionButton.onClick?.Invoke();
        }

        Task EnterCardGiveAwayPlayerIndexSelection()
        {
            HashSet<int> invalidTargetIndices = new() { _playerHandler.Index };
            for (int i = 0; i < _board.Players.Count; i++)
            {
                Player player = _board.Players[i];
                if (!player.HasCards)
                {
                    invalidTargetIndices.Add(i);
                }
            }

            int targetIndex = _bot.CardPlayerGiveAwayIndex(_board, invalidTargetIndices);
            _playerHandler.TriggerTargetReceivePickedUpCard(targetIndex);
            return Task.CompletedTask;
        }

        async Task EnterPlayPileGiveAwayPlayerIndexSelection()
        {
            int targetIndex = _bot.JokerTargetIndex(_board, new HashSet<int>() { _playerHandler.Index });
            await _playerHandler.TriggerTargetPickUpPlayPile(targetIndex);
        }

        Task EnterVotingForWinner()
        {
            int targetIndex = _bot.VoteForWinnerIndex(_board, new HashSet<int> { _playerHandler.Index });
            _playerHandler.TriggerVoteForPlayer(targetIndex);
            return Task.CompletedTask;
        }
    }
}

