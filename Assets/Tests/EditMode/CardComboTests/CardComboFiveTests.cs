using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;

public class CardComboFiveTests 
{
    /* NOT for testing when the 2 is Playable, only that its effect works when played */

    [Test]
    public void SingleFiveNoMultiplierNoDraw() 
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 5, 6, 7 }, new() { 9 }, new() { 12 } },
            new() { new() { 8, 8, 8}, new() { 10 }, new() { 13 } },
            new() { new() { 2, 3, 4}, new() { 11 }, new() { 14 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();
        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 0 });
        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);  // Resets coz it's a non effect multiplying increasing card
        Assert.False(board.HandsAreFlipped);

        Player player0 = board.Players[0];
        Player player1 = board.Players[1];
        Player player2 = board.Players[2];

        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), player0.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 6, 7 }, CardSuit.DebugDefault), player1.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 8, 8, 8 }, CardSuit.DebugDefault), player2.Hand);

        Assert.AreEqual(new CardsList(new List<int>() { 9 }, CardSuit.DebugDefault), player0.KarmaUp);
        Assert.AreEqual(new CardsList(new List<int>() { 10 }, CardSuit.DebugDefault), player1.KarmaUp);
        Assert.AreEqual(new CardsList(new List<int>() { 11 }, CardSuit.DebugDefault), player2.KarmaUp);

        Assert.AreEqual(new CardsList(new List<int>() { 12 }, CardSuit.DebugDefault), player0.KarmaDown);
        Assert.AreEqual(new CardsList(new List<int>() { 13 }, CardSuit.DebugDefault), player1.KarmaDown);
        Assert.AreEqual(new CardsList(new List<int>() { 14 }, CardSuit.DebugDefault), player2.KarmaDown);
    }

    [Test]
    public void MultipleFivesLessThanPlayerCountNoDraw()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 5, 5, 7 }, new() { 9 }, new() { 12 } },
            new() { new() { 8, 8, 8}, new() { 10 }, new() { 13 } },
            new() { new() { 2, 3, 4}, new() { 11 }, new() { 14 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();
        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 0, 1 });
        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);  // Resets coz it's a non effect multiplying increasing card
        Assert.False(board.HandsAreFlipped);

        Player player0 = board.Players[0];
        Player player1 = board.Players[1];
        Player player2 = board.Players[2];

        Assert.AreEqual(new Hand(new List<int>() { 8, 8, 8 }, CardSuit.DebugDefault), player0.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), player1.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 7 }, CardSuit.DebugDefault), player2.Hand);
    }

    [Test]
    public void MultipleFivesEqualToPlayerCountNoDraw()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 5, 5, 5, 7 }, new() { 9 }, new() { 12 } },
            new() { new() { 8, 8, 8}, new() { 10 }, new() { 13 } },
            new() { new() { 2, 3, 4}, new() { 11 }, new() { 14 } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();
        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 0, 1, 2 });
        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);  // Resets coz it's a non effect multiplying increasing card
        Assert.False(board.HandsAreFlipped);

        Player player0 = board.Players[0];
        Player player1 = board.Players[1];
        Player player2 = board.Players[2];

        Assert.AreEqual(new Hand(new List<int>() { 7 }, CardSuit.DebugDefault), player0.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 8, 8, 8 }, CardSuit.DebugDefault), player1.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), player2.Hand);
    }

    [Test]
    public void SingleFiveWithDraw()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 5, 6, 7 }, new() { 9 }, new() { 12 } },
            new() { new() { 8, 8, 8}, new() { 10 }, new() { 13 } },
            new() { new() { 2, 3, 4}, new() { 11 }, new() { 14 } }
        };

        List<int> drawCardValues = new() { 15 };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        PlayerController testController = new();
        CardsList cards = board.Players[0].Hand.PopMultiple(new int[] { 0 });
        board.PlayCards(cards, testController);

        Assert.AreEqual(BoardPlayOrder.UP, board.PlayOrder);
        Assert.AreEqual(BoardTurnOrder.RIGHT, board.TurnOrder);
        Assert.AreEqual(1, board.EffectMultiplier);  // Resets coz it's a non effect multiplying increasing card
        Assert.False(board.HandsAreFlipped);

        Player player0 = board.Players[0];
        Player player1 = board.Players[1];
        Player player2 = board.Players[2];

        Assert.AreEqual(new Hand(new List<int>() { 2, 3, 4 }, CardSuit.DebugDefault), player0.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 6, 7, 15 }, CardSuit.DebugDefault), player1.Hand);
        Assert.AreEqual(new Hand(new List<int>() { 8, 8, 8 }, CardSuit.DebugDefault), player2.Hand);
    }
}
