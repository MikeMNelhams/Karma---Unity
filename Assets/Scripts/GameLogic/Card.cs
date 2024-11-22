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
            [SerializeField] CardSuit _suit;
            [SerializeField] CardValue _value;

            public CardSuit Suit { get => _suit; set => _suit = value; }
            public CardValue Value { get => _value; set => _value = value; }

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

        [System.Serializable]
        public class CardSuit : IEquatable<CardSuit>
        {
            // TODO Switch from tuple to a class!
            static readonly Dictionary<CardSuitType, Tuple<CardColor, string, string>> _suitDataMap = new()
            {
                {CardSuitType.HEARTS, new (CardColor.RED, "Hearts", "\u2665") },
                {CardSuitType.DIAMONDS, new (CardColor.RED, "Diamonds", "\u2666") },
                {CardSuitType.CLUBS, new (CardColor.BLACK, "Clubs", "\u2663") },
                {CardSuitType.SPADES, new (CardColor.BLACK, "Spades", "\u2660") }
            };

            public static readonly CardSuit Hearts = new (CardSuitType.HEARTS);
            public static readonly CardSuit Diamonds = new(CardSuitType.DIAMONDS);
            public static readonly CardSuit Clubs = new (CardSuitType.CLUBS);
            public static readonly CardSuit Spades = new(CardSuitType.SPADES);

            public static readonly CardSuit DebugDefault = Hearts;

            [SerializeField] protected CardSuitType _suit;

            public CardColor Color {get; private set;}
            public string Name { get; private set;}
            protected string Shorthand { get; private set;}

            public CardSuit(CardSuitType suit)
            {
                _suit = suit;

                Tuple<CardColor, string, string> cardSuitData = _suitDataMap[suit];
                Color = cardSuitData.Item1;
                Name = cardSuitData.Item2;
                Shorthand = cardSuitData.Item3;
            }

            public override string ToString()
            {
                return Shorthand;
            }

            public bool Equals(CardSuit other)
            {
                if (other is null) { return false; }
                if (ReferenceEquals(this, other)) { return true; }
                return Shorthand == other.Shorthand;
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

        public enum CardSuitType : byte
        {
            HEARTS,
            DIAMONDS,
            CLUBS,
            SPADES
        }
    }
}