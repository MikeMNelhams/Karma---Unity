using KarmaLogic.BasicBoard;
using System;
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
        [SerializeField] int _subMode;
        [SerializeField] BasicBoardParams _basicBoardParams;
        [SerializeField] bool _useBasicBoardPreset = true;
        [SerializeField] int _basicBoardPresetSelected = -1;

        public KarmaPlayerMode Mode()
        {
            UnityEngine.Debug.Log("Using a board preset?: " + _useBasicBoardPreset);

            if (_useBasicBoardPreset)
            {
                return (_mode, _subMode) switch
                {
                    (PlayerMode.Singleplayer, (int)SinglePlayerMode.Solo) => new KarmaSingleplayerSolo(_basicBoardPresetSelected),
                    (PlayerMode.Singleplayer, (int)SinglePlayerMode.Many) => new KarmaSingleplayerMany(_basicBoardPresetSelected),
                    (PlayerMode.Multiplayer, _) => new KarmaMultiplayer(_basicBoardPresetSelected),
                    _ => throw new KarmaPlayerModeException("Invalid starting conditions!"),
                };
            }

            return (_mode, _subMode) switch
            {
                (PlayerMode.Singleplayer, (int)SinglePlayerMode.Solo) => new KarmaSingleplayerSolo(_basicBoardParams),
                (PlayerMode.Singleplayer, (int)SinglePlayerMode.Many) => new KarmaSingleplayerMany(_basicBoardParams),
                (PlayerMode.Multiplayer, _) => new KarmaMultiplayer(_basicBoardParams),
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

    public enum SinglePlayerMode : int
    {
        Solo,
        Many
    }
}