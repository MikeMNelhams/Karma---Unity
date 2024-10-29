using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Controller;
using System.Collections.Generic;
using KarmaLogic.Bots;
using KarmaLogic.BasicBoard;
using UnityEngine;
using System.Collections;

public class BotController : Controller
{
    public IBot _bot;

    public BotController(IBot bot)
    {
        _bot = bot;
    }

    public BoardTurnOrder ChooseStartDirection(IBoard board)
    {
        return _bot.PreferredStartDirection(board);
    }

    public int GiveAwayCardIndex(IBoard board, HashSet<int> excludedCardsIndices)
    {
        return _bot.CardGiveAwayIndex(board, excludedCardsIndices);
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

    public int VoteForWinner(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        return _bot.VoteForWinnerIndex(board, excludedPlayerIndices);
    }

    public override void EnterWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        
    }

    public override void ExitWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        
    }

    public override void EnterPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        BoardPlayerAction selectedAction = _bot.SelectAction(board);
        if (!board.CurrentLegalActions.Contains(selectedAction))
        {
            throw new InvalidBoardPlayerActionException(selectedAction);
        }
        UnityEngine.Debug.Log("Bot selected action: " + selectedAction);
        if (selectedAction is PickupPlayPile) { characterProperties.PickupPlayPileButton.onClick?.Invoke(); return; }
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

        UnityEngine.Debug.Log("State of confirmed player " + characterProperties.Controller.State.GetHashCode());
        characterProperties.ConfirmSelectionButton.onClick?.Invoke();
    }

    public override void ExitPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        
    }

    public override void EnterVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void EnterCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void EnterCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void EnterPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        throw new System.NotImplementedException();
    }
}
