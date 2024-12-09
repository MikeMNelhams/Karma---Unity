using KarmaLogic.Board;
using KarmaLogic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;


namespace KarmaLogic
{
    namespace CardCombos
    {
        public class CardComboFactory
        {
            protected Dictionary<CardValue, int> _counts;
            protected CardsList _cards;
            protected Dictionary<CardValue, Type> _cardComboMap = new();

            public CardComboFactory()
            {
                _counts = new Dictionary<CardValue, int>();
                _cardComboMap.Add(CardValue.TWO, typeof(CardCombo_TWO));
                _cardComboMap.Add(CardValue.THREE, typeof(CardCombo_THREE));
                _cardComboMap.Add(CardValue.FOUR, typeof(CardCombo_FOUR));
                _cardComboMap.Add(CardValue.FIVE, typeof(CardCombo_FIVE));
                _cardComboMap.Add(CardValue.SIX, typeof(CardCombo_SIX));
                _cardComboMap.Add(CardValue.SEVEN, typeof(CardCombo_SEVEN));
                _cardComboMap.Add(CardValue.EIGHT, typeof(CardCombo_EIGHT));
                _cardComboMap.Add(CardValue.NINE, typeof(CardCombo_NINE));
                _cardComboMap.Add(CardValue.TEN, typeof(CardCombo_TEN));
                _cardComboMap.Add(CardValue.JACK, typeof(CardCombo_JACK));
                _cardComboMap.Add(CardValue.QUEEN, typeof(CardCombo_QUEEN));
                _cardComboMap.Add(CardValue.KING, typeof(CardCombo_KING));
                _cardComboMap.Add(CardValue.ACE, typeof(CardCombo_ACE));
                _cardComboMap.Add(CardValue.JOKER, typeof(CardCombo_JOKER));
            }

            public bool IsValidCombo()
            {
                if (_counts.Count > 2)
                {
                    return false;
                }
                if (_counts.Count == 2)
                {
                    return _counts.ContainsKey(CardValue.SIX);
                }
                return true;
            }

            public void SetCounts(CardsList cards)
            {
                _counts = new Dictionary<CardValue, int>();
                _cards = cards;
                foreach (Card card in cards)
                {
                    if (!_counts.ContainsKey(card.Value))
                    {
                        _counts[card.Value] = 0;
                    }
                    else
                    {
                        _counts[card.Value]++;
                    }
                }
            }

            public CardValue ComboCardValue()
            {
                if (_counts.Count == 1) { return _cards[0].Value; }
                else if (_counts.Count == 2)
                {
                    CardValue majorValue = _cards[0].Value;
                    foreach (CardValue cardValue in _cards.CardValues)
                    {
                        if (cardValue != CardValue.SIX)
                        {
                            majorValue = cardValue;
                            break;
                        }
                    }
                    return majorValue;
                }
                UnityEngine.Debug.Log("Card value counts are invalid: " + _counts.Count);
                throw new NotImplementedException();
            }

            public CardCombo CreateCombo()
            {
                CardValue cardValue = ComboCardValue();
                if (!_cardComboMap.ContainsKey(cardValue))
                {
                    throw new NotImplementedException("No mapping is defined for: " + cardValue);
                }
                Type type = _cardComboMap[cardValue];
                return Activator.CreateInstance(type, _cards, _counts) as CardCombo;
            }

            public List<bool> ComboVisibility(IBoard board)
            {
                CardValue cardValue = ComboCardValue();
                if (cardValue == CardValue.FOUR)
                {
                    return Enumerable.Repeat<bool>(false, _cards.Count).ToList<bool>();
                }
                if (cardValue == CardValue.JACK)
                {
                    bool visibility = !(board.PlayPile.Count > 0 && board.PlayPile[^1].Value == CardValue.FOUR);
                    return Enumerable.Repeat<bool>(visibility, _cards.Count).ToList<bool>();
                }
                return Enumerable.Repeat<bool>(true, _cards.Count).ToList<bool>();
            }
        }
    }
}

