using KarmaLogic.Cards;
using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CardSuit))]
public class CardSuitDrawer : PropertyDrawer
{
    int _selected = 0;
    bool _selectedChanged = false;
    protected string[] _options = Enum.GetNames(typeof(CardSuitType));

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int selectedBefore = _selected;

        EditorGUI.BeginChangeCheck();

        this._selected = EditorGUI.Popup(position, "Suit", property.FindPropertyRelative("_suit").intValue, _options);

        if (EditorGUI.EndChangeCheck())
        {
            _selectedChanged = selectedBefore != _selected;
            if (_selectedChanged)
            {
                property.FindPropertyRelative("_suit").intValue = _selected; 
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif