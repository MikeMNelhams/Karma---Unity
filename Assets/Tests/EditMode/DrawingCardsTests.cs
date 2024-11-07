using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using DataStructures;

public class DrawingCardsTests 
{
    [Test]
    public void DrawCardsUntilFullAtStart()
    {
        List<int> testRanks = new() { 2 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 3, 4, 5 };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();
        
        Hand correctHand = new (new List<int> { 2, 4, 5 }, CardSuit.DebugDefault);
        Assert.AreEqual(correctHand, board.CurrentPlayer.Hand);
    }

    [Test]
    public void DrawCardsUntilFullAtStartDoesNotOverDraw()
    {
        List<int> testRanks = new() { 2, 2, 2 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 3, 4, 5 };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        Hand correctHand = new(new List<int> { 2, 2, 2 }, CardSuit.DebugDefault);
        Assert.AreEqual(correctHand, board.CurrentPlayer.Hand);
    }

    [Test]
    public void DrawCardsAfterPlayingGroupCombo()
    {
        List<int> testRanks = new() { 2, 2, 2 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 3, 4, 5 };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 0, 1 });
        board.PlayCards(cards);

        Hand correctHand = new(new List<int> { 2, 4, 5 }, CardSuit.DebugDefault);
        Assert.AreEqual(correctHand, board.CurrentPlayer.Hand);
    }

    [Test]
    public void DrawCardsAfterBurningJoker()
    {
        List<int> testRanks = new() { 2, 2, 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 3, 4, 5 };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, handsAreFlipped: true);

        board.StartTurn();

        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 2 });
        board.PlayCards(cards);

        Hand correctHand = new(new List<int> { 2, 2, 5 }, CardSuit.DebugDefault);
        Assert.AreEqual(correctHand, board.CurrentPlayer.Hand);
    }

    [Test]
    public void DrawCardsAfterPlayingTenToBurnPlayPile()
    {
        List<int> testRanks = new() { 2, 2, 10 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 3, 4 };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, handsAreFlipped: true);

        board.StartTurn();

        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 2 });
        board.PlayCards(cards);

        Hand correctHand = new(new List<int> { 2, 2, 4 }, CardSuit.DebugDefault);
        Assert.AreEqual(correctHand, board.CurrentPlayer.Hand);
    }

    [Test]
    public void DrawCardsAfterBurningGroupCombo()
    {
        List<int> testRanks = new() { 2, 2, 2, 2 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } },
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 3, 4 };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, handsAreFlipped: true);

        board.StartTurn();

        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 0, 1, 2, 3 });
        board.PlayCards(cards);

        Hand correctHand = new(new List<int> { 3, 4 }, CardSuit.DebugDefault);
        Assert.AreEqual(correctHand, board.CurrentPlayer.Hand);
    }
}
