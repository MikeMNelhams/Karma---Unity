using System.Collections.Generic;
using NUnit.Framework;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Cards;

public class CardComboTwoTests
{
    /* NOT for testing when the 2 is Playable, only that its effect works when played */

    [Test]
    public void TwoResetsPlayOrderOnly()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);

        board.StartTurn();

        PlayerController testController = new ();

        CardsList cards = new (new List<int>() { 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);  // Resets coz it's a non effect multiplying increasing card
        Assert.True(board.HandsAreFlipped);
    }

    [Test]
    public void TwoDoesNothingWhenAlreadyReset()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.UP);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }

    [Test]
    public void TwoExclusiveGroupComboResetsPlayOrderOnly()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 2, 2, 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }

    [Test]
    public void TwoWithFillerGroupComboResetsPlayOrderOnly()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 2, 2, 2, 6 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }

    [Test]
    public void TwosBurningFromPlayPileStillResetsPlayOrder()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 2, 2, 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }
}
