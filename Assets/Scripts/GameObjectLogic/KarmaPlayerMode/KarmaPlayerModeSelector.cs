using KarmaLogic.BasicBoard;
using System.Collections.Generic;
using UnityEngine;
using KarmaPlayerMode.Singleplayer;
using KarmaPlayerMode.Multiplayer;

namespace KarmaPlayerMode
{
    [System.Serializable]
    public class KarmaPlayerModeSelector
    {
        [SerializeField] PlayerMode _mode;
        [SerializeField] BasicBoardParams _basicBoardParams;
        [SerializeField] List<KarmaPlayerStartInfo> _playersStartInfo;
        [SerializeField] int _basicBoardPresetSelected = -1;

        public KarmaPlayerMode Mode()
        {
            if (_basicBoardPresetSelected != -1)
            {
                return _mode switch
                {
                    PlayerMode.Singleplayer => new KarmaSingleplayer(_playersStartInfo, _basicBoardPresetSelected),
                    PlayerMode.Multiplayer => new KarmaMultiplayer(_playersStartInfo, _basicBoardPresetSelected),
                    _ => throw new KarmaPlayerModeException("Invalid starting conditions!"),
                };
            }

            return _mode switch
            {
                PlayerMode.Singleplayer => new KarmaSingleplayer(_playersStartInfo, _basicBoardParams),
                PlayerMode.Multiplayer => new KarmaMultiplayer(_playersStartInfo, _basicBoardParams),
                _ => throw new KarmaPlayerModeException("Invalid starting conditions!"),
            };
        }

        public void SetBoardPresetIndex(int presetIndex)
        {
            _basicBoardPresetSelected = presetIndex;
        }
    }

    public enum PlayerMode : int
    {
        Singleplayer,
        Multiplayer
    }
}