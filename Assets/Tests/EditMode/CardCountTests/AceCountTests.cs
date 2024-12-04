using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Cards;

public class AceCountTests
{
    [Test]
    public void AceCountEmptyBoard()
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

        Assert.AreEqual(0, board.CardValuesInPlayCounts[CardValue.ACE]);
        Assert.AreEqual(0, board.CardValuesTotalCounts[CardValue.ACE]);
    }

    [Test]
    public void AceCountIsCorrectAtStartNormalBoard()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 11, 14}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 14 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14, 2, 3, 4, 5 };
        List<int> burnCardValues = new() { 14 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        Assert.AreEqual(3, board.CardValuesInPlayCounts[CardValue.ACE]);
        Assert.AreEqual(4, board.CardValuesTotalCounts[CardValue.ACE]);
    }

    [Test]
    public void AceCountAfterPlayingAceStaysSame()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 11, 14}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 14 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14, 2, 3, 4 };
        List<int> burnCardValues = new() { 14 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.Players[0].PopFromPlayable(new int[] { 2 });

        Assert.AreEqual(3, board.CardValuesInPlayCounts[CardValue.ACE]);

        board.PlayCards(cards);

        Assert.AreEqual(3, board.CardValuesInPlayCounts[CardValue.ACE]);
    }

    [Test]
    public void AceCountInPlayAfterBurningAceDecreases()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 3, 11, 14}, new() { 8 }, new() { 9 } },
            new() { new() { 2 }, new() { 3 }, new() { 14 } },
            new() { new() { 5 }, new() { 6 }, new() { 7 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14, 2, 3, 4, 14, 14, 14 };
        List<int> burnCardValues = new() { 14 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.Players[0].PopFromPlayable(new int[] { 2 });

        Assert.AreEqual(6, board.CardValuesInPlayCounts[CardValue.ACE]);

        board.PlayCards(cards);

        Assert.AreEqual(1, board.CardValuesInPlayCounts[CardValue.ACE]);
    }
}
