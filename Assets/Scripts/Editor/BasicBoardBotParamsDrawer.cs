using UnityEditor;
using UnityEngine;
using System;

namespace KarmaLogic.BasicBoard
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BasicBoardBotParams))]
    public class BasicBoardBotParamsDrawer : BasicBoardCharacterParamsDrawer
    {
        float parentHeight;
        float popupHeight;
        protected string[] _botOptions = Enum.GetNames(typeof(BotType));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            int selectedBotTypeBefore = property.FindPropertyRelative("_selectedBotType").intValue;

            EditorGUI.BeginProperty(position, label, property);

            position.y += parentHeight;
            position.height = popupHeight;
            int selectedBotType = EditorGUI.Popup(position, selectedBotTypeBefore, _botOptions);

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedBotTypeBefore != selectedBotType) { property.FindPropertyRelative("_selectedBotType").intValue = selectedBotType; }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            parentHeight = base.GetPropertyHeight(property, label);
            popupHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_selectedBotType"));

            return parentHeight + popupHeight + EditorGUIUtility.singleLineHeight;
        }
    }
#endif
}
