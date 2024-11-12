using KarmaLogic.Board;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaPlayerMode
{
    [System.Serializable]
    public class KarmaPlayerModeSelector
    {
        [SerializeField] PlayerMode _mode;

        public KarmaPlayerMode Mode(KarmaPlayerStartInfo[] startInfo, IBoard board, List<PlayerProperties> playerProperties, int turnLimit = 100)
        {
            return _mode switch
            {
                PlayerMode.Singleplayer => new KarmaSingleplayer(startInfo, board, playerProperties, turnLimit),
                PlayerMode.Multiplayer => new KarmaMultiplayer(startInfo, board, playerProperties),
                _ => throw new KarmaPlayerModeException("Invalid starting conditions!"),
            };
        }
    }

    public enum PlayerMode : int
    {
        Singleplayer,
        Multiplayer
    }
}