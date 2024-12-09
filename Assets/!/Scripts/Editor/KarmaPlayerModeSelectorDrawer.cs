using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using KarmaPlayerMode;
using System;
using KarmaLogic.BasicBoard;
using UnityEngine.UIElements;
using KarmaPlayerMode.Singleplayer;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(KarmaPlayerModeSelector))]
public class KarmaPlayerModeSelectorDrawer : PropertyDrawer
{
    int _selectedMode = 0;
    protected string[] _modeOptions = Enum.GetNames(typeof(PlayerMode));

    int _selectedSubMode = 0;
    protected string[][] _subModeOptions = 
    { 
        new string[2] { "Solo", "Many" }, 
        new string[1] { "Multiplayer" } 
    };

    bool _useBoardPresets = true;

    int _presetSelected = 0;

    protected string[][][] _presetOptions = 
    {
        new string[2][] 
        { 
            new string[24] {
                "TestStartQueenCombo", "TestStartJokerCombo", "TestStartVoting",
                "TestStartVoting2", "TestScenarioFullHand", "TestLeftwardsHandRotate",
                "TestGameWonNoVote", "TestPotentialWinnerIsSkippedInUnwonGame", "TestMultipleSeparateCardGiveaways",
                "TestQueenComboLastCardToWin", "TestQueenComboLastCardWithJokerInPlay", "TestValidJokerAsLastCardToWin",
                "TestGettingJokered", "TestJokerAsAceLastCardToWin", "TestAllPlayersNoActionsGameEnds",
                "TestAceNoHandDoesNotCrash", "TestAceAndFive", "TestJackOnNine", 
                "TestJackOnQueen", "TestJackOnAce", "TestJackOnKingOnQueen", 
                "TestMultipleCardGiveaway", "TestRandomStart", "DefaultSingleplayer"},
            new string[1]
            {
                "PlayRandomStart4Playable"
            }
        },
        new string[1][]
        {
            new string[1]
            {
                "Not implemented yet!"
            }
        }
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int selectedModeBefore = _selectedMode;
        int selectedSubModeBefore = _selectedSubMode;
        int presetSelectedBefore = _presetSelected;
        bool useBoardPresetBefore = _useBoardPresets;

        EditorGUI.BeginChangeCheck();

        _selectedMode = EditorGUI.Popup(position, "Player Mode", property.FindPropertyRelative("_mode").intValue, _modeOptions);
        position.y += EditorGUIUtility.singleLineHeight;

        _selectedSubMode = EditorGUILayout.Popup("Player Sub Mode", property.FindPropertyRelative("_subMode").intValue, _subModeOptions[_selectedMode]);

        string[][] _subPresetOptions = _presetOptions[_selectedMode];
        _selectedSubMode = Mathf.Min(_selectedSubMode, _subPresetOptions.Length - 1);

        _useBoardPresets = EditorGUILayout.Toggle("Use board presets?", property.FindPropertyRelative("_useBasicBoardPreset").boolValue);

        if (_useBoardPresets)
        {
            _presetSelected = EditorGUILayout.Popup("Board Preset", property.FindPropertyRelative("_basicBoardPresetSelected").intValue,
                _subPresetOptions[_selectedSubMode]);
            _presetSelected = Mathf.Min(_presetSelected, _subPresetOptions[_selectedSubMode].Length - 1);
        }
        else { SetSingleplayerBoardCustom(property); }

        if (EditorGUI.EndChangeCheck())
        {
            if (selectedModeBefore != _selectedMode) { property.FindPropertyRelative("_mode").intValue = _selectedMode; }
            if (selectedSubModeBefore != _selectedSubMode) { property.FindPropertyRelative("_subMode").intValue = _selectedSubMode; }
            if (useBoardPresetBefore != _useBoardPresets) { property.FindPropertyRelative("_useBasicBoardPreset").boolValue = _useBoardPresets; }
            if (presetSelectedBefore != _presetSelected) { property.FindPropertyRelative("_basicBoardPresetSelected").intValue = _presetSelected; }
        }

        EditorGUI.EndProperty();
    }

    void SetSingleplayerBoardCustom(SerializedProperty selector)
    {
        SerializedProperty basicBoardParams = selector.FindPropertyRelative("_basicBoardParams");
        EditorGUILayout.PropertyField(basicBoardParams);
    }
}
#endif
