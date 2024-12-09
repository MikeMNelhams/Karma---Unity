using KarmaLogic.Cards;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaLogic.BasicBoard
{
    [System.Serializable]
    public enum CharacterType: byte
    {
        Bot,
        Player
    }

    [System.Serializable]
    public class BasicBoardCharacterSelector
    {
        // Serialize ALL of the params, this way the editor remembers changes when toggling between type
        // Plus Unity does supporting polymorphic serialization on non-monobehaviours :|
        [SerializeField] BasicBoardBotParams _botParams = new ();
        [SerializeField] BasicBoardPlayerParams _playerParams = new ();

        [SerializeField] int _selectedType = (int)CharacterType.Bot;

        public BasicBoardCharacterSelector()
        {
            _selectedType = (int) CharacterType.Bot;
            _botParams = new BasicBoardBotParams();
            _playerParams = new BasicBoardPlayerParams();
        }

        public void SetParams(BasicBoardCharacterParams characterParams)
        {
            SetSelectedCharacterParams(characterParams);
            SetSelectedCharacterType(characterParams);
        }

        public void SetParams(List<List<int>> playersParams, CharacterType characterType, CardSuit suit = null)
        {
            CardSuit defaultSuit = suit;
            defaultSuit ??= CardSuit.DebugDefault;

            BasicBoardCharacterParams characterParams = characterType switch
            {
                CharacterType.Bot => new BasicBoardBotParams(playersParams, suit: defaultSuit),
                CharacterType.Player => new BasicBoardPlayerParams(playersParams, suit: defaultSuit),
                _ => throw new UnsupportedCharacterTypeException(),
            };

            SetParams(characterParams);
        }

        public BasicBoardCharacterParams ToCharacterParams()
        {
            return _selectedType switch
            {
                (int) CharacterType.Bot => _botParams,
                (int) CharacterType.Player => _playerParams,
                _ => throw new UnsupportedCharacterTypeException(),
            };
        }

        void SetSelectedCharacterParams(BasicBoardCharacterParams characterParams)
        {
            switch (characterParams)
            {
                case BasicBoardBotParams:
                    _botParams = (BasicBoardBotParams)characterParams; 
                    break;
                case BasicBoardPlayerParams:
                    _playerParams = (BasicBoardPlayerParams)characterParams;
                    break;
                default:
                    throw new UnsupportedCharacterTypeException();
            }
        }

        void SetSelectedCharacterType(BasicBoardCharacterParams characterParams)
        {
            _selectedType = characterParams switch
            {
                BasicBoardBotParams => (int) CharacterType.Bot,
                BasicBoardPlayerParams => (int) CharacterType.Player,
                _ => throw new UnsupportedCharacterTypeException(),
            };
        }
    }
}