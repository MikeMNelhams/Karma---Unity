using System.Collections.Generic;
using System;
using UnityEngine;

namespace KarmaLogic.Cards
{
    public enum CardSuitType : byte
    {
        HEARTS,
        DIAMONDS,
        CLUBS,
        SPADES
    }

    public enum CardColor : byte
    {
        RED = 0,
        BLACK = 1
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

        public static readonly CardSuit Hearts = new(CardSuitType.HEARTS);
        public static readonly CardSuit Diamonds = new(CardSuitType.DIAMONDS);
        public static readonly CardSuit Clubs = new(CardSuitType.CLUBS);
        public static readonly CardSuit Spades = new(CardSuitType.SPADES);

        public static readonly CardSuit DebugDefault = Hearts;

        [SerializeField] protected CardSuitType _suit;

        public CardColor Color { get => _suitDataMap[_suit].Item1; }
        public string Name { get => _suitDataMap[_suit].Item2; }
        protected string Shorthand { get => _suitDataMap[_suit].Item3; }

        public CardSuit(CardSuitType suit)
        {
            _suit = suit;
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
}
