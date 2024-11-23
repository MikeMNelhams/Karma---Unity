using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Board;
using KarmaLogic.Cards;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public class BasicBoardParams
    {
        [SerializeField] List<BasicBoardPlayerParams> _playersParams;
        [SerializeField] List<Card> _drawPileCards;
        [SerializeField] List<Card> _burnPileCards;
        [SerializeField] List<Card> _playPileCards;

        [SerializeField] BoardTurnOrder _turnOrder = BoardTurnOrder.RIGHT;
        [SerializeField] BoardPlayOrder _playOrder;
        [SerializeField] bool _handsAreFlipped;
        [SerializeField] int _effectMultiplier = 1;
        [SerializeField] int _whoStarts;
        [SerializeField] bool _hasBurnedThisTurn;
        [SerializeField] int _turnsPlayed;
        [SerializeField] int _turnLimit;

        public List<BasicBoardPlayerParams> PlayersParams {  get { return _playersParams; } }
        public BoardTurnOrder BoardTurnOrder { get { return _turnOrder; } }
        public BoardPlayOrder BoardPlayOrder { get { return _playOrder; } }
        public bool HandsAreFlipped { get {  return _handsAreFlipped; } }
        public int EffectMultiplier { get { return _effectMultiplier; } }
        public int WhoStarts { get { return _whoStarts; } }
        public bool HasBurnedThisTurn { get { return _hasBurnedThisTurn; } }
        public int TurnsPlayed { get { return _turnsPlayed; } }

        public int TurnLimit { get { return _turnLimit; } }

        public List<Card> DrawPileCards { get => _drawPileCards; }
        public List<Card> PlayPileCards { get => _playPileCards; }
        public List<Card> BurnPileCards { get => _burnPileCards; }

        public BasicBoardParams(List<BasicBoardPlayerParams> playersParams = null, List<int> drawPileValues = null,
            List<int> playPileValues = null, List<int> burnPileValues = null, 
            BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
            bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
            bool hasBurnedThisTurn = false, int turnsPlayed = 0, CardSuit suit = null)
        {
            _playersParams = new List<BasicBoardPlayerParams>();
            _drawPileCards = new List<Card>();
            _playPileCards = new List<Card>();
            _burnPileCards = new List<Card>();

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (playersParams != null) { _playersParams.AddRange(playersParams); }
            if (drawPileValues != null) { _drawPileCards.AddRange(CardsFromValues(drawPileValues, defaultSuit)); }
            if (playPileValues != null) { _playPileCards.AddRange(CardsFromValues(playPileValues, defaultSuit)); }
            if (burnPileValues != null) { _burnPileCards.AddRange(CardsFromValues(burnPileValues, defaultSuit)); }

            _turnOrder = turnOrder;
            _playOrder = playOrder;
            _handsAreFlipped = handsAreFlipped;
            _effectMultiplier = Mathf.Max(effectMultiplier, 1);
            _whoStarts = whoStarts;
            _hasBurnedThisTurn = hasBurnedThisTurn;
            _turnsPlayed = turnsPlayed;
        }

        public BasicBoardParams(List<List<List<int>>> playersParams, List<int> drawPileValues = null,
            List<int> playPileValues = null, List<int> burnPileValues = null,
            BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
            bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
            bool hasBurnedThisTurn = false, int turnsPlayed = 0, CardSuit suit = null)
        {
            _playersParams = new List<BasicBoardPlayerParams>();
            _drawPileCards = new List<Card>();
            _playPileCards = new List<Card>();
            _burnPileCards = new List<Card>();

            List<BasicBoardPlayerParams> playerParams = new();

            foreach (List<List<int>> playerCardValues in playersParams)
            {
                playerParams.Add(new BasicBoardPlayerParams(playerCardValues, isPlayableCharacter: false));
            }

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (playersParams != null) { _playersParams.AddRange(playerParams); }
            if (drawPileValues != null) { _drawPileCards.AddRange(CardsFromValues(drawPileValues, defaultSuit)); }
            if (playPileValues != null) { _playPileCards.AddRange(CardsFromValues(playPileValues, defaultSuit)); }
            if (burnPileValues != null) { _burnPileCards.AddRange(CardsFromValues(burnPileValues, defaultSuit)); }

            _turnOrder = turnOrder;
            _playOrder = playOrder;
            _handsAreFlipped = handsAreFlipped;
            _effectMultiplier = Mathf.Max(effectMultiplier, 1);
            _whoStarts = whoStarts;
            _hasBurnedThisTurn = hasBurnedThisTurn;
            _turnsPlayed = turnsPlayed;
        }

        public BasicBoardParams(List<BasicBoardPlayerParams> playersParams, List<Card> drawPileCards, 
            List<Card> playPileCards, List<Card> burnPileCards, BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, 
            BoardPlayOrder playOrder = BoardPlayOrder.UP, bool handsAreFlipped = false, int effectMultiplier = 1, 
            int whoStarts = 0, bool hasBurnedThisTurn = false, int turnsPlayed = 0)
        {
            _playersParams = new List<BasicBoardPlayerParams>();
            _drawPileCards = new List<Card>();
            _playPileCards = new List<Card>();
            _burnPileCards = new List<Card>();

            if (playersParams != null) { _playersParams.AddRange(playersParams); }
            if (drawPileCards != null) { _drawPileCards.AddRange(drawPileCards); }
            if (playPileCards != null) { _playPileCards.AddRange(playPileCards); }
            if (burnPileCards != null) { _burnPileCards.AddRange(burnPileCards); }

            _turnOrder = turnOrder;
            _playOrder = playOrder;
            _handsAreFlipped = handsAreFlipped;
            _effectMultiplier = Mathf.Max(effectMultiplier, 1);
            _whoStarts = whoStarts;
            _hasBurnedThisTurn = hasBurnedThisTurn;
            _turnsPlayed = turnsPlayed;
        }

        List<Card> CardsFromValues(List<int> values, CardSuit suit)
        {
            List<Card> cards = new ();

            foreach (int value in values)
            {
                cards.Add(new Card((CardValue)value, suit));
            }
            return cards;
        }
    }
}

