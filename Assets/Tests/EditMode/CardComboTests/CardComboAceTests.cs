using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardComboAceTests
{
    [Test]
    public void AceFlipsHands()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT);
        BasicBoard board = new (boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 14 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);
    }

    [Test]
    public void AceFlipsHandsBackToNotFlipped()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 14 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }

    [Test]
    public void AceGroupComboEven()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 14, 14 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);
    }

    [Test]
    public void AceGroupComboOdd()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 14, 14, 14 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);
    }

    [Test]
    public void AceGroupComboOddSixFilled()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 14, 14, 14, 6, 14 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);
    }
}
