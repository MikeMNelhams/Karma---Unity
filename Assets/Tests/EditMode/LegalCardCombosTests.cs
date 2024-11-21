using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using DataStructures;

public class LegalCardCombosTests 
{
    [Test]
    public void AllSingleCardsPlayableOnEmptyExceptJoker()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 15 }, new() { 14 }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };

        Assert.AreEqual(0, predictedLegalCombos.Count);
        Assert.False(predictedLegalCombos.Contains(combo));

        for (int i = 2; i < 15; i++)
        {
            playerCardValues = new()
            {
                new() { new() { i }, new() { }, new() { } }
            };

            drawCardValues = new() { };
            playCardValues = new() { };
            burnCardValues = new() { };

            board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

            board.StartTurn();

            predictedLegalCombos = board.CurrentLegalCombos;

            combo = new() { (CardValue)i };

            Assert.AreEqual(1, predictedLegalCombos.Count);
            Assert.True(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void AllCardsPlayableOnEmptyExceptJoker()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(13, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.False(predictedLegalCombos.Contains(combo));

        for (int i = 2; i < 15; i++)
        {
            combo = new() { (CardValue) i };
            Assert.True(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void TwoPlayableOnEverything()
    {
        for (int i = 2; i < 16; i++)
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2 }, new() { }, new() { } }
            };

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { i };
            List<int> burnCardValues = new() { };

            BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

            board.StartTurn();

            LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

            FrozenMultiSet<CardValue> combo = new() { CardValue.TWO };

            Assert.AreEqual(1, predictedLegalCombos.Count);
            Assert.True(predictedLegalCombos.Contains(combo));  
        }
    }

    [Test]
    public void OrderDependentCardsCannotBePlayedOnHigherRankUpwards()
    {
        List<int> orderDependentRanks = new() { 3, 5, 7, 8, 9, 10, 11, 12, 13 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(1, board.PlayPile.Count);
        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.ACE), board.PlayPile.VisibleTopCard);

        Assert.AreEqual(0, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo;

        foreach (int rank in orderDependentRanks)
        {
            combo = new() { (CardValue)rank };
            Assert.False(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void OrderDependentCardsCannotBePlayedOnLowerRankDownwards()
    {
        List<int> orderDependentRanks = new() { 3, 5, 7, 8, 9, 10, 11, 12, 13 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };  // It's impossible for 2 to be on the board when going downwards, but it allows for testing this case easily.
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(1, board.PlayPile.Count);
        Assert.AreEqual(new Card(CardSuit.DebugDefault, CardValue.TWO), board.PlayPile.VisibleTopCard);

        Assert.AreEqual(0, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo;

        foreach (int rank in orderDependentRanks)
        {
            combo = new() { (CardValue)rank };
            Assert.False(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void OrderDependentCardsCanBePlayedOnLowerRankUpwards()
    {
        List<int> orderDependentRanks = new() { 3, 5, 7, 8, 9, 10, 11, 12, 13 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(9, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo;

        foreach (int rank in orderDependentRanks)
        {
            combo = new() { (CardValue)rank };
            Assert.True(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void OrderDependentCardsCanBePlayedOnHigherRankDownwards()
    {
        List<int> orderDependentRanks = new() { 3, 5, 7, 8, 9, 10, 11, 12, 13 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(9, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo;

        foreach (int rank in orderDependentRanks)
        {
            combo = new() { (CardValue)rank };
            Assert.True(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void OrderDependentCardCombo()
    {
        List<int> orderDependentRanks = new() { 3, 3, 3, 3 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new () { CardValue.THREE};
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.THREE, CardValue.THREE };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.THREE, CardValue.THREE, CardValue.THREE };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.THREE, CardValue.THREE, CardValue.THREE, CardValue.THREE };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void TwoCardCombo()
    {
        List<int> testRanks = new() { 2, 2, 2, 2 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.TWO, CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.TWO, CardValue.TWO, CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void AcePlayableUpwards()
    {
        List<int> testRanks = new() { 14 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(1, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.ACE };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void AceNotPlayableDownwards()
    {
        List<int> testRanks = new() { 14 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(0, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.ACE };
        Assert.False(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void AceCardCombo()
    {
        List<int> testRanks = new() { 14, 14, 14, 14};

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.ACE };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.ACE, CardValue.ACE };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.ACE, CardValue.ACE, CardValue.ACE };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.ACE, CardValue.ACE, CardValue.ACE, CardValue.ACE };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerPlayableOnAceUpwards()
    {
        List<int> testRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(1, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerUnPlayableOnTwoWhenAcesInPlay()
    {
        List<int> testRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } },
            new() { new() { 14 } , new() { 2, 3 }, new() { 5 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(0, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.False(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerPlayableOnAceDownwards()
    {
        List<int> testRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(1, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerCardCombo()
    {
        List<int> testRanks = new() { 15, 15, 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(3, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.JOKER, CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerPlayableOnNonAceFlipped()
    {
        List<int> testRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14, 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, handsAreFlipped: true);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(1, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void FlippedHandsAlwaysLegalAndDoesNotAllowPlainCombos()
    {
        List<int> testRanks = new() { 3, 3, 7, 7 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 9 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, handsAreFlipped: true);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(2, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo;
        foreach (int rank in testRanks)
        {
            combo = new() { (CardValue)rank };
            Assert.True(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void SixPlayableAloneAsNonFiller()
    {
        List<int> testRanks = new() { 2, 3, 4, 6 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo;
        foreach (int rank in testRanks)
        {
            combo = new() { (CardValue)rank };
            Assert.True(predictedLegalCombos.Contains(combo));
        }
    }

    [Test]
    public void SixPlayableAsFillerAndAloneWhenPlayableAlone()
    {
        List<int> testRanks = new() { 2, 2, 2, 6 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(5, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.TWO, CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.TWO, CardValue.TWO, CardValue.TWO, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void SixPlayableAsFillerButNotAloneWhenUnplayableAlone()
    {
        List<int> testRanks = new() { 2, 2, 2, 6 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.TWO, CardValue.TWO, CardValue.TWO };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.TWO, CardValue.TWO, CardValue.TWO, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void SixPlayableAsJokerFillerWhenJokerPlayableButSixUnplayableAlone()
    {
        List<int> testRanks = new() { 6, 15, 15, 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.JOKER, CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.JOKER, CardValue.JOKER, CardValue.JOKER, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void SixPlayableAloneButNotAsJokerFillerWhenJokerUnplayableButSixPlayableAlone()
    {
        List<int> testRanks = new() { 6, 15, 15, 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { 14 };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new (playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(1, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void AllSixesNoUnplayableCombos()
    {
        List<int> testRanks = new() { 6, 6, 6, 6 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 3 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.SIX, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.SIX, CardValue.SIX, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.SIX, CardValue.SIX, CardValue.SIX, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void MoreSixThanMinorCardButStillValidFillerButNotValidFillerAlone()
    {
        List<int> testRanks = new() { 8, 8, 8, 6, 6, 6, 6 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(7, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.EIGHT };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.EIGHT, CardValue.EIGHT };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.EIGHT, CardValue.EIGHT, CardValue.EIGHT };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.EIGHT, CardValue.EIGHT, CardValue.EIGHT, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.EIGHT, CardValue.EIGHT, CardValue.EIGHT, CardValue.SIX, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.EIGHT, CardValue.EIGHT, CardValue.EIGHT, CardValue.SIX, CardValue.SIX, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.EIGHT, CardValue.EIGHT, CardValue.EIGHT, CardValue.SIX, CardValue.SIX, CardValue.SIX, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerSoloPlayableIfNoAces()
    {
        List<int> testRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(1, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerGroupPlayableIfNoAces()
    {
        List<int> testRanks = new() { 15, 15, 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(3, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.JOKER, CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
    }

    [Test]
    public void JokerGroupUnplayableIfNoAcesButDownwards()
    {
        List<int> testRanks = new() { 15, 15, 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues, boardPlayOrder: BoardPlayOrder.DOWN);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(0, predictedLegalCombos.Count);
    }

    [Test]
    public void JokerGroupSixFilledPlayableIfNoAces()
    {
        List<int> testRanks = new() { 15, 15, 15, 6 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { testRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoard board = new(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        LegalCombos predictedLegalCombos = board.CurrentLegalCombos;

        Assert.AreEqual(4, predictedLegalCombos.Count);

        FrozenMultiSet<CardValue> combo = new() { CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));

        combo = new() { CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.JOKER, CardValue.JOKER, CardValue.JOKER };
        Assert.True(predictedLegalCombos.Contains(combo));
        combo = new() { CardValue.JOKER, CardValue.JOKER, CardValue.JOKER, CardValue.SIX };
        Assert.True(predictedLegalCombos.Contains(combo));
    }
}
