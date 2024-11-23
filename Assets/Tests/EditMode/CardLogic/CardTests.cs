using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using DataStructures;
using System.Linq;

public class CardTests
{
    [Test]
    public void DifferentSuitCardsAreNotEqual()
    {
        Card card1 = new(CardValue.TWO, CardSuit.Hearts);
        Card card2 = new(CardValue.TWO, CardSuit.Diamonds);

        Assert.AreNotEqual(card1, card2);
    }

    [Test]
    public void DictionaryHashingDifferentSuitCards()
    {
        Card card1 = new(CardValue.TWO, CardSuit.Hearts);
        Card card2 = new(CardValue.TWO, CardSuit.Diamonds);

        Dictionary<Card, int> _dictionary = new()
        {
            { card1, 1 }
        };
        
        Assert.True(_dictionary.ContainsKey(card1));
        Assert.False(_dictionary.ContainsKey(card2));
    }

    [Test]
    public void FrozenMultiSetHashingDifferentSuitCards()
    {
        Card card1 = new(CardValue.TWO, CardSuit.Hearts);
        Card card2 = new(CardValue.TWO, CardSuit.Diamonds);

        FrozenMultiSet<Card> frozenMultiSet = new()
        {
            card1
        };
        Assert.True(frozenMultiSet.Contains(card1));
        Assert.False(frozenMultiSet.Contains(card2));
    }
}
