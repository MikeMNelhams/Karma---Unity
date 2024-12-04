using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Board;
using KarmaLogic.Cards;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public class BasicBoardParams
    {
        [SerializeField] List<BasicBoardCharacterSelector> _characterSelectors;

        [SerializeField] List<Card> _drawPileCards;
        [SerializeField] List<Card> _playPileCards;
        [SerializeField] List<Card> _burnPileCards;

        [SerializeField] BoardTurnOrder _turnOrder = BoardTurnOrder.RIGHT;
        [SerializeField] BoardPlayOrder _playOrder = BoardPlayOrder.UP;
        [SerializeField] bool _handsAreFlipped = false;
        [SerializeField] int _effectMultiplier = 1;
        [SerializeField] int _whoStarts = 0;
        [SerializeField] bool _hasBurnedThisTurn = false;
        [SerializeField] int _turnsPlayed = 0;
        [SerializeField] int _turnLimit = 100;

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

        public List<BasicBoardCharacterParams> CharactersParams
        {
            get
            {
                List<BasicBoardCharacterParams> characterParams = new();
                foreach (BasicBoardCharacterSelector characterSelector in _characterSelectors)
                {
                    characterParams.Add(characterSelector.ToCharacterParams());
                }
                return characterParams;
            }
        }

        public BasicBoardParams(List<BasicBoardCharacterParams> playersParams = null, List<int> drawPileValues = null,
            List<int> playPileValues = null, List<int> burnPileValues = null, 
            BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
            bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
            bool hasBurnedThisTurn = false, int turnsPlayed = 0, CardSuit suit = null)
        {
            _characterSelectors = new List<BasicBoardCharacterSelector>();

            _drawPileCards = new List<Card>();
            _playPileCards = new List<Card>();
            _burnPileCards = new List<Card>();

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (playersParams != null) { SetCharactersParams(playersParams); }
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

        public BasicBoardParams(List<List<List<int>>> playersParams, List<CharacterType> characterTypes, 
            List<int> drawPileValues = null, List<int> playPileValues = null, List<int> burnPileValues = null,
            BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
            bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
            bool hasBurnedThisTurn = false, int turnsPlayed = 0, CardSuit suit = null)
        {
            _characterSelectors = new List<BasicBoardCharacterSelector>();

            _drawPileCards = new List<Card>();
            _playPileCards = new List<Card>();
            _burnPileCards = new List<Card>();

            List<BasicBoardCharacterParams> playerParams = new();

            foreach (List<List<int>> playerCardValues in playersParams)
            {
                playerParams.Add(new BasicBoardBotParams(playerCardValues));
            }

            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            if (playersParams != null) { SetCharactersParams(playersParams, characterTypes, suit); }
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

            UnityEngine.Debug.LogWarning("Number of bots in playerParams: " + playersParams.Count);
            UnityEngine.Debug.LogWarning("Number of selectors in BasicBoardParams.CharacterSelectors: " + _characterSelectors.Count);
        }

        public BasicBoardParams(List<BasicBoardCharacterParams> playersParams, List<Card> drawPileCards, 
            List<Card> playPileCards, List<Card> burnPileCards, BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, 
            BoardPlayOrder playOrder = BoardPlayOrder.UP, bool handsAreFlipped = false, int effectMultiplier = 1, 
            int whoStarts = 0, bool hasBurnedThisTurn = false, int turnsPlayed = 0)
        {
            _characterSelectors = new List<BasicBoardCharacterSelector>();

            _drawPileCards = new List<Card>();
            _playPileCards = new List<Card>();
            _burnPileCards = new List<Card>();

            if (playersParams != null) { SetCharactersParams(playersParams); }
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

        public static BasicBoardParams AllBots(List<List<List<int>>> playersParams, 
            List<int> drawPileValues = null, List<int> playPileValues = null, List<int> burnPileValues = null,
            BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
            bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
            bool hasBurnedThisTurn = false, int turnsPlayed = 0, CardSuit suit = null)
        {
            List<CharacterType> characterTypes = new();
            for (int i = 0; i < playersParams.Count; i++)
            {
                characterTypes.Add(CharacterType.Bot);
            }
            return new BasicBoardParams(playersParams, characterTypes, drawPileValues, playPileValues, burnPileValues, turnOrder, 
                playOrder, handsAreFlipped, effectMultiplier, whoStarts, hasBurnedThisTurn, turnsPlayed, suit);
        }

        void SetCharactersParams(List<BasicBoardCharacterParams> playersParams)
        {
            for (int i = 0; i < playersParams.Count; i++)
            {
                _characterSelectors.Add(new BasicBoardCharacterSelector());
            }

            for (int i = 0; i < playersParams.Count; i++)
            {
                _characterSelectors[i].SetParams(playersParams[i]);
            }
        }

        void SetCharactersParams(List<List<List<int>>> playersParams, List<CharacterType> characterTypes, CardSuit suit)
        {

            for (int i = 0; i < playersParams.Count; i++)
            {
                _characterSelectors.Add(new BasicBoardCharacterSelector());
            }

            for (int i = 0; i < playersParams.Count; i++)
            {
                _characterSelectors[i].SetParams(playersParams[i], characterTypes[i], suit);
            }
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

