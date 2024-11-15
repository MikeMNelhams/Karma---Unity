using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Board;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public class BasicBoardParams
    {
        // TODO FINISH THIS + CUSTOM EDITOR :/
        [SerializeField] List<BasicBoardPlayerParams> _playerCardValues;
        [SerializeField] List<int> _drawPileValues;
        [SerializeField] List<int> _burnPileValues;
        [SerializeField] List<int> _playPileValues;

        [SerializeField] BoardTurnOrder _turnOrder = BoardTurnOrder.RIGHT;
        [SerializeField] BoardPlayOrder _playOrder;
        [SerializeField] bool _handsAreFlipped;
        [SerializeField] int _effectMultiplier = 1;
        [SerializeField] int _whoStarts;
        [SerializeField] bool _hasBurnedThisTurn;
        [SerializeField] int _turnsPlayed;
        [SerializeField] int _turnLimit;

        public List<BasicBoardPlayerParams> PlayerCardValues {  get { return _playerCardValues; } }
        public List<int> DrawPileValues { get { return _drawPileValues; } }
        public List<int> BurnPileValues { get { return _burnPileValues; } }
        public List<int> PlayPileValues { get { return _playPileValues; } }
        public BoardTurnOrder BoardTurnOrder { get { return _turnOrder; } }
        public BoardPlayOrder BoardPlayOrder { get { return _playOrder; } }
        public bool HandsAreFlipped { get {  return _handsAreFlipped; } }
        public int EffectMultiplier { get { return _effectMultiplier; } }
        public int WhoStarts { get { return _whoStarts; } }
        public bool HasBurnedThisTurn { get { return _hasBurnedThisTurn; } }
        public int TurnsPlayed { get { return _turnsPlayed; } }

        public int TurnLimit { get { return _turnLimit; } }

        public BasicBoardParams(List<BasicBoardPlayerParams> playerCardValues = null, List<int> drawPileValues = null, 
            List<int> burnPileValues = null, List<int> playPileValues = null, 
            BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
            bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
            bool hasBurnedThisTurn = false, int turnsPlayed = 0)
        {
            _playerCardValues = new List<BasicBoardPlayerParams>();
            _drawPileValues = new List<int>();
            _burnPileValues = new List<int>();
            _playPileValues = new List<int>();

            if (playerCardValues != null) { _playerCardValues.AddRange(playerCardValues); }
            if (drawPileValues != null) { _drawPileValues.AddRange(drawPileValues); }
            if (burnPileValues != null) { _burnPileValues.AddRange(burnPileValues); }
            if (playPileValues != null) { _playPileValues.AddRange(playPileValues); }

            _turnOrder = turnOrder;
            _playOrder = playOrder;
            _handsAreFlipped = handsAreFlipped;
            _effectMultiplier = effectMultiplier;
            _whoStarts = whoStarts;
            _hasBurnedThisTurn = hasBurnedThisTurn;
            _turnsPlayed = turnsPlayed;
        }
    }
}

