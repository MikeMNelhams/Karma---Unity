using KarmaLogic.Cards;
using KarmaLogic.Players;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic.BasicBoard
{
    public class BasicBoardFactory
    {
        public static BasicBoardParams RandomStartAllPlayable(int numberOfPlayers, int numberOfJokers = 1, int whoStarts = 0)
        {
            List<CardSuit> cardSuits = new()
            {
                CardSuit.Hearts,
                CardSuit.Diamonds,
                CardSuit.Clubs,
                CardSuit.Spades
            };

            CardsList jokers = new();
            for (int i = 0; i < numberOfJokers; i++)
            {
                CardSuit suit = cardSuits[i % cardSuits.Count];
                jokers.Add(new Card(CardValue.JOKER, suit));
            }

            CardsList deck = new(); 

            // 1-4 players = 1 deck. 5-8 = 2 decks, etc. 
            int numberOfDecks = (int)Mathf.Floor((numberOfPlayers - 1.0f) / 4.0f) + 1;
            UnityEngine.Debug.Log("Playing with: " + numberOfDecks + " decks!");
            for (int d = 0; d < numberOfDecks; d++)
            {
                foreach (CardSuit suit in cardSuits)
                {
                    for (int i = 2; i < 15; i++)
                    {
                        deck.Add(new Card((CardValue)i, suit));
                    }
                }
            }

            deck.Shuffle();

            List<CardsList> playerKarmaDowns = new();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                playerKarmaDowns.Add(deck.PopMultiple(indicesToPop));
            }

            deck.Add(jokers);
            deck.Shuffle();

            List<CardsList> playerKarmaUps = new();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                playerKarmaUps.Add(deck.PopMultiple(indicesToPop));
            }

            List<Hand> playerHands = new();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                Hand hand = new(deck.PopMultiple(indicesToPop));
                hand.Sort();
                playerHands.Add(hand); ;
            }

            List<BasicBoardCharacterParams> playerParams = new();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerParams.Add(new BasicBoardPlayerParams(new Player(playerHands[i], playerKarmaUps[i], playerKarmaDowns[i])));
            }

            return new BasicBoardParams(playerParams, deck.ToList(), new List<Card>(), new List<Card>(), whoStarts: whoStarts);
        }

        public static BasicBoardParams RandomStartSingleplayerDefault()
        {
            List<CardSuit> cardSuits = new()
            {
                CardSuit.Hearts,
                CardSuit.Diamonds,
                CardSuit.Clubs,
                CardSuit.Spades
            };

            CardsList jokers = new();
            for (int i = 0; i < 1; i++)
            {
                CardSuit suit = cardSuits[i % cardSuits.Count];
                jokers.Add(new Card(CardValue.JOKER, suit));
            }

            CardsList deck = new();
            for (int d = 0; d < 1; d++)
            {
                foreach (CardSuit suit in cardSuits)
                {
                    for (int i = 2; i < 15; i++)
                    {
                        deck.Add(new Card((CardValue)i, suit));
                    }
                }
            }

            deck.Shuffle();

            List<CardsList> playerKarmaDowns = new();
            for (int i = 0; i < 4; i++)
            {
                int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                playerKarmaDowns.Add(deck.PopMultiple(indicesToPop));
            }

            deck.Add(jokers);
            deck.Shuffle();

            List<CardsList> playerKarmaUps = new();
            for (int i = 0; i < 4; i++)
            {
                int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                playerKarmaUps.Add(deck.PopMultiple(indicesToPop));
            }

            List<Hand> playerHands = new();
            for (int i = 0; i < 4; i++)
            {
                int[] indicesToPop = new int[3] { i * 3, i * 3 + 1, i * 3 + 2 };
                Hand hand = new(deck.PopMultiple(indicesToPop));
                hand.Sort();
                playerHands.Add(hand); ;
            }

            List<BasicBoardCharacterParams> playerParams = new()
            {
                new BasicBoardPlayerParams(new Player(playerHands[0], playerKarmaUps[0], playerKarmaDowns[0]), true)
            };

            for (int i = 1; i < 4; i++)
            {
                playerParams.Add(new BasicBoardBotParams(new Player(playerHands[i], playerKarmaUps[i], playerKarmaDowns[i])));
            }

            return new BasicBoardParams(playerParams, deck.ToList(), new List<Card>(), new List<Card>());
        }
    }
}

