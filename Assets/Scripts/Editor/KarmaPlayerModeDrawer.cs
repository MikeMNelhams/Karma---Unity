using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using KarmaPlayerMode;
using System;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(KarmaPlayerModeSelector))]
public class KarmaPlayerModeDrawer : PropertyDrawer
{
    int _selected = 0;
    readonly string[] _options = Enum.GetNames(typeof(PlayerMode));

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();

        // TODO ask for: which player starts INT FIELD 0 <= x <= Player count.

        this._selected = EditorGUILayout.Popup("Player Mode", _selected, _options);

        if (EditorGUI.EndChangeCheck())
        {
            property.FindPropertyRelative("_mode").intValue = _selected;
            if ((PlayerMode)_selected == PlayerMode.Singleplayer)
            {
                // TODO Update the property drawer for singleplayer
            }
            if ((PlayerMode)_selected == PlayerMode.Multiplayer)
            {
                // TODO Update the property drawer for multiplayer
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
