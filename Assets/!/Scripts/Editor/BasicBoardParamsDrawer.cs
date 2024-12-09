using KarmaLogic.BasicBoard;
using KarmaPlayerMode;
using System;
using UnityEditor;
using UnityEngine;

namespace KarmaLogic.BasicBoard
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BasicBoardParams))]
    public class BasicBoardParamsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("_turnOrder"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_playOrder"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_handsAreFlipped"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_effectMultiplier"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_whoStarts"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_hasBurnedThisTurn"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_turnsPlayed"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_turnLimit"));

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indentLevel + 1;

            EditorGUILayout.PropertyField(property.FindPropertyRelative("_drawPileCards"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_playPileCards"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_burnPileCards"));

            EditorGUILayout.PropertyField(property.FindPropertyRelative("_characterSelectors"));

            EditorGUI.indentLevel = indentLevel;

            EditorGUI.EndProperty();
        }
    }
#endif
}
