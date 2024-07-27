using DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace KarmaLogic
{
    namespace Cards
    {
        public class CardsList : IList<Card>
        {
            protected List<Card> _cards;
            protected static System.Random rng = new();

            public CardsList()
            {
                this._cards = new List<Card>();
            }

            public CardsList(Card card)
            {
                _cards = new List<Card> { card };
            }

            public CardsList(List<Card> cards)
            {
                _cards = cards;
            }

            public CardsList(List<int> cardValues, CardSuit suit)
            {

                _cards = new List<Card>();
                for (int i=0; i < cardValues.Count; i++)
                {
                    Card card = new (suit, (CardValue)cardValues[i]);
                    _cards.Add(card);
                }
            }

            public static CardsList Repeat(Card card, int numberOfCopies)
            {
                List<Card> cardsList = Enumerable.Repeat(card, numberOfCopies).ToList<Card>();
                return new CardsList(cardsList);
            }

            public Card this[int index] { get => _cards[index]; set => _cards[index] = value; }
            public CardsList this[List<int> indices] { get => Get(indices); }

            public virtual Card Pop(int index)
            {
                Card poppedCard = _cards[index];
                RemoveAt(index);
                return poppedCard;
            }

            public virtual Card Pop()
            {
                int lastIndex = _cards.Count - 1;
                Card poppedCard = _cards[lastIndex];
                RemoveAt(lastIndex);
                return poppedCard;
            }

            public virtual CardsList PopMultiple(int[] indices)
            {
                HashSet<int> excludedCards = new(indices);
                CardsList poppedCards = new();
                List<Card> unpoppedCards = new();
                for (int i = 0; i < _cards.Count; i++)
                {
                    Card card = _cards[i];
                    if (excludedCards.Contains(i))
                    {
                        poppedCards.Add(card);
                    }
                }

                for (int i = 0; i < _cards.Count; i++)
                {
                    Card card = _cards[i];
                    if (!excludedCards.Contains(i))
                    {
                        unpoppedCards.Add(card);
                    }
                }
                _cards = unpoppedCards;
                return poppedCards;
            }

            public int Count => _cards.Count;

            public DictionaryDefaultInt<CardValue> CountAllCardValues()
            {
                DictionaryDefaultInt<CardValue> counts = new();
                foreach (Card card in _cards)
                {
                    counts.Add(card.Value, 1);
                }
                return counts;
            }

            public virtual void Add(Card item)
            {
                _cards.Add(item);
            }

            public virtual void Add(CardsList cards)
            {
                foreach (Card card in cards)
                {
                    Add(card);
                }
            }

            public bool Remove(Card item)
            {
                return _cards.Remove(item);
            }

            public CardsList Remove(CardsList cards)
            {
                // This is the LIST difference. Maintains order and duplicates.
                CardsList removedCards = new();
                Dictionary<Card, uint> targetCounts = new();
                CardsList leftovers = new();
                foreach (Card card in cards)
                {
                    if (!targetCounts.ContainsKey(card))
                    {
                        targetCounts[card] = 0;
                    }
                    targetCounts[card]++;
                }


                for (int i = 0; i < this._cards.Count; i++)
                {
                    Card card = this._cards[i];
                    if (targetCounts.ContainsKey(card) && targetCounts[card] > 0)
                    {
                        targetCounts[card]--;
                        removedCards.Add(card);
                    }
                    else
                    {
                        leftovers.Add(card);
                    }
                }
                this._cards = leftovers._cards;
                return removedCards;
            }

            public CardsList Remove(FrozenMultiSet<CardValue> cards)
            {
                CardsList removedCards = new();
                CardsList leftovers = new();
                
                for (int i = 0; i < this._cards.Count; i++)
                {
                    Card card = this._cards[i];
                    if (cards.Contains(card.Value) && cards[card.Value] > 0)
                    {
                        cards.Remove(card.Value);
                    }
                    else
                    {
                        leftovers.Add(card);
                    }
                }
                this._cards = leftovers._cards;
                return removedCards;
            }

            public virtual void Clear()
            {
                _cards.Clear();
            }

            public Card Swap(int index, Card card)
            {
                Card cardBeforeSwap = _cards[index];
                _cards[index] = card;
                return cardBeforeSwap;
            }

            public virtual int[] Sort()
            {
                int[] indices = Enumerable.Range(0, _cards.Count).ToArray();
                Array.Sort(_cards.ToArray(), indices);
                _cards.Sort();
                return indices;
            }

            public virtual int[] Shuffle()
            {
                // Fisher-Yates Shuffle: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
                int n = _cards.Count;
                int[] indices = Enumerable.Range(0, n).ToArray();
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
                    (indices[n], indices[k]) = (indices[k], indices[n]);
                }
                return indices;
            }

            public override string ToString()
            {
                string listString = "";
                foreach (Card card in _cards)
                {
                    listString += card.ToString() + ", ";
                }

                return "Cards[" + listString + "]";
            }

            public bool Contains(Card item)
            {
                return _cards.Contains(item);
            }

            public bool Contains(CardsList cards)
            {
                return Where(cards).Count == cards.Count;
            }

            public List<int> Where(CardsList cards)
            {
                CardsList targetCards = cards.CopyShallow();
                CardsList thisCopy = CopyShallow();
                List<int> indices = new();

                for (int i = targetCards.Count - 1; i > 0; i--)
                {
                    Card targetCard = targetCards[i];
                    for (int j = thisCopy.Count - 1; j > 0; j--)
                    {
                        if (thisCopy[j] == targetCard)
                        {
                            indices.Add(j);
                            targetCards.RemoveAt(i);
                        }
                    }
                }

                return indices;
            }

            public bool IsExclusively(CardValue cardValue)
            {
                return _cards.All(card => card.Value == cardValue);
            }

            public CardsList Get(List<int> indices)
            {
                CardsList cards = new();
                for (int i = 0; i < indices.Count; i++)
                {
                    cards.Add(_cards[i]);
                }
                return cards;
            }

            public int CountValue(CardValue targetValue)
            {
                int total = 0;
                foreach (Card card in _cards)
                {
                    if (card.Value == targetValue)
                    {
                        total++;
                    }
                }
                return total;
            }

            public List<CardValue> CardValues
            {
                get
                {
                    if (_cards == null || _cards.Count == 0)
                    {
                        return new List<CardValue>();
                    }
                    List<CardValue> cardValues = new();
                    foreach (Card card in _cards)
                    {
                        cardValues.Add(card.Value);
                    }
                    return cardValues;
                }
            }

            public bool IsReadOnly => ((ICollection<Card>)_cards).IsReadOnly;

            public void CopyTo(Card[] array, int arrayIndex)
            {
                _cards.CopyTo(array, arrayIndex);
            }

            public IEnumerator<Card> GetEnumerator()
            {
                return _cards.GetEnumerator();
            }

            public int IndexOf(Card item)
            {
                return _cards.IndexOf(item);
            }

            public void Insert(int index, Card item)
            {
                _cards.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                _cards.RemoveAt(index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _cards.GetEnumerator();
            }

            public CardsList CopyShallow()
            {
                CardsList newCardsList = new ();
                foreach (Card card in _cards)
                {
                    newCardsList.Add(card);
                }
                return newCardsList;
            }
        }
    }
}