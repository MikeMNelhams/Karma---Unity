using DataStructures;
using Karma.Board;
using Karma.Cards;
using Karma.Controller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : IController
{
    public bool IsAwaitingInput {  get; set; }
    public FrozenMultiSet<CardValue> SelectedCardValues { get; set; }
    public BoardPlayerAction SelectedAction { get; set; }

    public PlayerController() 
    { 
        IsAwaitingInput = false;
        SelectedCardValues = new ();
    }

    IEnumerator WaitForInput()
    {

    }

    FrozenMultiSet<CardValue> GetCardSelection()
    {
        IsAwaitingInput = true;
        while (IsAwaitingInput)
        {

        }
        return SelectedCardValues;
    }

    public int GiveAwayCardIndex(IBoard board, HashSet<int> excludedCardsIndices)
    {
        throw new System.NotImplementedException();
    }

    public int GiveAwayPlayerIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        throw new System.NotImplementedException();
    }

    public int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        throw new System.NotImplementedException();
    }

    public bool WantsToMulligan(IBoard board)
    {
        throw new System.NotImplementedException();
    }

    public int MulliganHandIndex(IBoard board)
    {
        throw new System.NotImplementedException();
    }

    public int MulliganKarmaUpIndex(IBoard board)
    {
        throw new System.NotImplementedException();
    }

    public BoardTurnOrder ChooseStartDirection(IBoard board)
    {
        throw new System.NotImplementedException();
    }

    public BoardPlayerAction SelectAction(IBoard board)
    {
        IsAwaitingInput = true;
        while (IsAwaitingInput)
        {
            
        }
        return SelectedAction;
    }

    public FrozenMultiSet<CardValue> SelectCardsToPlay(IBoard board)
    {
        return GetCardSelection();
    }

    public int VoteForWinner(IBoard board, HashSet<int> excludedPlayerIndices)
    {
        throw new System.NotImplementedException();
    }
}