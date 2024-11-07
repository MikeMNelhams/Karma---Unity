using System;
using System.Collections.Generic;
using KarmaLogic.Board;

namespace KarmaLogic
{
    namespace Cards
    {
        public abstract class CardCombo : IEquatable<CardCombo>
        {
            public CardsList Cards { get; protected set; }
            protected Dictionary<CardValue, int> _counts;

            public delegate void OnFinishApplyComboListener();

            readonly Queue<OnFinishApplyComboListener> _onFinishApplyComboListeners;

            public CardCombo(CardsList cards, Dictionary<CardValue, int> counts)
            {
                Cards = cards;
                _counts = counts;
                _onFinishApplyComboListeners = new Queue<OnFinishApplyComboListener>();
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

            public void RegisterOnFinishApplyComboListener(OnFinishApplyComboListener listener)
            {
                _onFinishApplyComboListeners.Enqueue(listener);
            }

            protected void TriggerOnFinishApplyComboListeners()
            {
                while (_onFinishApplyComboListeners.Count > 0)
                {
                    OnFinishApplyComboListener listener = _onFinishApplyComboListeners.Dequeue() 
                        ?? throw new NullReferenceException("Null listener reference when applying combo!");
                    listener.Invoke();
                }
            }
        }
    }
}