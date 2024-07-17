using System;
using UnityEngine;

namespace Karma
{
    namespace Cards
    {
        public class Card : IEquatable<Card>, IComparable<Card>
        {
            public readonly CardSuit suit;
            public readonly CardValue value;

            public Card(CardSuit suit, CardValue value)
            {
                this.suit = suit;
                this.value = value;
            }

            public bool Equals(Card other)
            {
                return suit == other.suit && value == other.value;
            }

            public override bool Equals(object other)
            {
                if (other is null) return false;
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
                return new { suit, value }.GetHashCode();
            }

            public override string ToString()
            {
                string valueString;
                int valueInt = (int)value;
                if ((0 <= valueInt) && (valueInt <= 10)) { valueString = valueInt.ToString(); }
                else { valueString = value.ToString().Substring(0, 2).Up; }
                return valueString + suit.ToString();
            }

            public static bool operator ==(Card x, Card y) => x.Equals(y);
            public static bool operator !=(Card x, Card y) => !(x == y);
            public static bool operator >(Card x, Card y) => x.value > y.value;
            public static bool operator <(Card x, Card y) => x.value < y.value;
            public static bool operator >=(Card x, Card y) => x.value >= y.value;
            public static bool operator <=(Card x, Card y) => x.value <= y.value;
        }

        public class CardSuit : IEquatable<CardSuit>
        {
            public static readonly CardSuit Hearts = new(CardColor.RED, "Hearts", "\u2665");
            public static readonly CardSuit Diamonds = new(CardColor.RED, "Diamonds", "\u2666");
            public static readonly CardSuit Clubs = new(CardColor.BLACK, "Clubs", "\u2663");
            public static readonly CardSuit Spades = new(CardColor.BLACK, "Spades", "\u2660");

            public static readonly CardSuit DebugDefault = Hearts;

            public readonly CardColor color;
            public readonly String name;
            readonly String shorthand;

            public CardSuit(CardColor color, String name, String shorthand)
            {
                this.color = color;
                this.name = name;
                this.shorthand = shorthand;
            }

            public override string ToString()
            {
                return shorthand;
            }

            public bool Equals(CardSuit other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) { return true; }
                return shorthand == other.shorthand;
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