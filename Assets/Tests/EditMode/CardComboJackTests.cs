using System.Collections.Generic;
using NUnit.Framework;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Cards;
using Karma.Players;
using UnityEditor;

public class CardComboJackTests
{
    [Test]
    public void JackOnEmpty()
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

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);
        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(1, board.PlayPile.Count);
        Assert.AreEqual(new Card(CardSuit.Hearts, CardValue.JACK), board.PlayPile[0]);
    }

    [Test]
    public void JackOnTwo()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(2, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new (new List<int>() { 2, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackSingleOnThreeOnlyDoublesOnce()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(4, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackGroupComboOnThreeMultipliesByTwoToPowerOfJackCount()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11, 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(8, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 11, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackGroupComboSixFilledOnThreeMultipliesByTwoToPowerOfComboCardCount()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11, 11, 6, 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(32, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 11, 11, 6, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackOnFourInvisible()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 4 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 4, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(null, board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void JackOnFourOnVisibleCardIsVisible()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3, 4 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 11, 11, 6, 11 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 4, 11, 11, 6, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(new Card(CardSuit.Hearts, CardValue.THREE), board.PlayPile.VisibleTopCard);
    }

    [Test]
    public void JackOnFive()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 2, 3, 4 }, new() { }, new() { } },
            new() { new() { 5, 5, 11}, new() { }, new() { } },
            new() { new() { 6, 7, 8}, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 9, 10 };
        List<int> playCardValues = new() { 5 };
        List<int> burnCardValues = new() { 2 };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 2, whoStarts: 1);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = board.Players[1].PopFromPlayable(new int[] { 2 });
        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(1, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 5, 11 }, CardSuit.Hearts);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Player player0 = board.Players[0];
        Player player1 = board.Players[1];
        Player player2 = board.Players[2];

        Assert.AreEqual(new Hand(new List<int>() { 5, 5, 10 }, CardSuit.Hearts), player0.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 6, 7, 8 }, CardSuit.Hearts), player1.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.Hearts), player2.Hand);
    }
}