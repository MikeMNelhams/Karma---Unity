using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic.BasicBoard
{
    public class BoardTestFactory
    {
        public static BasicBoardParams BotVotingTestBoard()
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

        public static BasicBoardParams BotTestScenario1()
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
    }
}
