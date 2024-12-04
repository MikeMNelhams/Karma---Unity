using System.Collections.Generic;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using UnityEngine;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public abstract class BasicBoardCharacterParams
    {
        [SerializeField] protected List<Card> _handCards;
        [SerializeField] protected List<Card> _karmaUpCards;
        [SerializeField] protected List<Card> _karmaDownCards;

        public List<Card> HandCards { get => _handCards; }
        public List<Card> KarmaUpCards { get => _karmaUpCards; }
        public List<Card> KarmaDownCards { get => _karmaDownCards; }
        public abstract bool IsPlayableCharacter { get; }
        public abstract bool AreLegalHintsEnabled { get; }

        public Player ToPlayer()
        {
            return new Player(new Hand(HandCards), new CardsList(KarmaDownCards), new CardsList(KarmaUpCards));
        }

        protected List<Card> CardsFromValues(List<int> values, CardSuit suit)
        {
            List<Card> cards = new();

            foreach (int value in values)
            {
                cards.Add(new Card((CardValue)value, suit));
            }

            return cards;
        }
    }
}