using System.Collections.Generic;
using NUnit.Framework;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Cards;

public class CardComboSevenTests
{
    [Test]
    public void SevenReversesPlayOrderOnly()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 7 }, CardSuit.Hearts);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);
    }

    [Test]
    public void SevenEvenCountComboDoesNothing()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 7, 7 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
    }

    [Test]
    public void SevenOddCountComboReversesPlayOrder()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 7, 7, 7 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
    }

    [Test]
    public void SevenOddFilledComboReversesPlayOrder()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 7, 7, 7, 6, 6 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
    }
}
