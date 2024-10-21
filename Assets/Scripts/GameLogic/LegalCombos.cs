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
            // A non-legal is sublegal if it is currently NOT legal, but it's possible to become legal by adding more cards
            // A combo could only become legal if it can BECOME a 6 filled legal combo:
            //  1   Combo has at least 1 non-6 and at least 1 6.
            //  2   Non-6 is legal by itself
            //  3   Non-6 count < required filler count
            //  4   CardValueMaxCounts[Non-6] >= req_filler_count
            // However, condition 3 is always the case if 1 & 2 are true
            if (combo.TotalCount == 0) { return false; }
            if (IsLegal(combo)) { return false; }
            if (combo.KeyCount != 2) { return false; }

            bool containsFiller = combo.Contains(CardValue.SIX);
            if (!containsFiller) { return false; }

            CardValue nonSix = CardValue.TWO; // Dummy variable

            foreach (CardValue key in combo)
            {
                if (key != CardValue.SIX)
                {
                    nonSix = key;
                    break;
                }
            }

            if (!IsLegal(new FrozenMultiSet<CardValue> { nonSix })) { return false; }

            return CardValueMaxCounts[nonSix] >= 3;
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

