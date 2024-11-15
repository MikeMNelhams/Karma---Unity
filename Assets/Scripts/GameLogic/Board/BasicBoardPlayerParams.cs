using System.Collections;
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

        public List<int> HandValues { get { return _handValues; } }
        public List<int> KarmaUpValues { get { return _karmaUpValues; } }
        public List<int> KarmaDownValues { get { return _karmaDownValues; } }

        public BasicBoardPlayerParams(List<int> handValues = null, List<int> karmaUpValues = null, List<int> karmaDownValues = null)
        {
            _handValues = new List<int>();
            _karmaUpValues = new List<int>();
            _karmaDownValues = new List<int>();

            if (handValues != null ) { _handValues.AddRange(handValues); }

            if (karmaUpValues != null) { _karmaUpValues.AddRange(karmaUpValues); }

            if (karmaDownValues != null) { _karmaDownValues.AddRange(karmaDownValues); }
        }
    }
}