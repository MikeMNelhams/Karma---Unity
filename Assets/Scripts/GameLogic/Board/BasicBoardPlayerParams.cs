using System;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public class BasicBoardPlayerParams
    {
        [SerializeField] List<int> _handValues;
        [SerializeField] List<int> _karmaUpValues;
        [SerializeField] List<int> _karmaDownValues;
        [SerializeField] bool _isPlayableCharacter;

        public List<int> HandValues { get { return _handValues; } }
        public List<int> KarmaUpValues { get { return _karmaUpValues; } }
        public List<int> KarmaDownValues { get { return _karmaDownValues; } }
        public bool IsPlayableCharacter { get { return _isPlayableCharacter; } }

        public BasicBoardPlayerParams(List<int> handValues = null, List<int> karmaUpValues = null, List<int> karmaDownValues = null, bool isPlayableCharacter = false)
        {
            _handValues = new List<int>();
            _karmaUpValues = new List<int>();
            _karmaDownValues = new List<int>();
            _isPlayableCharacter = isPlayableCharacter;

            if (handValues != null ) { _handValues.AddRange(handValues); }

            if (karmaUpValues != null) { _karmaUpValues.AddRange(karmaUpValues); }

            if (karmaDownValues != null) { _karmaDownValues.AddRange(karmaDownValues); }
        }

        public BasicBoardPlayerParams(List<List<int>> playerCardValues = null, bool isPlayableCharacter = false)
        {
            _handValues = new List<int>();
            _karmaUpValues = new List<int>();
            _karmaDownValues = new List<int>();
            _isPlayableCharacter = isPlayableCharacter;

            if (playerCardValues == null) { return; }

            if (playerCardValues.Count != 3) { throw new NotSupportedException("Invalid number of player card values!"); }

            _handValues.AddRange(playerCardValues[0]);
            _handValues.AddRange(playerCardValues[1]);
            _handValues.AddRange(playerCardValues[2]);
        }
    }
}