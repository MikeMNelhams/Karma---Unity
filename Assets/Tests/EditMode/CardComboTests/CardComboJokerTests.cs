using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardComboJokerTests
{
    [Test]
    public void JokerGivesAwayCardAndBurnsJokerOnly()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { 2 }, new() { 3 }, new() { 4 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2, 3, 4, 5 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 15 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);
        Assert.AreEqual(4, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 2, 3, 4, 5 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.JOKER), board.BurnPile[0]);
    }

    [Test]
    public void JokerOnEmpty()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { 2 }, new() { 3 }, new() { 4 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 15 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);
        Assert.AreEqual(0, board.PlayPile.Count);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.JOKER), board.BurnPile[0]);
    }

    [Test]
    public void JokerGroupCombo()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { 2 }, new() { 3 }, new() { 4 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 15, 15, 15 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(3, board.BurnPile.Count);
        Assert.AreEqual(1, board.PlayPile.Count);

        PlayCardPile expectedBurnPile = new(new List<int>() { 15, 15, 15}, CardSuit.DebugDefault);
        Assert.AreEqual(expectedBurnPile, board.BurnPile);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.ACE), board.PlayPile[0]);
    }

    [Test]
    public void JokerGroupComboSixFiller()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { 2 }, new() { 3 }, new() { 4 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 15, 15, 6, 15 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(4, board.BurnPile.Count);
        Assert.AreEqual(1, board.PlayPile.Count);

        PlayCardPile expectedBurnPile = new(new List<int>() { 15, 15, 6, 15 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedBurnPile, board.BurnPile);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.ACE), board.PlayPile[0]);
    }

    [Test]
    public void JokerStillValidToTargetWinners()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 15 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);
        Assert.AreEqual(1, board.PlayPile.Count);

        PlayCardPile expectedBurnPile = new(new List<int>() { 15 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedBurnPile, board.BurnPile);

        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.ACE), board.PlayPile[0]);
    }
}
