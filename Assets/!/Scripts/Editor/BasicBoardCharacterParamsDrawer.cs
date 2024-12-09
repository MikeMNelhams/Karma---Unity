using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KarmaLogic.BasicBoard
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BasicBoardCharacterParams), true)]
    public class BasicBoardCharacterParamsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property.FindPropertyRelative("_handCards"));

            position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_handCards"), includeChildren: true) + 1.0f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_karmaUpCards"));

            position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_karmaUpCards"), includeChildren: true) + 1.0f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_karmaDownCards"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propertyHeight = EditorGUI.GetPropertyHeight(property, includeChildren: true);
            return propertyHeight;
        }
    }
#endif
}