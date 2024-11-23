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
        [SerializeField] bool _useBasicBoardPreset = true;
        [SerializeField] int _basicBoardPresetSelected = -1;

        public KarmaPlayerMode Mode()
        {
            UnityEngine.Debug.Log("Using a board preset?: " + _useBasicBoardPreset);

            if (_useBasicBoardPreset)
            {
                return _mode switch
                {
                    PlayerMode.Singleplayer => new KarmaSingleplayer(_basicBoardPresetSelected),
                    PlayerMode.Multiplayer => new KarmaMultiplayer(_basicBoardPresetSelected),
                    _ => throw new KarmaPlayerModeException("Invalid starting conditions!"),
                };
            }

            return _mode switch
            {
                PlayerMode.Singleplayer => new KarmaSingleplayer(_basicBoardParams),
                PlayerMode.Multiplayer => new KarmaMultiplayer(_basicBoardParams),
                _ => throw new KarmaPlayerModeException("Invalid starting conditions!"),
            };
        }

        public void SetBoardPresetIndex(int presetIndex)
        {
            _basicBoardPresetSelected = presetIndex;
        }

        public void SetIsUsingBoardPresets(bool isUsingBoardPresets)
        {
            _useBasicBoardPreset = isUsingBoardPresets;
        }
    }

    public enum PlayerMode : int
    {
        Singleplayer,
        Multiplayer
    }
}