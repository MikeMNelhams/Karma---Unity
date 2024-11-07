using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardComboKingTests
{
    [Test]
    public void KingWithEmptyBurnPileDoesNothing()
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

        CardsList cards = new(new List<int>() { 13 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(1, board.PlayPile.Count);
        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.KING), board.PlayPile[0]);
    }

    [Test]
    public void KingPlaysBottomOfBurnPile()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { 2, 7 };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 13 }, CardSuit.DebugDefault);

        board.PlayCards(cards);
        
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);
        Assert.AreEqual(2, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 13, 2 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);  // It should play the two ONLY
    }

    [Test]
    public void KingPlaysBottomOfBurnPileJackPlaysKing()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { 11, 2 };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN, boardTurnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 13 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(3, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 13, 11, 2 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
    }

    [Test]
    public void KingPlaysKingThenBurns()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 13, 13};
        List<int> burnCardValues = new() { 13 };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 13 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(4, board.BurnPile.Count);
        Assert.AreEqual(0, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void KingGroupComboSixFilledDoesNotLoopInfinitely()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() {  };
        List<int> burnCardValues = new() { 13, 13 };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 13, 13, 6, 13 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(6, board.PlayPile.Count);
        
        PlayCardPile expectedPlayPile = new(new List<int>() { 13, 13, 6, 13, 13, 13 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }
}
