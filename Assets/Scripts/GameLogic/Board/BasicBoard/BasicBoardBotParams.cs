using System;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using KarmaLogic.Bots;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public enum BotType : byte
    {
        IntegrationTestBot,
        JackTestBot,
        DecentlyFunBot
    }

    [System.Serializable]
    public class BasicBoardBotParams : BasicBoardCharacterParams
    {
        public override bool IsPlayableCharacter { get => false; }
        public override bool AreLegalHintsEnabled { get => false; }

        [SerializeField] protected int _selectedBotType = (int) BotType.IntegrationTestBot;

        public BasicBoardBotParams(List<int> handValues, List<int> karmaUpValues, List<int> karmaDownValues, CardSuit suit = null, 
            BotType botType = BotType.IntegrationTestBot)
        {
            _selectedBotType = (int)botType;

            _handCards = new List<Card>();
            _karmaUpCards = new List<Card>();
            _karmaDownCards = new List<Card>();

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (handValues != null) { _handCards.AddRange(CardsFromValues(handValues, defaultSuit)); }
            if (karmaUpValues != null) { _karmaUpCards.AddRange(CardsFromValues(karmaUpValues, defaultSuit)); }
            if (karmaDownValues != null) { _karmaDownCards.AddRange(CardsFromValues(karmaDownValues, defaultSuit)); }
        }

        public BasicBoardBotParams(List<List<int>> playerCardValues = null, CardSuit suit = null, BotType botType = BotType.IntegrationTestBot)
        {
            _selectedBotType = (int) botType;

            _handCards = new List<Card>();
            _karmaUpCards = new List<Card>();
            _karmaDownCards = new List<Card>();

            if (playerCardValues == null) { return; }

            if (playerCardValues.Count != 3) { throw new NotSupportedException("Invalid number of player card values!"); }

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            _handCards.AddRange(CardsFromValues(playerCardValues[0], defaultSuit));
            _karmaUpCards.AddRange(CardsFromValues(playerCardValues[1], defaultSuit));
            _karmaDownCards.AddRange(CardsFromValues(playerCardValues[2], defaultSuit));
        }

        public BasicBoardBotParams(List<Card> handCards, List<Card> karmaUpCards, List<Card> karmaDownCards, BotType botType = BotType.IntegrationTestBot)
        {
            _selectedBotType = (int)botType;

            _handCards = handCards;
            _karmaUpCards = karmaUpCards;
            _karmaDownCards = karmaDownCards;
        }

        public BasicBoardBotParams(Player player, BotType botType)
        {
            _selectedBotType = (int) botType;

            _handCards = player.Hand.ToList();
            _karmaUpCards = player.KarmaUp.ToList();
            _karmaDownCards = player.KarmaDown.ToList();
        }

        public BotBase Bot(string name, float delay)
        {
            return (BotType)_selectedBotType switch
            {
                BotType.IntegrationTestBot => new IntegrationTestBot(name, delay),
                BotType.DecentlyFunBot => new DecentlyFunBot(name, delay),
                BotType.JackTestBot => new JackTestBot(name, delay),
                _ => throw new UnsupportedBotTypeException(),
            };
        }
    }
}