using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using DataStructures;

public class LegalMovesTests
{
    [Test]
    public void EmptyBoardEmptySelectablePredictsNoLegalActions()
    {
        List<int> orderDependentRanks = new() { };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        Assert.AreEqual(0, board.CurrentLegalActions.Count);
    }

    [Test]
    public void PlayableCardComboEmptyBoard()
    {
        List<int> orderDependentRanks = new() { 3 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        HashSet<BoardPlayerAction> predictedPlayerActions = board.CurrentLegalActions;

        Assert.AreEqual(1, predictedPlayerActions.Count);
        Assert.True(predictedPlayerActions.Contains(new PlayCardsCombo()));
    }

    [Test]
    public void UnPlayableCardComboEmptyBoard()
    {
        List<int> orderDependentRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { 14 }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        HashSet<BoardPlayerAction> predictedPlayerActions = board.CurrentLegalActions;

        Assert.AreEqual(0, predictedPlayerActions.Count);
    }

    [Test]
    public void NoCardsButPickupPileHasCards()
    {
        // The player has WON so shouldn't be able to pickup!
        List<int> orderDependentRanks = new() { };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        HashSet<BoardPlayerAction> predictedPlayerActions = board.CurrentLegalActions;

        Assert.AreEqual(0, predictedPlayerActions.Count);
    }

    [Test]
    public void UnPlayableCardComboButPickupPileHasCards()
    {
        List<int> orderDependentRanks = new() { 15 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 14, 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        HashSet<FrozenMultiSet<CardValue>> predictedCombos = board.CurrentLegalCombos;

        UnityEngine.Debug.Log("predicted combos: " + predictedCombos);

        HashSet<BoardPlayerAction> predictedPlayerActions = board.CurrentLegalActions;

        Assert.AreEqual(1, predictedPlayerActions.Count);
        Assert.True(predictedPlayerActions.Contains(new PickupPlayPile()));
    }

    [Test]
    public void PlayableCardComboAndPickupPileHasCards()
    {
        List<int> orderDependentRanks = new() { 3 };

        List<List<List<int>>> playerCardValues = new()
        {
            new() { orderDependentRanks, new() { }, new() { } }
        };

        List<int> drawCardValues = new() { };
        List<int> playCardValues = new() { 2 };
        List<int> burnCardValues = new() { };

        BasicBoard board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues);

        board.StartTurn();

        HashSet<BoardPlayerAction> predictedPlayerActions = board.CurrentLegalActions;

        Assert.AreEqual(2, predictedPlayerActions.Count);
        Assert.True(predictedPlayerActions.Contains(new PlayCardsCombo()));
        Assert.True(predictedPlayerActions.Contains(new PickupPlayPile()));
    }
}
