using System;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Cards;
using KarmaLogic.Players;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public class BasicBoardPlayerParams
    {
        [SerializeField] protected List<Card> _handCards;
        [SerializeField] protected List<Card> _karmaUpCards;
        [SerializeField] protected List<Card> _karmaDownCards;
        [SerializeField] protected bool _isPlayableCharacter;

        public List<Card> HandCards { get => _handCards; }
        public List<Card> KarmaUpCards { get { return _karmaUpCards; } }
        public List<Card> KarmaDownCards { get { return _karmaDownCards; } }
        public bool IsPlayableCharacter { get { return _isPlayableCharacter; } }

        public BasicBoardPlayerParams(List<int> handValues = null, List<int> karmaUpValues = null, 
            List<int> karmaDownValues = null, bool isPlayableCharacter = false, CardSuit suit = null)
        {
            _handCards = new List<Card>();
            _karmaUpCards = new List<Card>();
            _karmaDownCards = new List<Card>();
            _isPlayableCharacter = isPlayableCharacter;

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (handValues != null ) { _handCards.AddRange(CardsFromValues(handValues, defaultSuit)); }

            if (karmaUpValues != null) { _karmaUpCards.AddRange(CardsFromValues(karmaUpValues, defaultSuit)); }

            if (karmaDownValues != null) { _karmaDownCards.AddRange(CardsFromValues(karmaDownValues, defaultSuit)); }
        }

        public BasicBoardPlayerParams(List<List<int>> playerCardValues = null, bool isPlayableCharacter = false, 
            CardSuit suit = null)
        {
            _handCards = new List<Card>();
            _karmaUpCards = new List<Card>();
            _karmaDownCards = new List<Card>();
            _isPlayableCharacter = isPlayableCharacter;

            if (playerCardValues == null) { return; }

            if (playerCardValues.Count != 3) { throw new NotSupportedException("Invalid number of player card values!"); }

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            _handCards.AddRange(CardsFromValues(playerCardValues[0], defaultSuit));
            _karmaUpCards.AddRange(CardsFromValues(playerCardValues[1], defaultSuit));
            _karmaDownCards.AddRange(CardsFromValues(playerCardValues[2], defaultSuit));
        }

        public BasicBoardPlayerParams(List<Card> handCards, List<Card> karmaUpCards, List<Card> karmaDownCards, bool isPlayableCharacter = false)
        {
            _handCards = handCards;
            _karmaUpCards = karmaUpCards;
            _karmaDownCards = karmaDownCards;
            _isPlayableCharacter = isPlayableCharacter;
        }

        public BasicBoardPlayerParams(Player player, bool isPlayableCharacter = false)
        {
            _handCards = player.Hand.ToList();
            _karmaUpCards = player.KarmaUp.ToList();
            _karmaDownCards = player.KarmaDown.ToList();
            _isPlayableCharacter = isPlayableCharacter;
        }

        public Player ToPlayer()
        {
            return new Player(new Hand(_handCards), new CardsList(_karmaDownCards), new CardsList(_karmaUpCards));
        } 

        List<Card> CardsFromValues(List<int> values, CardSuit suit)
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