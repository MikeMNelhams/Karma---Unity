using System;
using UnityEngine;

namespace KarmaLogic
{
    namespace Cards
    {
        public class Card : IEquatable<Card>, IComparable<Card>
        {
            public CardSuit Suit { get; }
            public CardValue Value { get; }

            public Card(CardSuit suit, CardValue value)
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
                return new Card(Suit, Value);
            }

            public static bool operator ==(Card x, Card y) => x.Equals(y);
            public static bool operator !=(Card x, Card y) => !(x == y);
            public static bool operator >(Card x, Card y) => x.Value > y.Value;
            public static bool operator <(Card x, Card y) => x.Value < y.Value;
            public static bool operator >=(Card x, Card y) => x.Value >= y.Value;
            public static bool operator <=(Card x, Card y) => x.Value <= y.Value;
        }

        public class CardSuit : IEquatable<CardSuit>
        {
            public static readonly CardSuit Hearts = new(CardColor.RED, "Hearts", "\u2665");
            public static readonly CardSuit Diamonds = new(CardColor.RED, "Diamonds", "\u2666");
            public static readonly CardSuit Clubs = new(CardColor.BLACK, "Clubs", "\u2663");
            public static readonly CardSuit Spades = new(CardColor.BLACK, "Spades", "\u2660");

            public static readonly CardSuit DebugDefault = Hearts;

            public readonly CardColor _color;
            public readonly string _name;
            readonly string _shorthand;

            public CardSuit(CardColor color, string name, string shorthand)
            {
                _color = color;
                _name = name;
                _shorthand = shorthand;
            }

            public override string ToString()
            {
                return _shorthand;
            }

            public bool Equals(CardSuit other)
            {
                if (other is null) { return false; }
                if (ReferenceEquals(this, other)) { return true; }
                return _shorthand == other._shorthand;
            }
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

        public enum CardColor : byte
        {
            RED = 0,
            BLACK = 1
        }
    }
}