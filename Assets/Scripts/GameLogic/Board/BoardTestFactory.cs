using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Cards;
using KarmaLogic.Players;

namespace KarmaLogic.BasicBoard
{
    public class BoardTestFactory
    {
        public static BasicBoardParams BotQueenCombo()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 12, 12, 12}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 2, 5, 12, 12 }, new() { 3, 3, 3}, new() { } },
                new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                new() { new() { 2, 4, 5, 12, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { 7, 6, 5, 4, 4 };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotJokerCombo()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 2, 5, 12, 14 }, new() { 3, 3, 3}, new() { } },
                new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 15, 2 }, new() { 2, 13, 9 } },
                new() { new() { }, new() { }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 3, 4, 6, 14 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotVotingTestBoard1()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 12, 13, 14, 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { }, new() {}, new() { } },
                new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                new() { new() { }, new() { }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 7 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotVotingTestBoard2()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { }, new() { }, new() { } },
                new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 15, 2 }, new() { 2, 13, 9 } },
                new() { new() { }, new() { }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 3, 4, 6, 14 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestFullHand()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 12, 13, 14, 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 2, 5, 12, 12 }, new() { 3, 3, 3}, new() { } },
                new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                new() { new() { 2, 4, 5, 12, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { 10, 11, 12 };
            List<int> playCardValues = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 7 };
            List<int> burnCardValues = new() { 5, 6 };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestLeftHandRotate()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 5, 6, 7}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 2, 2, 2 }, new() { 3, 3, 3}, new() { } },
                new() { new() { 3, 3, 3 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                new() { new() { 4, 4, 4 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { 10, 11, 12 };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { 5, 6 };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues, Board.BoardTurnOrder.LEFT);
        }

        public static BasicBoardParams BotTestGameWonNoVote()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 3, 4}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 3, 4, 5}, new() { }, new() { } },
                new() { new() { 2, 4, 5, 12}, new() { }, new() { 2, 13, 9 } },
                new() { new() { }, new() { }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 3, 4, 6, 14 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestPotentialWinnerIsSkippedInUnwonGame()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 3, 4}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 3, 4, 5}, new() { }, new() { 15 } },
                new() { new() { 2, 4, 5}, new() { }, new() { } },
                new() { new() { }, new() { }, new() { } } // Should be skipped
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 3, 4, 6, 14, 2, 3 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestMultipleSeparateCardGiveaways()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 2, 2, 2, 2, 4, 4 }, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { 12, 13, 13, 13 }, new() { 2, 2, 2 }, new() { 15 } },
                new() { new() { 2, 2, 2, 2, 2, 4, 4 }, new() { 2, 2, 2 }, new() { } },
                new() { new() { 2, 2, 2, 2, 2, 4, 4 }, new() { 2, 2, 2 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { 12 };
            List<int> playCardValues = new() { 2, 2, 2, 2, 2, 2, 2 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestQueenComboLastCardToWin()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 2, 2 }, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { }, new() { }, new() { 12 } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 2 }, new() { } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 2 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 2, 2, 2, 2, 2, 2 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestQueenComboLastCardWithJokerInPlay()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 2, 2, 2 }, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { }, new() { }, new() { 12 } },
                new() { new() { 2, 2, 2 }, new() { 2, 15, 2 }, new() { 3 } },
                new() { new() { 2, 2, 2 }, new() { 15, 2, 2 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 2, 2, 2, 2, 2, 2, 2 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestValidJokerAsLastCardToWin()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 14 }, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                new() { new() { }, new() { }, new() { 15 } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 3 }, new() { 3 } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 7 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestJokerAsAceLastCardToWin()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 15 }, new() { }, new() { } },
                new() { new() { 2, 2, 2}, new() { 4, 4, 4 }, new() { } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 3 }, new() { 3 } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 7 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestGettingJokered()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 15 }, new() { 2, 2, 2 }, new() { 2, 15 } },
                new() { new() { }, new() { }, new() { } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 3 }, new() { 3 } },
                new() { new() { 2, 2, 2 }, new() { 2, 2, 7 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { 14 };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestAllPlayersNoActionsGameEnds()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 15 }, new() { }, new() { 14, 15 } },
                new() { new() { }, new() { }, new() { } },
                new() { new() { 15 }, new() { 2, 2, 3 }, new() { 3 } },
                new() { new() { 15 }, new() { 2, 2, 7 }, new() { } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestAceNoHandDoesNotCrash()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { }, new() { 14, 14 }, new() { 14, 15 } },
                new() { new() { 2, 5, 6 }, new() { 11, 12, 13 }, new() { 5, 4, 11 } },
                new() { new() { 4, 4, 4 }, new() { 9, 3, 14 }, new() { 6, 6, 6 } },
                new() { new() { 2, 10, 11 }, new() { 2, 4, 4 }, new() { 13, 13, 13 } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestAceAndFive()
        {
            List<List<List<int>>> playerCardValues = new()
            {
                new() { new() { 14 }, new() { 2, 2, 2 }, new() { 14, 15 } },
                new() { new() { 2, 5, 6 }, new() { 11, 12, 13 }, new() { 5, 4, 11 } },
                new() { new() { 5, 14 }, new() { 9, 3, 14 }, new() { 6, 6, 6 } },
                new() { new() { 2, 10, 11 }, new() { 2, 4, 4 }, new() { 13, 13, 13 } }
            };

            List<BasicBoardPlayerParams> players = new();

            foreach (List<List<int>> playerValues in playerCardValues)
            {
                players.Add(new BasicBoardPlayerParams(playerValues, false));
            }

            List<int> drawCardValues = new() { };
            List<int> playCardValues = new() { };
            List<int> burnCardValues = new() { };

            return new BasicBoardParams(players, drawCardValues, playCardValues, burnCardValues);
        }

        public static BasicBoardParams BotTestRandomStart(int numberOfPlayers, int numberOfJokers=1, int whoStarts=0)
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

            List<BasicBoardPlayerParams> playerParams = new();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerParams.Add(new BasicBoardPlayerParams(new Player(playerHands[i], playerKarmaUps[i], playerKarmaDowns[i]), false));
            }

            return new BasicBoardParams(playerParams, deck.ToList(), new List<Card>(), new List<Card>(), whoStarts: whoStarts);
        }
    }
}
