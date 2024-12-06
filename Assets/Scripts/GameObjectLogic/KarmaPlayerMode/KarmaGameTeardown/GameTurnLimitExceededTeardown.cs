using System;

namespace KarmaPlayerMode.GameTeardown
{
    public class GameTurnLimitExceededTeardown : IKarmaPlayerModeTeardown
    {
        public void Apply(KarmaPlayerMode playerMode)
        {
            playerMode.IsGameOver = true;
            playerMode.IsGameWon = false;
            UnityEngine.Debug.LogWarning("Game has finished by turn limit exceeded. Game ranks: " + string.Join(Environment.NewLine, 
                playerMode.GameRanks));
        }
    }
}