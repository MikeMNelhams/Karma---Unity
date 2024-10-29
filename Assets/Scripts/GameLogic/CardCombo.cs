using System;
using System.Collections.Generic;
using KarmaLogic.Board;
using KarmaLogic.Controller;

namespace KarmaLogic
{
    namespace Cards
    {
        public abstract class CardCombo : IEquatable<CardCombo>
        {
            public CardsList Cards { get; protected set; }
            public Controller.Controller Controller { get; protected set; }
            protected Dictionary<CardValue, int> _counts;

            public CardCombo(CardsList cards, Controller.Controller controller, Dictionary<CardValue, int> counts)
            {
                Cards = cards;
                Controller = controller;
                _counts = counts;
            }

            public int Length { get { return Cards.Count; } }

            public abstract void Apply(IBoard board);

            public override string ToString()
            {
                return GetType().Name + "(" + Cards.ToString() + ")";
            }

            public override int GetHashCode()
            {
                return Cards.GetHashCode();
            }

            public bool Equals(CardCombo other)
            {
                return Cards.Equals(other.Cards);
            }
        }
    }
}