using DataStructures;
using KarmaLogic.Cards;
using KarmaLogic.Board;
using KarmaLogic.CardCombos;
using System;
using System.Linq;
using System.Collections.Generic;


namespace KarmaLogic
{
    namespace BasicBoard
    {
        public class CardComboCalculator
        {
            public static LegalCombos FillerCombos(CardsList cards, CardValue filler, int minToFiller)
            {
                LegalCombos output = new ();
                if (cards.Count == 0) { return output; }
                if (cards.Count == 1) { return SingleCardCombo(cards); }

                Dictionary<CardValue, int> count = new ();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.Value;
                    if (!count.ContainsKey(key)) { count[key] = 1; }
                    else { count[key]++; }

                    List<CardValue> comboValues = Enumerable.Repeat(key, count[key]).ToList<CardValue>();
                    outputComboValues.Add(comboValues);
                    FrozenMultiSet<CardValue> combo = new (comboValues);
                    output.Add(combo);
                }

                int fillerCount = count.GetValueOrDefault(filler);
                if (fillerCount == 0) { return output; }
                LegalCombos outputsIncludingFiller = new();
                foreach (FrozenMultiSet<CardValue> combo in output)
                {
                    if (combo.TotalCount < minToFiller) { continue; }
                    for (int i = 1; i < fillerCount + 1; i++)
                    {
                        List<CardValue> fillerComboValues = Enumerable.Repeat(combo.First(), combo.TotalCount).ToList<CardValue>();
                        fillerComboValues.AddRange(Enumerable.Repeat(filler, i).ToList<CardValue>());
                        outputsIncludingFiller.Add(new FrozenMultiSet<CardValue>(fillerComboValues));
                    }
                }
                output.UnionWith(outputsIncludingFiller);
                output.ExceptWith(IncorrectlyDoubledFillerCombos(filler, fillerCount));
                output.UnionWith(outputsIncludingFiller);
                return output;
            }
            public static LegalCombos FillerAndFilterCombos(CardsList cards, CardValue filler, Func<CardValue, bool> filter, int minToFiller)
            {
                LegalCombos output = new ();
                if (cards.Count == 0) { return output; }
                if (cards.Count == 1)
                {
                    if (filter(cards[0].Value)) { return output; }
                    return SingleCardCombo(cards);
                }

                Dictionary<CardValue, int> count = new ();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.Value;
                    if (!count.ContainsKey(key)) { count[key] = 1; }
                    else { count[key]++; }
                    if (filter(key)) { continue; }

                    List<CardValue> comboValues = Enumerable.Repeat(key, count[key]).ToList<CardValue>();
                    outputComboValues.Add(comboValues);
                    FrozenMultiSet<CardValue> combo = new (comboValues);
                    output.Add(combo);
                }

                int fillerCount = count.GetValueOrDefault(filler);
                if (fillerCount == 0) {return output; }
                LegalCombos outputsIncludingFiller = new();
                foreach (FrozenMultiSet<CardValue> combo in output)
                {
                    if (combo.TotalCount < minToFiller) { continue; }
                    for (int i = 1; i < fillerCount + 1; i++) 
                    {
                        List<CardValue> fillerComboValues = Enumerable.Repeat(combo.First(), combo.TotalCount).ToList<CardValue>();
                        fillerComboValues.AddRange(Enumerable.Repeat(filler, i).ToList<CardValue>());
                        outputsIncludingFiller.Add(new FrozenMultiSet<CardValue>(fillerComboValues));
                    }
                }
                output.UnionWith(outputsIncludingFiller);
                output.ExceptWith(IncorrectlyDoubledFillerCombos(filler, fillerCount));
                return output;
            }

