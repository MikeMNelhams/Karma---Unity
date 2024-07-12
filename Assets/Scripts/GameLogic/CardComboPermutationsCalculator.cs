using DataStructures;
using Karma.Cards;
using System;
using System.Linq;
using System.Collections.Generic;
using Karma.Board;

namespace Karma
{
    namespace BasicBoard
    {
        public class CardComboCalculator
        {
            public static HashSet<FrozenMultiSet<CardValue>> FillerCombos(CardsList cards, CardValue filler, int minToFiller)
            {
                HashSet<FrozenMultiSet<CardValue>> output = new ();
                if (cards.Count == 0) { return output; }
                if (cards.Count == 1) { return SingleCardCombo(cards); }

                Dictionary<CardValue, int> count = new ();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.value;
                    if (!count.ContainsKey(key)) { count[key] = 1; }
                    else { count[key]++; }

                    List<CardValue> comboValues = Enumerable.Repeat(key, count[key]).ToList<CardValue>();
                    outputComboValues.Add(comboValues);
                    FrozenMultiSet<CardValue> combo = new (comboValues);
                    output.Add(combo);
                }

                int fillerCount = count.GetValueOrDefault(filler);
                if (fillerCount == 0) { return output; }
                HashSet<FrozenMultiSet<CardValue>> outputsIncludingFiller = new();
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
            public static HashSet<FrozenMultiSet<CardValue>> FillerAndFilterCombos(CardsList cards, CardValue filler, Func<CardValue, bool> filter, int minToFiller)
            {
                HashSet<FrozenMultiSet<CardValue>> output = new ();
                if (cards.Count == 0) { return output; }
                if (cards.Count == 1)
                {
                    if (filter(cards[0].value)) { return output; }
                    return SingleCardCombo(cards);
                }

                Dictionary<CardValue, int> count = new ();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.value;
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
                HashSet<FrozenMultiSet<CardValue>> outputsIncludingFiller = new();
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

            public static HashSet<FrozenMultiSet<CardValue>> FillerNotExclusiveCombos(CardsList cards, CardValue filler, int minToFiller)
            {
                if (cards.Count == 0) { return new HashSet<FrozenMultiSet<CardValue>>(); }
                if (cards.Count == 1) { return SingleCardCombo(cards); }

                Dictionary<CardValue, int> count = new ();
                HashSet<FrozenMultiSet<CardValue>> output = new();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.value;
                    if (!count.ContainsKey(key)) { count[key] = 1; }
                    else { count[key]++; }
                    List<CardValue> comboValues = Enumerable.Repeat(key, count[key]).ToList<CardValue>();
                    outputComboValues.Add(comboValues);
                    FrozenMultiSet<CardValue> combo = new (comboValues);
                    output.Add(combo);
                }
                int fillerCount = count.GetValueOrDefault(filler);
                if (fillerCount == 0) { return output; }
                HashSet<FrozenMultiSet<CardValue>> outputsIncludingFiller = new();
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

            public static HashSet<FrozenMultiSet<CardValue>> FillerFilterNotExclusiveCombos(CardsList cards, CardValue filler, Func<CardValue, bool> filter, int minToFiller)
            {
                HashSet<FrozenMultiSet<CardValue>> output = new();
                if (cards.Count == 0) { return output; }
                if (cards.Count == 1)
                {
                    if (filter(cards[0].value)) { return output; }
                    return SingleCardCombo(cards);
                }

                Dictionary<CardValue, int> count = new ();
                List<List<CardValue>> outputComboValues = new();
                foreach (Card card in cards)
                {
                    CardValue key = card.value;
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
                HashSet<FrozenMultiSet<CardValue>> outputsIncludingFiller = new();
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

            public static HashSet<FrozenMultiSet<CardValue>> SingleCardCombo(CardsList cards)
            {
                FrozenMultiSet<CardValue> combo = new() { cards[0].value };
                HashSet<FrozenMultiSet<CardValue>> output = new() { combo };
                return output;
            }

            public static HashSet<FrozenMultiSet<CardValue>> HandFlippedCombos(CardsList cards) 
            {
                HashSet<FrozenMultiSet<CardValue>> combos = new();
                foreach (CardValue cardValue in cards.CardValues)
                {
                    FrozenMultiSet<CardValue> frozenMultiSet = new() { cardValue };
                    combos.Add(frozenMultiSet);
                }
                return combos;
            }

            static HashSet<FrozenMultiSet<CardValue>> IncorrectlyDoubledFillerCombos(CardValue filler, int fillerCount)
            {
                HashSet<FrozenMultiSet<CardValue>> output = new();
                for (int i = 1; i < fillerCount + 1; i++)
                {
                    List<CardValue> cardValues = Enumerable.Repeat(filler, fillerCount + i).ToList();
                    output.Add(new FrozenMultiSet<CardValue>(cardValues));
                }
                return output;
            }

            static HashSet<FrozenMultiSet<CardValue>> FillerCombosNotExclusivelyFiller(CardValue filler, int fillerCount)
            {
                HashSet<FrozenMultiSet<CardValue>> output = new();
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

            public static CardsList PlayableCards(BoardPlayOrder playOrder, CardsList cards, CardValue topValue) 
            {
                CardsList playableCards = new();
                foreach (Card card in cards)
                {
                    if (card.value == CardValue.JOKER || IsPotentiallyPlayable(playOrder, card.value, topValue)) {  playableCards.Add(card); }
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
        }
    }
}


