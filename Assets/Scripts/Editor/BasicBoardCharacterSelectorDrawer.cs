using UnityEditor;
using UnityEngine;
using System;

namespace KarmaLogic.BasicBoard
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BasicBoardCharacterSelector))]
    public class BasicBoardCharacterSelectorDrawer : PropertyDrawer
    {
        CharacterType _selectedCharacterType = CharacterType.Bot;
        protected string[] _characterTypeOptions = Enum.GetNames(typeof(CharacterType));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CharacterType selectedCharacterTypeBefore = _selectedCharacterType;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            Rect popupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            _selectedCharacterType = (CharacterType) EditorGUI.Popup(popupRect, "Character Type:", 
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
            SerializedProperty characterParams = _selectedCharacterType switch
            {
                CharacterType.Bot => selector.FindPropertyRelative("_botParams"),
                CharacterType.Player => selector.FindPropertyRelative("_playerParams"),
                _ => throw new NotImplementedException("Unsupported player-character type!"),
            };

            float propertyHeight = EditorGUI.GetPropertyHeight(characterParams, includeChildren: true);
            position.height = propertyHeight;
            EditorGUI.PropertyField(position, characterParams, includeChildren: true);
        }
    }
#endif
}
