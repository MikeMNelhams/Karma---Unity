using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

namespace Karma
{
    namespace Cards
    {
        public class CardPile : CardsList
        {
            public CardPile() : base(){ }
            public CardPile(List<Card> cards) : base(cards) { }
            public CardPile(CardsList cards) : base(cards.ToList()) { }
            public CardPile(List<int> cardValues, CardSuit suit) : base(cardValues, suit) { }
            
            public virtual CardsList RemoveFromBottom(int splitIndex)
            {
                CardsList removedCards = new();
                for (int i = splitIndex; i < _cards.Count; i++)
                {
                    removedCards.Add(_cards[i]);
                }
                _cards.RemoveRange(0, splitIndex);
                return removedCards;
            }

            public static CardPile Empty
            {
                get
                {
                    return new CardPile();
                }
            }
        }

        public class PlayCardPile : CardPile
        {
            public List<bool> Visibles { get; protected set; }
            public PlayCardPile() : base() { Visibles = CheckVisibles(); }
            public PlayCardPile(List<int> cardValues, CardSuit suit) : base(cardValues, suit) { Visibles = CheckVisibles(); }

#nullable enable
            public Card? VisibleTopCard
#nullable disable
            {
                get
                {
                    if (Visibles.Count == 0) { return null; }
                    if (Visibles.Count == 1) 
                    { 
                        if (Visibles[0]) { return _cards[0]; }
                        return null;
                    }
                    for (int i=Visibles.Count - 1; i > 0; i--)
                    {
                        if (Visibles[i])
                        {
                            return _cards[i];
                        }
                    }
                    return null;
                }
            }

            public void Add(Card card, bool isVisible)
            {
                base.Add(card);
                Visibles.Add(isVisible);
            }

            public override void Add(Card card)
            {
                base.Add(card);
                bool isVisible = true;
                if (card.value == CardValue.FOUR)
                {
                    isVisible = false;
                }
                Visibles.Add(isVisible);
            }

            public void Add(CardsList cards, List<bool> visibilities)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    Add(cards[i], visibilities[i]);
                }
            }

            public override void Add(CardsList cards)
            {
                List<bool> visibilities = new();
                foreach (Card card in cards)
                {
                    if (card.value != CardValue.FOUR) 
                    {
                        visibilities.Add(true);
                    }
                    else
                    {
                        visibilities.Add(false);
                    }
                }
                Add(cards, visibilities);
            }

            public override Card Pop()
            {
                Visibles.RemoveAt(Visibles.Count - 1);
                return base.Pop();
            }

            public override Card Pop(int index)
            {
                Visibles.RemoveAt(index);
                return base.Pop(index);
            }

            public override CardsList PopMultiple(int[] indices)
            {
                HashSet<int> excludedIndices = new(indices);
                List<bool> keptVisibles = new();
                for (int i = 0; i < indices.Length; i++)
                {
                    if (!excludedIndices.Contains(i))
                    {
                        keptVisibles.Add(Visibles[i]);
                    }
                }
                Visibles = keptVisibles;
                return base.PopMultiple(indices);
            }

            public override void Clear()
            {
                Visibles.Clear();
                base.Clear();
            }

            public override CardsList RemoveFromBottom(int splitIndex)
            {
                Visibles.RemoveRange(splitIndex, Visibles.Count - 1);
                return base.RemoveFromBottom(splitIndex);
            }

            public override void Shuffle()
            {
                // Fisher-Yates Shuffle: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
                int n = _cards.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
                    (Visibles[n], Visibles[k]) = (Visibles[k], Visibles[n]);
                }
            }

            public bool ContainsMinLengthRun(int runLength)
            {
                if (Count < runLength) { return false; }
                int totalRun = 1;
                CardValue majorValue = _cards[0].value;
                foreach (Card card in _cards) 
                { 
                    if (card.value == majorValue) 
                    { 
                        totalRun++; 
                    }
                    else
                    {
                        totalRun = 1;
                        majorValue = card.value;
                    }

                    if (totalRun == runLength) { return true; }
                }
                return false;
            }

            public static new PlayCardPile Empty
            {
                get
                {
                    return new PlayCardPile();
                }
            }

            private List<bool> CheckVisibles()
            {
                if (_cards.Count == 0) { return new List<bool>(); }
                if (_cards.Count == 1) { return new List<bool> { _cards[0].value == CardValue.FOUR }; }

                bool[] foundVisibles = Enumerable.Repeat(true, Count).ToArray();
                List<CardValue> cardValues = base.CardValues;
                CardValue cardValue;
                for (int i = 0; i < cardValues.Count; i++)
                {
                    cardValue = cardValues[i];
                    if (cardValue == CardValue.FOUR) { foundVisibles[i] = false; }
                    if (cardValue == CardValue.JACK && i != 0 && cardValues[i-1] == CardValue.FOUR)
                    {
                        foundVisibles[i] = false;
                    }
                }
                return new List<bool>(foundVisibles);
            }
        }
    }
}
