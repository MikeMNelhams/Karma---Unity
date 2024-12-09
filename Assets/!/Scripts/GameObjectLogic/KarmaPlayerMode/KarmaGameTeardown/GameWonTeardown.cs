using System;

namespace KarmaPlayerMode.GameTeardown
{
    public class GameWonTeardown : IKarmaPlayerModeTeardown
    {
        public void Apply(KarmaPlayerMode playerMode)
        {
            playerMode.IsGameOver = true;
            playerMode.IsGameWon = true;
            UnityEngine.Debug.LogWarning("Game has finished. Game ranks: " + string.Join(Environment.NewLine, playerMode.GameRanks));
        }
    }

}