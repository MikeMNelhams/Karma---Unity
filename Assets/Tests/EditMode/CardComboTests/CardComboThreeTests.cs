using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 3 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 1);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 3, 3, 3 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 4);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 3, 3, 3, 3 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 4);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 3, 3, 3 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(1, board.EffectMultiplier);
    }

    [Test]
    public void ThreeWithFillerGroupSixAtEndComboCorrectlyMultipliesEffectMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 1);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 3, 3, 3, 6 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(16, board.EffectMultiplier);
    }

    [Test]
    public void ThreeWithFillerGroupNotSixAtEndComboCorrectlyMultipliesEffectMultiplier()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 1);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 3, 3, 6, 3, 3 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(32, board.EffectMultiplier);
    }
}
