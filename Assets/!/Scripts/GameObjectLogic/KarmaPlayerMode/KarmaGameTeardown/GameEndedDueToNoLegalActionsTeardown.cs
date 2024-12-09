using System;

namespace KarmaPlayerMode.GameTeardown
{
    public class GameEndedDueToNoLegalActionsTeardown : IKarmaPlayerModeTeardown
    {
        public void Apply(KarmaPlayerMode playerMode)
        {
            playerMode.IsGameOver = true;
            playerMode.IsGameWon = false;
            playerMode.IsGameOverDueToNoLegalActions = true;
            UnityEngine.Debug.LogWarning("Game has been ended early, as no legal actions can be made on the board.\nGame Ranks:\n" +
                string.Join(Environment.NewLine, playerMode.GameRanks));
        }
    }
}