            public static LegalCombos FillerNotExclusiveCombos(CardsList cards, CardValue filler, int minToFiller)
            {
                if (cards.Count == 0) { return new LegalCombos(); }
                if (cards.Count == 1) { return SingleCardCombo(cards); }

                Dictionary<CardValue, int> count = new ();
                LegalCombos output = new();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.Value;
                    if (!count.ContainsKey(key)) { count[key] = 1; }
                    else { count[key]++; }
                    List<CardValue> comboValues = Enumerable.Repeat(key, count[key]).ToList<CardValue>();
                    outputComboValues.Add(comboValues);
                    FrozenMultiSet<CardValue> combo = new (comboValues);
                    output.Add(combo);
                }
                int fillerCount = count.GetValueOrDefault(filler);
                if (fillerCount == 0) { return output; }
                LegalCombos outputsIncludingFiller = new();
                foreach (FrozenMultiSet<CardValue> combo in output)
                {
                    if (combo.TotalCount < minToFiller) { continue; }
                    for (int i = 1; i < fillerCount + 1; i++)
                    {
                        List<CardValue> fillerComboValues = Enumerable.Repeat(combo.First(), combo.TotalCount).ToList<CardValue>();
                        fillerComboValues.AddRange(Enumerable.Repeat(filler, i).ToList<CardValue>());
                        outputsIncludingFiller.Add(new FrozenMultiSet<CardValue>(fillerComboValues));
                    }
                }
                output.UnionWith(outputsIncludingFiller);
                output.ExceptWith(FillerCombosNotExclusivelyFiller(filler, fillerCount));
                return output;
            }

            public static LegalCombos FillerFilterNotExclusiveCombos(CardsList cards, CardValue filler, Func<CardValue, bool> filter, int minToFiller)
            {
                LegalCombos output = new();
                if (cards.Count == 0) { return output; }
                if (cards.Count == 1)
                {
                    if (filter(cards[0].Value)) { return output; }
                    return SingleCardCombo(cards);
                }

                Dictionary<CardValue, int> count = new ();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.Value;
                    if (!count.ContainsKey(key)) { count[key] = 1; }
                    else { count[key]++; }
                    if (filter(key)) { continue; }
                    List<CardValue> comboValues = Enumerable.Repeat(key, count[key]).ToList<CardValue>();
                    outputComboValues.Add(comboValues);
                    FrozenMultiSet<CardValue> combo = new (comboValues);
                    output.Add(combo);
                }
                int fillerCount = count.GetValueOrDefault(filler);
                if (fillerCount == 0) { return output; }
                LegalCombos outputsIncludingFiller = new();
                foreach (FrozenMultiSet<CardValue> combo in output)
                {
                    if (combo.TotalCount < minToFiller) { continue; }
                    for (int i = 1; i < fillerCount + 1; i++)
                    {
                        List<CardValue> fillerComboValues = Enumerable.Repeat(combo.First(), combo.TotalCount).ToList<CardValue>();
                        fillerComboValues.AddRange(Enumerable.Repeat(filler, i).ToList<CardValue>());
                        outputsIncludingFiller.Add(new FrozenMultiSet<CardValue>(fillerComboValues));
                    }
                }

                output.UnionWith(outputsIncludingFiller);
                output.ExceptWith(FillerCombosNotExclusivelyFiller(filler, fillerCount));
                return output;
            }

            public static LegalCombos SingleCardCombo(CardsList cards)
            {
                FrozenMultiSet<CardValue> combo = new() { cards[0].Value };
                LegalCombos output = new() { combo };
                return output;
            }

            public static LegalCombos HandFlippedCombos(CardsList cards) 
            {
                LegalCombos combos = new();
                foreach (CardValue cardValue in cards.CardValues)
                {
                    FrozenMultiSet<CardValue> frozenMultiSet = new() { cardValue };
                    combos.Add(frozenMultiSet);
                }
                return combos;
            }

            static LegalCombos IncorrectlyDoubledFillerCombos(CardValue filler, int fillerCount)
            {
                LegalCombos output = new();
                for (int i = 1; i < fillerCount + 1; i++)
                {
                    List<CardValue> cardValues = Enumerable.Repeat(filler, fillerCount + i).ToList();
                    output.Add(new FrozenMultiSet<CardValue>(cardValues));
                }
                return output;
            }

            static LegalCombos FillerCombosNotExclusivelyFiller(CardValue filler, int fillerCount)
            {
                LegalCombos output = new();
                for (int i = 1; i < fillerCount * 2 + 1; i++)
                {
                    List<CardValue> cardValues = Enumerable.Repeat(filler, i).ToList();
                    output.Add(new FrozenMultiSet<CardValue>(cardValues));
                }
                return output;
            }

            public static bool IsJoker(CardValue cardValue)
            {
                return cardValue == CardValue.JOKER;
            }

            public static CardsList PlayableCards(BoardPlayOrder playOrder, CardsList cards, CardValue topValue, DictionaryDefaultInt<CardValue> cardValueInPlayCounts) 
            {
                CardsList playableCards = new();
                if (cardValueInPlayCounts[CardValue.ACE] > 0)
                {
                    return PlayableCardsAcesInPlay(playOrder, cards, topValue, playableCards);
                }

                return PlayableCardsNoAcesInPlay(playOrder, cards, topValue, playableCards);
            }

            static CardsList PlayableCardsAcesInPlay(BoardPlayOrder playOrder, CardsList cards, CardValue topValue, CardsList playableCards)
            {
                foreach (Card card in cards)
                {
                    if (card.Value == CardValue.JOKER || IsPotentiallyPlayable(playOrder, card.Value, topValue)) { playableCards.Add(card); }
                }
                return playableCards;
            }

            static CardsList PlayableCardsNoAcesInPlay(BoardPlayOrder playOrder, CardsList cards, CardValue topValue, CardsList playableCards)
            {
                foreach (Card card in cards)
                {
                    if (IsPotentiallyPlayable(playOrder, card.Value, topValue)) { playableCards.Add(card); }
                }
                return playableCards;
            }

            static bool IsPotentiallyPlayable(BoardPlayOrder playOrder, CardValue cardValue, CardValue topValue)
            {
                Func<CardValue, CardValue, bool> comparison = Comparison(playOrder);
                bool isAlwaysLegal = cardValue == CardValue.TWO || cardValue == CardValue.FOUR;
                return isAlwaysLegal || comparison(cardValue, topValue) || cardValue == CardValue.SIX;
            }

            static Func<CardValue, CardValue, bool> Comparison(BoardPlayOrder playOrder)
            {
                Func<CardValue, CardValue, bool> comparison = (x, y) => (int)x >= (int)y;
                if (playOrder == BoardPlayOrder.DOWN) { comparison = (x, y) => (int)x <= (int)y; }
                return comparison;
            }

            public static bool ContainsUnplayableFiller(BoardPlayOrder playOrder, CardsList cards, CardValue topValue, CardValue filler = CardValue.SIX)
            {
                bool containsFiller = cards.CardValues.ToHashSet<CardValue>().Contains(filler);
                Func<CardValue, CardValue, bool> comparison = Comparison(playOrder);
                bool fillerIsUnplayable = !comparison(filler, topValue);
                return containsFiller && fillerIsUnplayable;
            }

            public static bool ContainsPlayableJokersAsAceValues(bool jokersAreAces, CardsList cards)
            {
                return jokersAreAces && cards.CountValue(CardValue.JOKER) > 0;
            }
        }
    }
}


