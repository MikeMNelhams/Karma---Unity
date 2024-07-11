using Karma.Cards;
using Karma.Players;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Karma
{
    namespace BasicBoard
    {
        public class BoardFactory
        {
            public static BasicBoard RandomStart(int numberOfPlayers, int numberOfJokers = 1, int whoStarts = 0)
            {
                List<CardSuit> cardSuits = new()
                {
                    CardSuit.Hearts,
                    CardSuit.Diamonds,
                    CardSuit.Clubs,
                    CardSuit.Spades
                };

                CardsList jokers = new ();
                for (int i = 0; i < numberOfJokers; i++)
                {
                    CardSuit suit = cardSuits[i % cardSuits.Count];
                    jokers.Add(new Card(suit, CardValue.JOKER));
                }

                CardsList deck = new ();
                foreach (CardSuit suit in cardSuits)
                {
                    for (int i = 2; i < 15; i++)
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

                List<Hand> playerHands = new ();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                    Hand hand = new (deck.PopMultiple(indicesToPop));
                    hand.Sort();
                    playerHands.Add(hand);;
                }
                List<Player> players = new ();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players.Add(new Player(playerHands[i], playerKarmaUps[i], playerKarmaDowns[i]));
                }
                return new BasicBoard(players, new CardPile(deck), new CardPile(), new PlayCardPile(), whoStarts: whoStarts);
            }

            public static BasicBoard MatrixStart(List<List<List<int>>> playerCardValues, List<int> drawPileValues, List<int> playPileValues, List<int> burnPileValues, 
                int whoStarts=0)
            {
                List<Player> players = new();

                for (int i = 0; i < playerCardValues.Count; i++)
                {
                    players.Add(new Player(playerCardValues[i]));
                }

                CardPile drawPile = new(drawPileValues, CardSuit.Hearts);
                PlayCardPile playPile = new(playPileValues, CardSuit.Hearts);
                CardPile burnPile = new(burnPileValues, CardSuit.Hearts);

                return new BasicBoard(players, drawPile, burnPile, playPile, whoStarts: whoStarts);
            }
        }
    }
}
