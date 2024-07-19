using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(KarmaPlayerStartInfo))]
public class KarmaPlayerStartInfoDrawer : PropertyDrawer
{
    protected float lineHeight = 18f;

    readonly GUIContent checkBoxText = new ("Is Playable Character?:");
    readonly GUIStyle checkBoxStyle = GUIStyle.none;
    
    readonly GUIContent startPositionText = new ("Start Position:");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indentLevel = EditorGUI.indentLevel;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUI.indentLevel = 0;

        // Reserve an extra row for the Vector3
        Rect rt = GUILayoutUtility.GetRect(checkBoxText, checkBoxStyle);  
        rt.height = lineHeight * 2;

        Rect isPlayableCharacterCheckbox = new(position.x, position.y, rt.width, position.height);
        Rect startPosition = new(position.x, position.y + lineHeight, position.width, position.height);

        EditorGUIUtility.labelWidth -= 20;

        property.FindPropertyRelative("isPlayableCharacter").boolValue = EditorGUI.ToggleLeft(isPlayableCharacterCheckbox, "Is Playable Character?:", property.FindPropertyRelative("isPlayableCharacter").boolValue);
        property.FindPropertyRelative("startPosition").vector3Value = EditorGUI.Vector3Field(startPosition, startPositionText, property.FindPropertyRelative("startPosition").vector3Value);

        EditorGUI.indentLevel = indentLevel;
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.EndProperty();
    }
}
#endif