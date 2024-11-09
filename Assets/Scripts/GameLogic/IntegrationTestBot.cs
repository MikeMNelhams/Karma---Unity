using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Bots;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using System.Linq;
using KarmaLogic.BasicBoard;
using DataStructures;


public class IntegrationTestBot : IBot
{
    public string Name { get; protected set;}
    public float DelaySeconds { get; protected set; }
    protected List<BoardPlayerAction> _knownActions = new ();

    public IntegrationTestBot(string name, float delay)
    {
        Name = name;
        DelaySeconds = delay;
        _knownActions.Add(new PickupPlayPile());
        _knownActions.Add(new PlayCardsCombo());
    }

    public bool IsReady(IBoard board)
    {
        return board is not null;
    }

    public int CardGiveAwayIndex(IBoard board)
    {
        CardsList playableCards = board.CurrentPlayer.PlayableCards;
        List<CardValue> cardValues = playableCards.CardValues;
        HashSet<int> legalIndices = new();
        for (int i = 0; i < cardValues.Count; i++)
        {
            CardValue cardValue = cardValues[i];
            if (cardValue != CardValue.JOKER)
            {
                legalIndices.Add(i);
            }
        }
        return legalIndices.First();
    }

    public int CardPlayerGiveAwayIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        HashSet<int> legalIndices = OtherPlayerIndices(board);
        legalIndices.ExceptWith(excludedPlayerIndices);
        return legalIndices.First();
    }

    public FrozenMultiSet<CardValue> ComboToPlay(IBoard board)
    {
        LegalCombos legalCombos = board.CurrentLegalCombos;
        return legalCombos.First();
    }

    public int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        HashSet<int> potentialWinnerIndices = board.PotentialWinnerIndices;
        potentialWinnerIndices.ExceptWith(excludedPlayerIndices);
        if (potentialWinnerIndices.Count > 0)
        {
            UnityEngine.Debug.Log("Bot voting tactically");
            return potentialWinnerIndices.ToList<int>()[0];
        }
        HashSet<int> otherPlayerIndices = OtherPlayerIndices(board);
        otherPlayerIndices.ExceptWith(excludedPlayerIndices);
        return otherPlayerIndices.ToList<int>()[0];
    }

    public int MulliganHandIndex(IBoard board)
    {
        throw new System.NotImplementedException();
    }

    public int MulliganKarmaUpIndex(IBoard board)
    {
        throw new System.NotImplementedException();
    }

    public BoardTurnOrder PreferredStartDirection(IBoard board)
    {
        return BoardTurnOrder.RIGHT;
    }

    public BoardPlayerAction SelectAction(IBoard board)
    {
        foreach (BoardPlayerAction action in _knownActions)
        {
            if (board.CurrentLegalActions.Contains(action))
            {
                return action;
            }
        }

        throw new NoValidBoardPlayerActionsException();
    }

    public int VoteForWinnerIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        HashSet<int> potentialWinners = board.PotentialWinnerIndices;
        potentialWinners.ExceptWith(excludedPlayerIndices);
        List<int> potentialWinnerIndices = potentialWinners.ToList<int>();
        return potentialWinnerIndices[0];
    }

    public bool WantsToMulligan(IBoard board)
    {
        return false;
    }

    HashSet<int> OtherPlayerIndices(IBoard board)
    {
        HashSet<int> otherPlayers = Enumerable.Range(0, board.Players.Count).ToHashSet();
        otherPlayers.Remove(board.CurrentPlayerIndex);
        return otherPlayers;
    }
}
