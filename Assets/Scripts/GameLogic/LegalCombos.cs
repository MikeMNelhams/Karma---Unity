using DataStructures;
using KarmaLogic.Cards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KarmaLogic.CardCombos
{
    public class LegalCombos : IEnumerable
    {
        public HashSet<FrozenMultiSet<CardValue>> Combos { get; protected set; }
        public HashSet<CardValue> CardValues { get; protected set; }
        public MultiSet<CardValue> CardValueMaxCounts { get; protected set; }

        public int NumberOfCardValueTypes { get => CardValues.Count; }
        public int Count { get => Combos.Count; }

        public LegalCombos()
        {
            Combos = new HashSet<FrozenMultiSet<CardValue>>();
            CardValues = new HashSet<CardValue>();
            CardValueMaxCounts = new FrozenMultiSet<CardValue>();
        }

        public LegalCombos(HashSet<FrozenMultiSet<CardValue>> combos)
        {
            Combos = combos;
            CardValues = AllCardValues(combos);
            CardValueMaxCounts = CalculateCardValueMaxCounts(combos);
        }

        public bool IsLegal(FrozenMultiSet<CardValue> combo)
        {
            return Combos.Contains(combo);
        }

        public bool IsSubsetLegal(FrozenMultiSet<CardValue> combo)
        {
            // Assumes the combo is NOT legal
            if (combo.TotalCount == 0) { return false; }
            if (combo.KeyCount > 2) { return false; }

            bool containsFiller = combo.Contains(CardValue.SIX);
            if (!containsFiller) { return false; }

            int fillerCount = combo[CardValue.SIX];

            if (combo.KeyCount == 1)
            {
                return CardValueMaxCounts.Max() >= 3;
            }

            
            CardValue nonSix = CardValue.TWO;
            int nonFillerCount = 0;
            foreach (CardValue key in combo)
            {
                if (key != CardValue.SIX)
                {
                    nonSix = key;
                    nonFillerCount = combo[nonSix] - fillerCount;
                    break;
                }
            }

            return nonFillerCount <= CardValueMaxCounts[nonSix] && fillerCount <= CardValueMaxCounts[CardValue.SIX];
        }

        public bool Contains(FrozenMultiSet<CardValue> combo)
        {
            return Combos.Contains(combo);
        }

        public bool Add(FrozenMultiSet<CardValue> combo)
        {
            bool added = Combos.Add(combo);
            CardValueMaxCounts.AddMax(combo);

            foreach (CardValue key in combo.Keys)
            {
                CardValues.Add(key);
            }
            
            return added;
        }

        public void UnionWith(LegalCombos other)
        {
            Combos.UnionWith(other.Combos);
            CardValues.UnionWith(other.CardValues);
            CardValueMaxCounts.UnionMaxWith(other.CardValueMaxCounts);
        }

        public void ExceptWith(LegalCombos other)
        {
            Combos.ExceptWith(other.Combos);
            CardValues.ExceptWith(other.CardValues);
            CardValueMaxCounts = CalculateCardValueMaxCounts(Combos);
        }

        public FrozenMultiSet<CardValue> First()
        {
            return Combos.First();
        }

        HashSet<CardValue> AllCardValues(HashSet<FrozenMultiSet<CardValue>> combos)
        {
            HashSet<CardValue> allCardValues = new ();
            foreach (FrozenMultiSet<CardValue> combo in combos)
            {
                foreach (CardValue cardValue in combo)
                {
                    allCardValues.Add(cardValue);
                }
            }
            return allCardValues;
        }


        FrozenMultiSet<CardValue> CalculateCardValueMaxCounts(HashSet<FrozenMultiSet<CardValue>> combos)
        {
            Dictionary<CardValue, int> allCardValueCounts = new ();
            foreach (FrozenMultiSet<CardValue> combo in combos)
            {
                foreach (CardValue cardValue in combo)
                {
                    int count = combo[cardValue];
                    if (!allCardValueCounts.ContainsKey(cardValue))
                    {
                        allCardValueCounts.Add(cardValue, count);
                    }
                    else
                    {
                        allCardValueCounts[cardValue] = Mathf.Max(allCardValueCounts[cardValue], count);
                    }
                }
            }

            return new FrozenMultiSet<CardValue>(allCardValueCounts);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (FrozenMultiSet<CardValue> combo in Combos)
            {
                yield return combo;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

