using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);
        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(1, board.PlayPile.Count);
        Assert.AreEqual(new Card(CardValue.JACK, CardSuit.DebugDefault), board.PlayPile[0]);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(2, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new (new List<int>() { 2, 11 }, CardSuit.DebugDefault);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(4, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 11 }, CardSuit.DebugDefault);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11, 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(8, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 11, 11 }, CardSuit.DebugDefault);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11, 11, 6, 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(32, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 11, 11, 6, 11 }, CardSuit.DebugDefault);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 4, 11 }, CardSuit.DebugDefault);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11, 11, 6, 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 3, 4, 11, 11, 6, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(new Card(CardValue.THREE, CardSuit.DebugDefault), board.PlayPile.VisibleTopCard);
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

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, effectMultiplier: 2, whoStarts: 1);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = board.Players[1].PopFromPlayable(new int[] { 2 });
        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.False(board.HandsAreFlipped);

        Assert.AreEqual(1, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 5, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Player player0 = board.Players[0];
        Player player1 = board.Players[1];
        Player player2 = board.Players[2];

        Assert.AreEqual(new Hand(new List<int>() { 5, 5, 10 }, CardSuit.DebugDefault), player0.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 6, 7, 8 }, CardSuit.DebugDefault), player1.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), player2.Hand);
    }

    [Test]
    public void JackOnSixDoesNothing()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 6 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, effectMultiplier: 2, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 6, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackOnSeven()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 7 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);
        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 7, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackOnEight()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 8 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 8, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackOnNine()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 9 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 9, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        board.StepPlayerIndex(1);
        board.EndTurn();

        Assert.AreEqual(2, board.CurrentPlayerIndex);
    }

    [Test]
    public void JackOnTen()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 10 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.PlayPile.Count);

        CardPile expectedBurnPile = new(new List<int>() { 10, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedBurnPile, board.BurnPile);

        Assert.AreEqual(0, board.CurrentPlayerIndex);
    }

    [Test]
    public void JackOnJackOnNonJackDoesNothing()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2, 11 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(3, board.PlayPile.Count);

        CardPile expectedPlayPile = new(new List<int>() { 2, 11, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);
    }

    [Test]
    public void JackOnQueenNoCardsToGiveAway()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 12 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.Null(board.Players[0].CardGiveAwayHandler);
    }

    [Test]
    public void JackOnQueenCardsToGiveAway()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { 10, 11, 12 }, new() { } },
            new() { new() { }, new() { 2 }, new() { } },
            new() { new() { }, new() { 3 }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 12 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.NotNull(board.Players[0].CardGiveAwayHandler);
        Assert.AreEqual(1, board.Players[0].CardGiveAwayHandler.NumberOfCardsRemainingToGiveAway);
    }

    [Test]
    public void JackOnKingWithEmptyBurnPileDoesNothing()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 13 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);
        Assert.AreEqual(2, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 13, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

    }

    [Test]
    public void JackOnKingPlaysBottomOfBurnPile()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 13 };
        List<int> burnCardValues = new() { 2, 7 };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT, handsAreFlipped: true);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(1, board.BurnPile.Count);
        Assert.AreEqual(3, board.PlayPile.Count);

        PlayCardPile expectedPlayPile = new(new List<int>() { 13, 11, 2 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);  // It should play the two ONLY
    }

    [Test]
    public void JackOnAce()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } },
            new() { new() { }, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14 };
        List<int> burnCardValues = new() { };

        BasicBoardParams boardParams = BasicBoardParams.AllBots(playerCardValues, drawCardValues, playCardValues, burnCardValues, 
            playOrder: BoardPlayOrder.DOWN, turnOrder: BoardTurnOrder.LEFT);
        BasicBoard board = new(boardParams);

        board.StartTurn();

        CardsList cards = new(new List<int>() { 11 }, CardSuit.DebugDefault);

        board.PlayCards(cards);

        Assert.AreEqual(BoardPlayOrder.DOWN, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.LEFT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);
        Assert.True(board.HandsAreFlipped);

        Assert.AreEqual(0, board.DrawPile.Count);
        Assert.AreEqual(0, board.BurnPile.Count);

        CardPile expectedPlayPile = new(new List<int>() { 14, 11 }, CardSuit.DebugDefault);
        Assert.AreEqual(expectedPlayPile, board.PlayPile);

        Assert.AreEqual(0, board.CurrentPlayerIndex);
    }
}
