using UnityEditor;
using UnityEngine;
using System;

namespace KarmaLogic.BasicBoard
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BasicBoardCharacterSelector))]
    public class BasicBoardCharacterSelectorDrawer : PropertyDrawer
    {
        protected string[] _characterTypeOptions = Enum.GetNames(typeof(CharacterType));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CharacterType selectedCharacterTypeBefore = (CharacterType) property.FindPropertyRelative("_selectedType").intValue;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            
            Rect popupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            CharacterType _selectedCharacterType = (CharacterType) EditorGUI.Popup(popupRect, "Character Type:", 
                property.FindPropertyRelative("_selectedType").intValue, _characterTypeOptions);

            position.y += EditorGUIUtility.singleLineHeight;

            DrawSelectedCharacterTypeGUI(property, position);

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedCharacterTypeBefore != _selectedCharacterType) 
                { 
                    property.FindPropertyRelative("_selectedType").intValue = (int)_selectedCharacterType; 
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float maxHeight = 3.0f * EditorGUIUtility.singleLineHeight;

            maxHeight = Mathf.Max(maxHeight, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_botParams"), includeChildren: true));
            maxHeight = Mathf.Max(maxHeight, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_playerParams"), includeChildren: true));

            return maxHeight + 1.0f * EditorGUIUtility.singleLineHeight;
        }

        void DrawSelectedCharacterTypeGUI(SerializedProperty selector, Rect position)
        {
            SerializedProperty characterParams = selector.FindPropertyRelative("_selectedType").intValue switch
            {
                (int) CharacterType.Bot => selector.FindPropertyRelative("_botParams"),
                (int) CharacterType.Player => selector.FindPropertyRelative("_playerParams"),
                _ => throw new UnsupportedCharacterTypeException(),
            };

            float propertyHeight = EditorGUI.GetPropertyHeight(characterParams, includeChildren: true);
            position.height = propertyHeight;
            EditorGUI.PropertyField(position, characterParams, includeChildren: true);
        }
    }
#endif
}
