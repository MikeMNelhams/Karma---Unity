using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class JokerCountTests
{
    [Test]
    public void JokerCountEmptyBoard()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        Assert.AreEqual(0, board.CardValuesInPlayCounts[CardValue.JOKER]);
        Assert.AreEqual(0, board.CardValuesTotalCounts[CardValue.JOKER]);
    }

    [Test]
    public void JokerCountIsCorrectAtStartNormalBoard()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 11, 15}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 15 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 15, 2, 3, 4, 5 };
        List<int> burnCardValues = new() { 15 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        Assert.AreEqual(3, board.CardValuesInPlayCounts[CardValue.JOKER]);
        Assert.AreEqual(4, board.CardValuesTotalCounts[CardValue.JOKER]);
    }

    [Test]
    public void JokerCountAfterPlayingJokerDecreases()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 11, 15}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 15 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 15, 2, 3, 4, 14 };
        List<int> burnCardValues = new() { 15 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.Players[0].PopFromPlayable(new int[] { 2 });

        board.PlayCards(cards);

        Assert.AreEqual(2, board.CardValuesInPlayCounts[CardValue.JOKER]);
    }

    [Test]
    public void JokerCountInPlayAfterPlayingJokerGroupComboDecreases()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 11, 15, 15, 15}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 15 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 15, 2, 3, 4, 14 };
        List<int> burnCardValues = new() { 15 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.Players[0].PopFromPlayable(new int[] { 2, 3, 4 });

        board.PlayCards(cards);

        Assert.AreEqual(2, board.CardValuesInPlayCounts[CardValue.JOKER]);
    }

    [Test]
    public void JokerCountInPlayAfterPlayingKingPlaysJokerThenReBurnsSameJoker()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 13, 15}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 15 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2, 3, 4, 11 };
        List<int> burnCardValues = new() { 15 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.Players[0].PopFromPlayable(new int[] { 1 });

        board.PlayCards(cards);

        Assert.AreEqual(2, board.CardValuesInPlayCounts[CardValue.JOKER]); // It should increase to 3 then back to 3.
        Assert.AreEqual(1, board.BurnPile.Count);
    }
}
