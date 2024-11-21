using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardComboFourTests
{
    [Test]
    public void SingleFourEmptyBoard()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4 }, CardSuit.DebugDefault);

        Assert.AreEqual(null, board.PlayPile.VisibleTopCard);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.True(board.HandsAreFlipped);
        Assert.AreEqual(1, board.EffectMultiplier);  // Non 3 should reset effect multiplier

        Assert.AreEqual(null, board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void FourGroupOnFourInvisible()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 4 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4, 4 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(null, board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void FourGroupOnNonFourvisible()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 4, 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4, 4 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.THREE), board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void FourGroupOnFourOnNonFourvisible()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 4, 3, 4 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4, 4 }, CardSuit.DebugDefault);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.THREE), board.PlayPile.VisibleTopCard);

        board.PlayCards(cards);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.THREE), board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void FourGroupSixFilledOnEmpty()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4, 6, 4, 4 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(null, board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void FourGroupSixFilledOnFour()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 4, 4 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4, 6, 4, 4 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(null, board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void FourGroupSixFilledOnFourOnNonFour()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3, 4, 3, 4, 4 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 4, 6, 4, 4 }, CardSuit.DebugDefault);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.THREE), board.PlayPile.VisibleTopCard);

        board.PlayCards(cards);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.THREE), board.PlayPile.VisibleTopCard);
    }
}
