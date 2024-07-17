using System.Collections.Generic;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;

public class CardSuitTests 
{
    [Test]
    public void DifferentSuitsNotEqual()
    {
        Assert.AreNotEqual(CardSuit.Hearts, CardSuit.Diamonds);
        Assert.AreNotEqual(CardSuit.Hearts, CardSuit.Clubs);
        Assert.AreNotEqual(CardSuit.Hearts, CardSuit.Spades);

        Assert.AreNotEqual(CardSuit.Diamonds, CardSuit.Clubs);
        Assert.AreNotEqual(CardSuit.Diamonds, CardSuit.Spades);

        Assert.AreNotEqual(CardSuit.Spades, CardSuit.Clubs);
    }

    [Test]
    public void SameSuitsAreEqual()
    {
        Assert.AreEqual(CardSuit.DebugDefault, CardSuit.DebugDefault);
        Assert.AreEqual(CardSuit.Hearts, CardSuit.Hearts);
        Assert.AreEqual(CardSuit.Diamonds, CardSuit.Diamonds);
        Assert.AreEqual(CardSuit.Spades, CardSuit.Spades);
        Assert.AreEqual(CardSuit.Clubs, CardSuit.Clubs);    
    }
}
