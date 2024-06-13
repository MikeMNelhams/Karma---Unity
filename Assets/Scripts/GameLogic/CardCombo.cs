using System.Collections.Generic;
using Karma.Board;
using Karma.Controller;

namespace Karma
{
    namespace Cards
    {
        public abstract class CardCombo
        {
            public CardsList Cards { get; protected set; }
            public IController Controller { get; protected set; }
            protected Dictionary<CardValue, int> _counts;

            public CardCombo(CardsList cards, IController controller, Dictionary<CardValue, int> counts)
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
        }
    }
}