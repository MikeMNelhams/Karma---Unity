using System.Collections.Generic;
using NUnit.Framework;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Cards;

public class CardComboTenTests
{
    [Test]
    public void TenBurnsWholePlayPileOnly()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3, 4, 5, 11};
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 10 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.CurrentPlayerIndex);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(5, board.BurnPile.Count); // 4 play pile cards, PLUS the 10 itself
    }

    [Test]
    public void TenComboBurns()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3, 4, 5 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 10, 10, 10}, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(0, board.CurrentPlayerIndex);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(6, board.BurnPile.Count);
    }

    [Test]
    public void TenBurningComboStillBurns()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3, 4, 5 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 10, 10, 10, 10 }, CardSuit.Hearts); // Four 10s would ALSO burn

        board.PlayCards(cards, testController);

        Assert.AreEqual(0, board.CurrentPlayerIndex);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(7, board.BurnPile.Count);
    }

    [Test]
    public void TenComboSixFilledBurnsInCorrectOrder()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3, 4, 6 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();

        CardsList cards = new(new List<int>() { 10, 10, 6, 10, 10 }, CardSuit.Hearts);

        board.PlayCards(cards, testController);

        Assert.AreEqual(0, board.CurrentPlayerIndex);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(8, board.BurnPile.Count);

        PlayCardPile correctPile = new(new List<int>() { 3, 4, 6, 10, 10, 6, 10, 10 }, CardSuit.Hearts);

        Assert.AreEqual(correctPile, board.BurnPile);
    }
}
