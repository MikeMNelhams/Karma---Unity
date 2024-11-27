using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KarmaLogic
{
    namespace Cards
    {
        public class Hand : CardsList
        {
            public delegate void OnHandOrderChange(int[] indices);
            event OnHandOrderChange HandOrderChangeEvent;

            public List<Card> CardsT { get => _cards; }

            public Hand() : base() { }

            public Hand(Card card) : base(card) { }

            public Hand(List<Card> cards) : base(cards) { }

            public Hand(List<int> cardValues, CardSuit suit) : base(cardValues, suit) { }

            public Hand(CardsList cards)
            {
                _cards = cards.ToList<Card>();
            }

            public override void Add(Card item)
            {
                int left = 0;
                int right = _cards.Count;
                int middle;
                CardValue targetValue = item.Value;
                CardValue middleValue;
                while (left < right)
                {
                    middle = (right - left) / 2 + left;
                    middleValue = _cards[middle].Value;

                    if (middleValue > targetValue)
                    {
                        right = middle;
                    }
                    else
                    {
                        left = middle + 1;
                    }
                }

                int insertIndex = Mathf.Max(left, 0);
                _cards.Insert(insertIndex, item);
            }

            public override string ToString()
            {
                string listString = "";
                foreach (Card card in _cards)
                {
                    listString += card.ToString() + ", ";
                }

                return "Hand[" + listString + "]";
            }

            public void SetHandCards(Hand other)
            {
                _cards = other._cards;
            }

            public override int[] Sort()
            {
                int[] indices = base.Sort();
                HandOrderChangeEvent?.Invoke(indices);
                return indices;
            }

            public override int[] Shuffle()
            {
                int[] indices = base.Shuffle();
                HandOrderChangeEvent?.Invoke(indices);
                return indices;
            }

            public void RegisterHandOrderChangeEvent(OnHandOrderChange eventListener)
            {
                HandOrderChangeEvent += eventListener;
            }

            public void UnregisterHandOrderChangeEvent(OnHandOrderChange eventListener)
            {
                HandOrderChangeEvent -= eventListener;
            }
        }
    }
}
