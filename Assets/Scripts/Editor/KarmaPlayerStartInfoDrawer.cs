using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(KarmaPlayerStartInfo))]
public class KarmaPlayerStartInfoDrawer : PropertyDrawer
{
    protected float lineHeight = 18f;
    protected int floatCounts = 2;
    protected float yPadding = 5f;

    protected GUIContent checkBoxText = new ("Is Playable Character?:");
    protected GUIStyle checkBoxStyle = GUIStyle.none;
    
    protected GUIContent startPositionText = new ("Start Position:");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indentLevel = EditorGUI.indentLevel;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUI.indentLevel = 0;

        Rect isPlayableCharacterCheckbox = new(position.x, position.y, position.width, lineHeight);
        Rect startPosition = new(position.x, position.y + lineHeight + yPadding, position.width, lineHeight);

        EditorGUIUtility.labelWidth -= 20;

        property.FindPropertyRelative("isPlayableCharacter").boolValue = EditorGUI.ToggleLeft(isPlayableCharacterCheckbox, "Is Playable Character?:", property.FindPropertyRelative("isPlayableCharacter").boolValue);
        property.FindPropertyRelative("startPosition").vector3Value = EditorGUI.Vector3Field(startPosition, startPositionText, property.FindPropertyRelative("startPosition").vector3Value);
        
        EditorGUI.indentLevel = indentLevel;
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //set the height of the drawer by the field size and padding
        return ((lineHeight + yPadding) * floatCounts);
    }
}
#endif