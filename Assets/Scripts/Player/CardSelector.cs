using DataStructures;
using Karma.Cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSelector
{
    protected FrozenMultiSet<Card> _selection;
    protected FrozenMultiSet<CardValue> _selectionValues;
    public FrozenMultiSet<Card> Selection { get => _selection; }
    public FrozenMultiSet<CardValue> SelectionCardValues { get => _selectionValues; }

    public CardSelector()
    {
        _selection = new();
        _selectionValues = new();
    }

    public void Add(GameObject cardObject)
    {
        CardObject renderer = cardObject.GetComponent<CardObject>();
        Card card = renderer.CurrentCard;
        Add(card);
    }

    void Add(Card card) { _selection.Add(card); _selectionValues.Add(card.value); }

    public void Remove(GameObject cardObject)
    {
        CardObject renderer = cardObject.GetComponent<CardObject>();
        Card card = renderer.CurrentCard;
        Remove(card);
    }

    void Remove(Card card) { _selection.Remove(card); _selectionValues.Remove(card.value);  }

    public void Toggle(GameObject cardObject)
    {
        CardObject renderer = cardObject.GetComponent<CardObject>();
        Card card = renderer.CurrentCard;
        Toggle(card);
    }

    public void Toggle(Card card)
    {
        if (_selection.Contains(card))
        {
            Remove(card);
        }
        else
        {
            Add(card);
        }
        PrintSelectedCards();
    }

    void PrintSelectedCards()
    {
        string selected = "Selected: ";
        foreach (Card selectedCard in _selection)
        {
            selected += selectedCard + " ";
        }
        Debug.Log(selected);
    }
}
