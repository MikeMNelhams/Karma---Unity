using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new (new List<int>() { 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, playOrder: BoardPlayOrder.UP);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, playOrder: BoardPlayOrder.DOWN);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 2, 2, 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, playOrder: BoardPlayOrder.DOWN);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 2, 2, 2, 6 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, playOrder: BoardPlayOrder.DOWN);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 2, 2, 2 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }
}
