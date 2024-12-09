using System;

namespace KarmaPlayerMode.GameTeardown
{
    public class GameExitedEarlyTeardown : IKarmaPlayerModeTeardown
    {
        public void Apply(KarmaPlayerMode playerMode)
        {
            playerMode.IsGameOver = true;
            playerMode.IsGameWon = false;
            UnityEngine.Debug.LogWarning("Game has been exited. Game Ranks:\n" +
                string.Join(Environment.NewLine, playerMode.GameRanks));
        }
    }
}

