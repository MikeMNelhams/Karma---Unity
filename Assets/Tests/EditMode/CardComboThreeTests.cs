using System.Collections.Generic;
using NUnit.Framework;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Cards;

public class CardComboThreeTests
{
    [Test]
    public void ThreeDoublesEffectMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 3 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(4, board.EffectMultiplier);
    }

    [Test]
    public void ThreeComboMultipliesEffectMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 1);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 3, 3, 3 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(8, board.EffectMultiplier);
    }

    [Test]
    public void FourThreesResetsMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 4);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 3, 3, 3, 3 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(1, board.EffectMultiplier);
    }

    [Test]
    public void ThreesBurningFromPlayPileResetsMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 4);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 3, 3, 3 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(1, board.EffectMultiplier);
    }

    [Test]
    public void ThreeWithFillerGroupComboCorrectlyMultipliesEffectMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 1);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 3, 3, 3, 6 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(16, board.EffectMultiplier);
    }
}
