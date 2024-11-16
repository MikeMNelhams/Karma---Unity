using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic
{
    namespace BasicBoard
    {
        public class BoardTestFactory
        {
            public static BasicBoard BotVotingTestBoard()
            {
                List<List<List<int>>> playerCardValues = new()
                {
                    new() { new() { 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 12, 13, 14, 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                    new() { new() { }, new() {}, new() { } },
                    new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                    new() { new() { }, new() { }, new() { } }
                };

                List<int> drawCardValues = new() { };
                List<int> playCardValues = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 7 };
                List<int> burnCardValues = new() { };

                return BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, 0);
            }

            public static BasicBoard BotQueenCombo()
            {
                List<List<List<int>>> playerCardValues = new()
                {
                    new() { new() { 12, 12, 12}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                    new() { new() { 2, 5, 12, 12 }, new() { 3, 3, 3}, new() { } },
                    new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                    new() { new() { 2, 4, 5, 12, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
                };

                List<int> drawCardValues = new() { 7, 6, 5, 4, 4 };
                List<int> playCardValues = new() { };
                List<int> burnCardValues = new() { };

                return BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, 0);
            }

            public static BasicBoard BotJokerCombo()
            {
                List<List<List<int>>> playerCardValues = new()
                {
                    new() { new() { 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                    new() { new() { 2, 5, 12, 14 }, new() { 3, 3, 3}, new() { } },
                    new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 15, 2 }, new() { 2, 13, 9 } },
                    new() { new() { }, new() { }, new() { } }
                };

                List<int> drawCardValues = new() { };
                List<int> playCardValues = new() { 2, 3, 4, 6, 14 };
                List<int> burnCardValues = new() { };

                return BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, 0);
            }

            public static BasicBoard BotVotingTestBoard2()
            {
                List<List<List<int>>> playerCardValues = new()
                {
                    new() { new() { 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                    new() { new() { }, new() { }, new() { } },
                    new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 15, 2 }, new() { 2, 13, 9 } },
                    new() { new() { }, new() { }, new() { } }
                };

                List<int> drawCardValues = new() { };
                List<int> playCardValues = new() { 2, 3, 4, 6, 14 };
                List<int> burnCardValues = new() { };

                return BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, 0);
            }

            public static BasicBoard BotTestScenario1()
            {
                List<List<List<int>>> playerCardValues = new()
                {
                    new() { new() { 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 12, 13, 14, 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
                    new() { new() { 2, 5, 12, 12 }, new() { 3, 3, 3}, new() { } },
                    new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
                    new() { new() { 2, 4, 5, 12, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
                };

                List<int> drawCardValues = new() { 10, 11, 12 };
                List<int> playCardValues = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 7 };
                List<int> burnCardValues = new() { };

                return BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, 0);
            }
        }
    }
}
