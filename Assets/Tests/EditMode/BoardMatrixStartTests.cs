using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;

public class BoardMatrixStartTests
{
    [Test]
    public void Empty()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(new PlayCardPile(), board.PlayPile);
        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(new CardPile(), board.DrawPile);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(new CardPile(), board.BurnPile);

        Assert.AreEqual(4, board.Players.Count);

        for (int i = 0; i < board.Players.Count; i++)
        {
            Assert.AreEqual(0, board.Players[i].PlayableCards.Count);
        }
    }

    [Test]
    public void DifferentExclusivePlayableCards()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 2, 3, 4 }, new() { }, new() { } },
            new() { new() {  }, new() { 5, 6, 7 }, new() { } },
            new() { new() {  }, new() { }, new() { 8, 9, 10 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(new PlayCardPile(), board.PlayPile);
        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(new CardPile(), board.DrawPile);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(new CardPile(), board.BurnPile);

        Assert.AreEqual(3, board.Players.Count);

        Assert.AreEqual(PlayingFrom.Hand, board.Players[0].PlayingFrom);
        Assert.AreEqual(PlayingFrom.KarmaUp, board.Players[1].PlayingFrom);
        Assert.AreEqual(PlayingFrom.KarmaDown, board.Players[2].PlayingFrom);

        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), board.Players[0].Hand);
        Assert.AreEqual(new Hand(new List<int>() { 5, 6, 7 }, CardSuit.DebugDefault), board.Players[1].KarmaUp);
        Assert.AreEqual(new Hand(new List<int>() { 8, 9, 10 }, CardSuit.DebugDefault), board.Players[2].KarmaDown);

        Assert.AreEqual(0, board.Players[0].KarmaUp.Count);
        Assert.AreEqual(0, board.Players[0].KarmaDown.Count);

        Assert.AreEqual(0, board.Players[1].Hand.Count);
        Assert.AreEqual(0, board.Players[1].KarmaDown.Count);

        Assert.AreEqual(0, board.Players[2].Hand.Count);
        Assert.AreEqual(0, board.Players[2].KarmaUp.Count);
    }

    [Test]
    public void DifferentCombinedStarts()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 2 }, new() { 3, 3 }, new() { } },
            new() { new() { 4, 4, 4 }, new() { }, new() { 5, 5, 5 } },
            new() { new() {  }, new() { 6, 6, 6 }, new() { 7, 7, 7 } },
            new() { new() { 8, 8, 8 }, new() { 9, 9, 9 }, new() { 10, 10, 10 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        Assert.AreEqual(0, board.PlayPile.Count);
        Assert.AreEqual(new PlayCardPile(), board.PlayPile);
        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(new CardPile(), board.DrawPile);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(new CardPile(), board.BurnPile);

        Assert.AreEqual(4, board.Players.Count);

        Assert.AreEqual(PlayingFrom.Hand, board.Players[0].PlayingFrom);
        Assert.AreEqual(PlayingFrom.Hand, board.Players[1].PlayingFrom);
        Assert.AreEqual(PlayingFrom.KarmaUp, board.Players[2].PlayingFrom);
        Assert.AreEqual(PlayingFrom.Hand, board.Players[3].PlayingFrom);

        Assert.AreEqual(new Hand(new List<int>() { 2 }, CardSuit.DebugDefault), board.Players[0].Hand);
        Assert.AreEqual(new CardsList(new List<int>() { 3, 3 }, CardSuit.DebugDefault), board.Players[0].KarmaUp);
        Assert.AreEqual(0, board.Players[0].KarmaDown.Count);

        Assert.AreEqual(new Hand(new List<int>() { 4, 4, 4 }, CardSuit.DebugDefault), board.Players[1].Hand);
        Assert.AreEqual(0, board.Players[1].KarmaUp.Count);
        Assert.AreEqual(new CardsList(new List<int>() { 5, 5, 5 }, CardSuit.DebugDefault), board.Players[1].KarmaDown);

        Assert.AreEqual(0, board.Players[2].Hand.Count);
        Assert.AreEqual(new CardsList(new List<int>() { 6, 6, 6 }, CardSuit.DebugDefault), board.Players[2].KarmaUp);
        Assert.AreEqual(new CardsList(new List<int>() { 7, 7, 7 }, CardSuit.DebugDefault), board.Players[2].KarmaDown);

        Assert.AreEqual(new Hand(new List<int>() { 8, 8, 8 }, CardSuit.DebugDefault), board.Players[3].Hand);
        Assert.AreEqual(new CardsList(new List<int>() { 9, 9, 9 }, CardSuit.DebugDefault), board.Players[3].KarmaUp);
        Assert.AreEqual(new CardsList(new List<int>() { 10, 10, 10 }, CardSuit.DebugDefault), board.Players[3].KarmaDown);
    }

    [Test]
    public void Piles()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        List<int> playCardValues = new() { 11, 12, 13, 14};
        List<int> burnCardValues = new() { 15, 15, 15};

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        // 6 cards should be drawn! (3 per player)
        Assert.AreEqual(new CardPile(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), board.DrawPile);

        Assert.AreEqual(new PlayCardPile(new List<int>() { 11, 12, 13, 14 }, CardSuit.DebugDefault), board.PlayPile);

        Assert.AreEqual(new CardPile(new List<int>() { 15, 15, 15 }, CardSuit.DebugDefault), board.BurnPile);

        Assert.AreEqual(2, board.Players.Count);
    }
}
