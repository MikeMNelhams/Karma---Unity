using DataStructures;
using Karma.Board;
using Karma.Cards;
using Karma.Controller;
using System.Collections.Generic;
using Karma.Bots;

public class BotController : IController
{
    public bool IsAwaitingInput { get; set; }
    public FrozenMultiSet<CardValue> SelectedCardValues { get; set; }
    public BoardPlayerAction SelectedAction { get; set; }
    public IBot _bot;

    public BotController(IBot bot)
    {
        _bot = bot;
        IsAwaitingInput = false;
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

    public BoardPlayerAction SelectAction(IBoard board) 
    {
        BoardPlayerAction selectedAction = _bot.SelectAction(board);
        if (!board.CurrentLegalActions.Contains(selectedAction))
        {
            throw new InvalidBoardPlayerActionException(selectedAction);
        }

        return selectedAction;

    }

    public FrozenMultiSet<CardValue> SelectCardsToPlay(IBoard board)
    {
        return _bot.CardsToPlay(board);
    }

    public int VoteForWinner(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        return _bot.VoteForWinnerIndex(board, excludedPlayerIndices);
    }
}
