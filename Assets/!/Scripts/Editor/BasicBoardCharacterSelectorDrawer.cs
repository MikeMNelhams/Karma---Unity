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

            GUIStyle typeSelectorStyle = new("popup")
            {
                fontStyle = FontStyle.Bold
            };

            CharacterType _selectedCharacterType = (CharacterType) EditorGUI.Popup(popupRect, "Character Type:", 
                property.FindPropertyRelative("_selectedType").intValue, _characterTypeOptions, typeSelectorStyle);

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

            maxHeight = (CharacterType)property.FindPropertyRelative("_selectedType").intValue switch
            {
                CharacterType.Bot => Mathf.Max(maxHeight, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_botParams"), includeChildren: true)),
                CharacterType.Player => Mathf.Max(maxHeight, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_playerParams"), includeChildren: true)),
                _ => throw new UnsupportedCharacterTypeException(),
            };
            return maxHeight;
        }

        void DrawSelectedCharacterTypeGUI(SerializedProperty selector, Rect position)
        {
            SerializedProperty characterParams = (CharacterType) selector.FindPropertyRelative("_selectedType").intValue switch
            {
                CharacterType.Bot => selector.FindPropertyRelative("_botParams"),
                CharacterType.Player => selector.FindPropertyRelative("_playerParams"),
                _ => throw new UnsupportedCharacterTypeException(),
            };

            float propertyHeight = EditorGUI.GetPropertyHeight(characterParams, includeChildren: true);
            position.height = propertyHeight;
            EditorGUI.PropertyField(position, characterParams, includeChildren: true);
        }
    }
#endif
}
