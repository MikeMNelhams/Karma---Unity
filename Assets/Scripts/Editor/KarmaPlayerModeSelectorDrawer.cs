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
    int _selected = 0;
    bool _selectedChanged = false;
    protected string[] _playerModeOptions = Enum.GetNames(typeof(PlayerMode));

    bool _useBoardPresets = true;
    bool _useBoardPresetsChanged = false;

    int _presetSelected = 0;
    protected bool _presetSelectedChanged = false;
    protected string[] _presetOptionsSingleplayer = new string[8] { "TestStartQueenCombo", "TestStartJokerCombo", "TestStartVoting", "TestStartVoting2", "TestScenarioFullHand", "TestLeftwardsHandRotate", "TestRandomStart", "RandomStart4"};

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int selectedBefore = _selected;
        int presetSelectedBefore = _presetSelected;
        bool useBoardPresetBefore = _useBoardPresets;

        EditorGUI.BeginChangeCheck();

        // TODO ask for: which player starts INT FIELD 0 <= x <= Player count.

        this._selected = EditorGUILayout.Popup("Player Mode", property.FindPropertyRelative("_mode").intValue, _playerModeOptions);
        this._useBoardPresets = EditorGUILayout.Toggle("Use board presets?", property.FindPropertyRelative("_useBasicBoardPreset").boolValue);

        if (_useBoardPresets)
        {
            this._presetSelected = EditorGUILayout.Popup("Board Preset", property.FindPropertyRelative("_basicBoardPresetSelected").intValue, _presetOptionsSingleplayer);
            property.FindPropertyRelative("_basicBoardPresetSelected").intValue = _presetSelected;
        }
        else
        {
            SetSingleplayerBoardCustom(property);
        }

        if (EditorGUI.EndChangeCheck())
        {
            _selectedChanged = selectedBefore != _selected;
            if (_selectedChanged) 
            { 
                property.FindPropertyRelative("_mode").intValue = _selected; 
            }

            _useBoardPresetsChanged = useBoardPresetBefore != _useBoardPresets;

            if (_useBoardPresetsChanged)
            {
                property.FindPropertyRelative("_useBasicBoardPreset").boolValue = _useBoardPresets;
            }

            _presetSelectedChanged = presetSelectedBefore != _presetSelected;
        }

        bool isSinglePlayer = (PlayerMode)_selected == PlayerMode.Singleplayer;

        if (!isSinglePlayer) { EditorGUI.EndProperty();  return; }

        EditorGUI.EndProperty();
    }

    void SetSingleplayerBoardCustom(SerializedProperty selector)
    {
        SerializedProperty basicBoardParams = selector.FindPropertyRelative("_basicBoardParams");
        EditorGUILayout.PropertyField(basicBoardParams);
    }
}
#endif
