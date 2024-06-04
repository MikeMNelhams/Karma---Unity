using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Cards { 
    public class CardsList : IList<Card>
    {   
        protected List<Card> _cards = new();
        private static Random rng = new();

        public CardsList() 
        {
            this._cards = new List<Card>();
        }

        public CardsList(List<Card> cards)
        {
            this._cards = cards;
        }

        public Card this[int index] { get => _cards[index]; set => _cards[index] = value; }
        public CardsList this[List<int> indices] { get => Get(indices); }

        public Card Pop(int index)
        {
            Card poppedCard = _cards[index];
            RemoveAt(index);
            return poppedCard;
        }

        public CardsList PopMultiple(int[] indices)
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

        public void Add(Card item)
        {
            _cards.Add(item);
        }

        public void Add(CardsList cards)
        {
            foreach(Card card in cards) 
            {
                Add(card);
            }
        }

        public bool Remove(Card item)
        {
            return _cards.Remove(item);
        }

        public CardsList RemoveCards(CardsList cards)
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

        public void Clear()
        {
            _cards.Clear();
        }

        public Card Swap(int index, Card card)
        {
            Card cardBeforeSwap = _cards[index];
            _cards[index] = card;
            return cardBeforeSwap;
        }

        public void Shuffle()
        {
            // Fisher-Yates Shuffle: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
            }
        }

        public override string ToString()
        {
            string listString = this._cards.ToString();
            listString = listString[1..];
            return "[" + listString + "]";
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
            return _cards.All(card => card.value == cardValue);
        }

        public CardsList Get(List<int> indices)
        {
            CardsList cards = new();
            for (int i=0; i < indices.Count; i++)
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
                if (card.value == targetValue)
                {
                    total++;
                }
            }
            return total;
        }

        public List<CardValue> CardValues()
        {
            if (_cards == null || _cards.Count == 0)
            {
                return new List<CardValue>();
            }
            List<CardValue> cardValues = new();
            foreach (Card card in _cards)
            {
                cardValues.Add(card.value);
            }
            return cardValues;
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
            CardsList newCardsList = new CardsList();
            foreach (Card card in _cards)
            {
                newCardsList.Add(card);
            }
            return newCardsList;
        }
    }
}