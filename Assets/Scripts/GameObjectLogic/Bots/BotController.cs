using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using KarmaLogic.Controller;
using System.Collections.Generic;
using KarmaLogic.Bots;
using KarmaLogic.BasicBoard;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class BotController : Controller
{
    readonly IBot _bot;

    public int DelaySeconds { get => _bot.DelaySeconds; }

    public BotController(IBot bot)
    {
        _bot = bot;
    }

    public BoardTurnOrder ChooseStartDirection(IBoard board)
    {
        return _bot.PreferredStartDirection(board);
    }

    public int GiveAwayCardIndex(IBoard board)
    {
        return _bot.CardGiveAwayIndex(board);
    }

    public int GiveAwayPlayerIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        return _bot.CardPlayerGiveAwayIndex(board, excludedPlayerIndices);
    }

    public int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        return _bot.JokerTargetIndex(board, excludedPlayerIndices);
    }

    public bool WantsToMulligan(IBoard board)
    {
        return _bot.WantsToMulligan(board);
    }

    public int MulliganHandIndex(IBoard board)
    {
        return _bot.MulliganHandIndex(board);
    }

    public int MulliganKarmaUpIndex(IBoard board)
    {
        return _bot.MulliganKarmaUpIndex(board);
    }

    public FrozenMultiSet<CardValue> SelectComboToPlay(IBoard board)
    {
        return _bot.ComboToPlay(board);
    }

    public override Task EnterWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        return Task.CompletedTask;
    }

    public override Task ExitWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        return Task.CompletedTask;
    }

    public override Task EnterPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        BoardPlayerAction selectedAction = _bot.SelectAction(board);
        if (!board.CurrentLegalActions.Contains(selectedAction))
        {
            throw new InvalidBoardPlayerActionException(selectedAction);
        }
        UnityEngine.Debug.Log("Bot selected action: " + selectedAction);
        if (selectedAction is PickupPlayPile) { characterProperties.PickupPlayPileButton.onClick?.Invoke(); return Task.CompletedTask; ; }
        if (selectedAction is not PlayCardsCombo) { throw new InvalidBoardPlayerActionException(selectedAction); }
        FrozenMultiSet<CardValue> selectedCombo = SelectComboToPlay(board);
        MultiSet<CardValue> combo = new ();

        foreach (SelectableCard cardObject in characterProperties.SelectableCardObjects)
        {
            CardValue cardValue = cardObject.CurrentCard.Value;
            if (!selectedCombo.Contains(cardValue)) { continue; }
            if (combo.Contains(cardValue) && combo[cardValue] >= selectedCombo[cardValue]) { continue; }

            combo.Add(cardValue, 1);
            characterProperties.TryToggleCardSelect(cardObject);
        }

        characterProperties.ConfirmSelectionButton.onClick?.Invoke();
        return Task.CompletedTask;
    }

    public override Task ExitPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        return Task.CompletedTask;
    }

    public override async Task EnterVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        int voteTargetIndex = _bot.VoteForWinnerIndex(board, new HashSet<int>() { characterProperties.Index });
        await characterProperties.TriggerVoteForPlayer(voteTargetIndex);
    }

    public override Task ExitVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        return Task.CompletedTask;
    }

    public override Task EnterCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        int cardGiveAwayIndex = _bot.CardGiveAwayIndex(board);
        UnityEngine.Debug.Log("Giving card to player index: " + cardGiveAwayIndex);
        SelectableCard selectedGiveawayCard = characterProperties.SelectableCardObjects[cardGiveAwayIndex];
        characterProperties.TryToggleCardSelect(selectedGiveawayCard);
        characterProperties.ConfirmSelectionButton.onClick?.Invoke();
        return Task.CompletedTask;

    }

    public override Task ExitCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        return Task.CompletedTask;
    }

    public override async Task EnterCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        HashSet<int> invalidTargetIndices = new () { characterProperties.Index };
        for (int i = 0; i < board.Players.Count; i++)
        {
            Player player = board.Players[i];
            if (!player.HasCards)
            {
                invalidTargetIndices.Add(i);
            }
        }

        int targetIndex = _bot.CardPlayerGiveAwayIndex(board, invalidTargetIndices);
        await characterProperties.TriggerTargetReceivePickedUpCard(targetIndex);
    }

    public override Task ExitCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        UnityEngine.Debug.Log("exiting player index selection state!");
        return Task.CompletedTask;
    }

    public override Task EnterPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override Task ExitPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }
}
