using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardComboQueenTests
{
    [Test]
    public void QueenNoCardsToGiveAway()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();


        CardsList cards = new(new List<int>() { 12 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.Null(board.Players[0].CardGiveAwayHandler);
    }

    [Test]
    public void QueenCardsToGiveAway()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { 10, 11, 12 }, new() { } },
            new() { new() { }, new() { 2 }, new() { } },
            new() { new() { }, new() { 3 }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 12 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.NotNull(board.Players[0].CardGiveAwayHandler);
        Assert.AreEqual(1, board.Players[0].CardGiveAwayHandler.NumberOfCardsRemainingToGiveAway);
    }

    [Test]
    public void QueenCardsToGiveAwayButNoValidTargets()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { 10, 11}, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 12 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.Null(board.Players[0].CardGiveAwayHandler);
    }

    [Test]
    public void QueenNotAllowedToGiveAwayJoker()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { 15}, new() { } },
            new() { new() { }, new() { 2 }, new() { } },
            new() { new() { }, new() { 3 }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues,
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 12 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.Null(board.Players[0].CardGiveAwayHandler);
    }

    [Test]
    public void QueenHandToKarmaUpAssertIsValid()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 12 }, new() { 6 }, new() { 7 } },
            new() { new() { 2 }, new() { 3 }, new() { 4 } },
            new() { new() { 3 }, new() { 4 }, new() { 5 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.CurrentPlayer.PopFromPlayable(new int[] { 0 }); 

        board.PlayCards(cards);

        Assert.NotNull(board.Players[0].CardGiveAwayHandler);
        Assert.AreEqual(1, board.Players[0].CardGiveAwayHandler.NumberOfCardsRemainingToGiveAway);
    }

    [Test]
    public void QueenKarmaUpToKarmaDownAssertIsInvalid()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { 12 }, new() { 7 } },
            new() { new() { 2 }, new() { 3 }, new() { 4 } },
            new() { new() { 3 }, new() { 4 }, new() { 5 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.CurrentPlayer.PopFromPlayable(new int[] { 0 });

        board.PlayCards(cards);

        Assert.Null(board.Players[0].CardGiveAwayHandler);
    }
}
