using System;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic
{
    namespace Cards
    {
        [System.Serializable]
        public class Card : IEquatable<Card>, IComparable<Card>
        {
            [SerializeField] CardValue _value;
            [SerializeField] CardSuit _suit;

            public CardSuit Suit { get => _suit; set => _suit = value; }
            public CardValue Value { get => _value; set => _value = value; }

            public Card(CardValue value, CardSuit suit)
            {
                Suit = suit;
                Value = value;
            }

            public bool Equals(Card other)
            {
                return Suit.Equals(other.Suit) && Value == other.Value;
            }

            public override bool Equals(object other)
            {
                if (other is null) { return false; }
                if (ReferenceEquals(this, other)) { return true; }
                if (GetType() != other.GetType()) { return false; }
                return Equals(other as Card);
            }

            public int CompareTo(Card other)
            {
                if (this > other) return 1;
                if (this < other) return -1;
                return 0;
            }

            public override int GetHashCode()
            {
                return new { Suit, Value }.GetHashCode();
            }

            public override string ToString()
            {
                string valueString;
                int valueInt = (int)Value;
                if ((0 <= valueInt) && (valueInt <= 10)) { valueString = valueInt.ToString(); }
                else { valueString = Value.ToString()[0..2]; }
                return valueString + Suit.ToString();
            }

            public Card Copy()
            {
                return new Card(Value, Suit);
            }

            public static bool operator ==(Card x, Card y) => x.Equals(y);
            public static bool operator !=(Card x, Card y) => !(x == y);
            public static bool operator >(Card x, Card y) => x.Value > y.Value;
            public static bool operator <(Card x, Card y) => x.Value < y.Value;
            public static bool operator >=(Card x, Card y) => x.Value >= y.Value;
            public static bool operator <=(Card x, Card y) => x.Value <= y.Value;
        }

        public enum CardValue : byte
        {
            TWO = 2,
            THREE = 3,
            FOUR = 4,
            FIVE = 5,
            SIX = 6,
            SEVEN = 7,
            EIGHT = 8,
            NINE = 9,
            TEN = 10,
            JACK = 11,
            QUEEN = 12,
            KING = 13,
            ACE = 14,
            JOKER = 15
        }
    }
}