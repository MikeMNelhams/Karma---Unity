using Karma.Cards;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector
{
    HashSet<Card> _selection;

    public CardSelector()
    {
        _selection = new();
    }

    public void Add(GameObject cardObject)
    {
        CardObject renderer = cardObject.GetComponent<CardObject>();
        Card card = renderer.CurrentCard;
        Add(card);
    }

    void Add(Card card) { _selection.Add(card); }

    public void Remove(GameObject cardObject)
    {
        CardObject renderer = cardObject.GetComponent<CardObject>();
        Card card = renderer.CurrentCard;
        Remove(card);
    }

    void Remove(Card card) { _selection.Remove(card); }

    public void Toggle(GameObject cardObject)
    {
        CardObject renderer = cardObject.GetComponent<CardObject>();
        Card card = renderer.CurrentCard;
        Toggle(card);
    }

    public void Toggle(Card card)
    {
        Debug.Log("Toggling selection of: " + card);
        if (_selection.Contains(card))
        {
            Remove(card);
        }
        else
        {
            Add(card);
        }
        Debug.Log("Currently selected #: " + _selection.Count);
        Debug.Log("-------------");
    }
}
