using Karma.Cards;
using Karma.Players;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karma
{
    namespace BasicBoard
    {
        public class BoardFactory
        {
            public static BasicBoard RandomStart(int numberOfPlayers, int numberOfJokers = 1, int whoStarts = 0)
            {
                List<CardSuit> cardSuits = new List<CardSuit>();
                cardSuits.Add(CardSuit.Hearts);
                cardSuits.Add(CardSuit.Diamonds);
                cardSuits.Add(CardSuit.Clubs);
                cardSuits.Add(CardSuit.Spades);

                CardsList jokers = new CardsList();
                foreach (CardSuit suit in cardSuits) { jokers.Add(new Card(suit, CardValue.JOKER)); }
                CardsList deck = new CardsList();
                foreach (CardSuit suit in cardSuits)
                {
                    for (int i = 2; i < 16; i++)
                    {
                        deck.Add(new Card(suit, (CardValue)i));
                    }
                }
                deck.Shuffle();

                List<CardsList> playerKarmaDowns = new ();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    int[] indicesToPop = new int[3] {i * 3, i * 3 + 1, i * 3 + 2};
                    playerKarmaDowns.Add(deck.PopMultiple(indicesToPop));
                }

                deck.Add(jokers);
                deck.Shuffle();

                List<CardsList> playerKarmaUps = new ();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2};
                    playerKarmaUps.Add(deck.PopMultiple(indicesToPop));
                }

                List<CardsList> playerHands = new ();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                    CardsList hand = deck.PopMultiple(indicesToPop);
                    hand.Sort();
                    playerHands.Add(hand);;
                }
                List<Player> players = new List<Player>();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players.Add(new Player(playerHands[i], playerKarmaUps[i], playerKarmaDowns[i]));
                }
                return new BasicBoard(players, new CardPile(deck));
            }
        }
    }
}
