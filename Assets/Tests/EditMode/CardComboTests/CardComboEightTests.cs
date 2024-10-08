using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardComboEightTests
{
    [Test]
    public void EightReversesTurnOrderOnly()
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

        CardsList cards = new(new List<int>() { 8 }, CardSuit.DebugDefault);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
    }

    [Test]
    public void EightEvenCountComboDoesNothing()
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

        CardsList cards = new(new List<int>() { 8, 8 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
    }

    [Test]
    public void EightOddCountComboReversesPlayOrder()
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

        CardsList cards = new(new List<int>() { 8, 8, 8 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
    }

    [Test]
    public void EightOddFilledComboReversesPlayOrder()
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

        CardsList cards = new(new List<int>() { 8, 8, 8, 6, 6 }, CardSuit.DebugDefault);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
    }
}
