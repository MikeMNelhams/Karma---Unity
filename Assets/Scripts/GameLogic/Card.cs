using System;

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

            public Card(CardSuit suit, CardValue value, bool isFlipped)
            {
                this.suit = suit;
                this.value = value;
            }

            public bool Equals(Card other)
            {
                return suit == other.suit && value == other.value;
            }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                return Equals(obj as Card);
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
                return value.ToString() + suit.ToString();
            }

            public static bool operator ==(Card x, Card y) => x.Equals(y);
            public static bool operator !=(Card x, Card y) => !(x == y);
            public static bool operator >(Card x, Card y) => x.value > y.value;
            public static bool operator <(Card x, Card y) => x.value < y.value;
            public static bool operator >=(Card x, Card y) => x.value >= y.value;
            public static bool operator <=(Card x, Card y) => x.value <= y.value;
        }


        public class CardSuit
        {
            public static readonly CardSuit Hearts = new(CardColor.RED, "Hearts", "\u2665");
            public static readonly CardSuit Diamonds = new(CardColor.RED, "Diamonds", "\u2666");
            public static readonly CardSuit Clubs = new(CardColor.BLACK, "Clubs", "\u2663");
            public static readonly CardSuit Spades = new(CardColor.BLACK, "Spades", "\u2660");

            readonly CardColor color;
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