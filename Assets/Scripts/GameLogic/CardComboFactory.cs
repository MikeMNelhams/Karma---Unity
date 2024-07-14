using Karma.Board;
using Karma.Controller;
using Karma.Players;
using Karma.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataStructures;

namespace Karma
{
    namespace CardCombos
    {
        public class CardCombo_TWO : CardCombo
        {
            public CardCombo_TWO(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts){}
            public override void Apply(IBoard board)
            {
                board.ResetPlayOrder();
            }
        }

        public class CardCombo_THREE : CardCombo
        {
            public CardCombo_THREE(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                board.EffectMultiplier *= (int)Math.Pow(2, Cards.Count);
            }
        }

        public class CardCombo_FOUR : CardCombo
        {
            public CardCombo_FOUR(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                return;
            }
        }

        public class CardCombo_FIVE : CardCombo
        {
            public CardCombo_FIVE(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                List<Hand> startHands = new ();
                foreach (Player player in board.Players)
                {
                    startHands.Add(player.Hand);
                }
                Deque<Hand> hands = new (startHands);
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                
                if (numberOfRepeats < board.Players.Count)
                {
                    board.RotateHands(numberOfRepeats, hands);
                    return;
                }
                board.RotateHands(board.Players.Count, hands);
                board.RotateHands(numberOfRepeats % board.Players.Count, hands);
                return;
            }
        }

        public class CardCombo_SIX : CardCombo
        {
            public CardCombo_SIX(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                return;
            }
        }

        public class CardCombo_SEVEN : CardCombo
        {
            public CardCombo_SEVEN(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                if (((Cards.Count * (uint)board.EffectMultiplier) & 0b1) == 0b0)
                {
                    return;
                }
                board.FlipPlayOrder();
            }
        }

        public class CardCombo_EIGHT : CardCombo
        {
            public CardCombo_EIGHT(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                if (((Cards.Count * (uint)board.EffectMultiplier) & 0b1) == 0b0)
                {
                    return;
                }
                board.FlipTurnOrder();
            }
        }
        public class CardCombo_NINE : CardCombo
        {
            public CardCombo_NINE(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                if (board.PlayPile.ContainsMinLengthRun(4)) { return; }
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                board.StepPlayerIndex(numberOfRepeats);
            }
        }

        public class CardCombo_TEN : CardCombo
        {
            public CardCombo_TEN(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                board.Burn(0);
            }
        }

        public class CardCombo_JACK : CardCombo
        {
            public CardCombo_JACK(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                if (board.PlayPile.Count <= Cards.Count) { return; }
                
                Card cardBelowCombo = board.PlayPile[^(1 + Cards.Count)];
                if (cardBelowCombo.value == CardValue.JACK) { return; }
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                if (cardBelowCombo.value == CardValue.THREE) { numberOfRepeats = Cards.Count; }
                Debug.Log("Card to replay:" + cardBelowCombo  + " " + numberOfRepeats + " many times");
                CardsList cardsToReplay = CardsList.Repeat(cardBelowCombo, numberOfRepeats);
                board.PlayCards(cardsToReplay, Controller, false);
            }
        }

        public class CardCombo_QUEEN : CardCombo
        {
            public CardCombo_QUEEN(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                Player currentPlayer = board.CurrentPlayer;
                if (!currentPlayer.HasCards) { return; }
                if (currentPlayer.PlayingFrom == PlayingFrom.KarmaUp && currentPlayer.KarmaUp.Count == 0) { return; }
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                board.StartGivingAwayCards(numberOfRepeats);  
            }
        }

        public class CardCombo_KING : CardCombo
        {
            public CardCombo_KING(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                if (board.PlayPile.ContainsMinLengthRun(4)) { board.Burn(0); }
                numberOfRepeats = Math.Min(numberOfRepeats, board.BurnPile.Count);
                if (numberOfRepeats == 0) { return; }
                CardsList cardsToPlay = board.BurnPile.RemoveFromBottom(numberOfRepeats);
                foreach (Card card in cardsToPlay)
                {
                    if (card.value == CardValue.JOKER) { board.NumberOfJokersInPlay++; }
                    board.PlayCards(new CardsList(card), Controller);
                }
            }
        }

        public class CardCombo_ACE : CardCombo
        {
            public CardCombo_ACE(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                if (((uint)(numberOfRepeats) & 0b1) == 0b0)
                {
                    board.FlipHands();
                    board.FlipHands();
                    return;
                }

                board.FlipHands();
                if (board.HandsAreFlipped)
                {
                    foreach (Player player in board.Players)
                    {
                        if (player.Hand.Count == 0) { continue; }
                        player.Hand.Shuffle();
                    }
                }
                else
                {
                    foreach (Player player in board.Players)
                    {
                        if (player.Hand.Count == 0) { continue; }
                        player.Hand.Sort();
                    }
                }
            }
        }

        public class CardCombo_JOKER : CardCombo
        {
            public CardCombo_JOKER(CardsList cards, IController controller, Dictionary<CardValue, int> counts) : base(cards, controller, counts) { }
            public override void Apply(IBoard board)
            {
                board.Burn(Cards.Count);
                if (board.PlayPile.Count == 0) { return; }
                
                board.StartGivingAwayPlayPile(board.PlayerIndexWhoStartedTurn);
            }
        }

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
                    if (!_counts.ContainsKey(card.value))
                    {
                        _counts[card.value] = 0;
                    }
                    else
                    {
                        _counts[card.value]++;
                    }
                }
            }

            public CardValue ComboCardValue()
            {
                if (_counts.Count == 1) { return _cards[0].value; }
                else if (_counts.Count == 2)
                {
                    CardValue majorValue = _cards[0].value;
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

            public CardCombo CreateCombo(IController controller)
            {
                CardValue cardValue = ComboCardValue();
                if (!_cardComboMap.ContainsKey(cardValue))
                {
                    throw new NotImplementedException("No mapping is defined for: " + cardValue);
                }
                Type type = _cardComboMap[cardValue];
                return Activator.CreateInstance(type, _cards, controller, _counts) as CardCombo;
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
                    bool visibility = !(board.PlayPile.Count > 0 && board.PlayPile[^1].value == CardValue.FOUR);
                    return Enumerable.Repeat<bool>(visibility, _cards.Count).ToList<bool>();
                }
                return Enumerable.Repeat<bool>(true, _cards.Count).ToList<bool>();
            }
        }
    }
}

