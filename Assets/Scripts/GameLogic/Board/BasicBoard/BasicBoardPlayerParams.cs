using System;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Cards;
using KarmaLogic.Players;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public class BasicBoardPlayerParams : BasicBoardCharacterParams
    {
        [SerializeField] protected bool _areLegalHintsEnabled;

        public override bool IsPlayableCharacter { get => true; }
        public override bool AreLegalHintsEnabled { get => _areLegalHintsEnabled; }

        public BasicBoardPlayerParams(List<int> handValues, List<int> karmaUpValues = null,
            List<int> karmaDownValues = null, CardSuit suit = null, bool areLegalHintsEnabled = true)
        {
            _handCards = new List<Card>();
            _karmaUpCards = new List<Card>();
            _karmaDownCards = new List<Card>();
            _areLegalHintsEnabled = areLegalHintsEnabled;

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (handValues != null ) { _handCards.AddRange(CardsFromValues(handValues, defaultSuit)); }

            if (karmaUpValues != null) { _karmaUpCards.AddRange(CardsFromValues(karmaUpValues, defaultSuit)); }

            if (karmaDownValues != null) { _karmaDownCards.AddRange(CardsFromValues(karmaDownValues, defaultSuit)); }
        }

        public BasicBoardPlayerParams(List<List<int>> playerCardValues = null, CardSuit suit = null, bool areLegalHintsEnabled = true)
        {
            _handCards = new List<Card>();
            _karmaUpCards = new List<Card>();
            _karmaDownCards = new List<Card>();
            _areLegalHintsEnabled = areLegalHintsEnabled;

            if (playerCardValues == null) { return; }

            if (playerCardValues.Count != 3) { throw new NotSupportedException("Invalid number of player card values!"); }

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            _handCards.AddRange(CardsFromValues(playerCardValues[0], defaultSuit));
            _karmaUpCards.AddRange(CardsFromValues(playerCardValues[1], defaultSuit));
            _karmaDownCards.AddRange(CardsFromValues(playerCardValues[2], defaultSuit));
        }

        public BasicBoardPlayerParams(List<Card> handCards, List<Card> karmaUpCards, List<Card> karmaDownCards, bool areLegalHintsEnabled = true)
        {
            _handCards = handCards;
            _karmaUpCards = karmaUpCards;
            _karmaDownCards = karmaDownCards;
            _areLegalHintsEnabled = areLegalHintsEnabled;
        }

        public BasicBoardPlayerParams(Player player, bool areLegalHintsEnabled = true)
        {
            _handCards = player.Hand.ToList();
            _karmaUpCards = player.KarmaUp.ToList();
            _karmaDownCards = player.KarmaDown.ToList();
            _areLegalHintsEnabled = areLegalHintsEnabled;
        }
    }
}