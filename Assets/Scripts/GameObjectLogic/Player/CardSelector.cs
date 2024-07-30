using DataStructures;
using KarmaLogic.Cards;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector
{
    public HashSet<CardObject> CardObjects { get; set; }
    protected FrozenMultiSet<Card> _selection;
    protected FrozenMultiSet<CardValue> _selectionValues;
    public CardsList Selection
    {
        get
        {
            CardsList selection = new ();
            foreach (Card card in _selection)
            {
                for (int i = 0; i < _selection[card]; i++)
                {
                    selection.Add(card);
                }
            }
            return selection;
        }
    }

    public FrozenMultiSet<CardValue> SelectionCardValues { get => _selectionValues; }

    public CardSelector()
    {
        CardObjects = new HashSet<CardObject>();
        _selection = new();
        _selectionValues = new();
    }

    public void Add(CardObject cardObject)
    {
        Card card = cardObject.CurrentCard;
        CardObjects.Add(cardObject);
        _selection.Add(card); 
        _selectionValues.Add(card.Value);
    }

    public void Remove(CardObject cardObject)
    {
        Card card = cardObject.CurrentCard;
        if (!CardObjects.Contains(cardObject)) { return; }
        CardObjects.Remove(cardObject);
        cardObject.DisableSelectShader();
        _selection.Remove(card); 
        _selectionValues.Remove(card.Value);
    }

    public void Toggle(CardObject cardObject)
    {
        if (CardObjects.Contains(cardObject))
        {
            Remove(cardObject);
        }
        else
        {
            Add(cardObject);
        }
        PrintSelectedCards();
    }

    public void Clear()
    {
        foreach (CardObject cardObject in CardObjects)
        {
            cardObject.DisableSelectShader();
        }
        CardObjects.Clear();
        _selection = new();
        _selectionValues = new();
    }

    void PrintSelectedCards()
    {
        Debug.Log("Selected: " + _selection);
    }
}
