using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CardHandPhysicsInfo))]
public class CardHandPhysicsInfoDrawer : PropertyDrawer
{
    protected float fieldWidth = 40.0f;
    protected float leftPadding = 15.0f;
    protected float rightPadding = 15.0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.indentLevel = 0;

        float width = position.width;
        float padding = (width - 4 * fieldWidth - leftPadding - rightPadding) / (4 - 1);
        float[] xPositions = new float[4];
        for (int i = 0; i < 4; i++)
        {
            xPositions[i] = position.x + i * fieldWidth + padding * i + leftPadding;
        }

        Rect startAngleRect = new(xPositions[0], position.y, fieldWidth, position.height);
        Rect endAngleRect = new (xPositions[1], position.y, fieldWidth, position.height);
        Rect distanceFromHolderRect = new (xPositions[2], position.y, fieldWidth, position.height);
        Rect yOffsetRect = new (xPositions[3], position.y, fieldWidth, position.height);

        EditorGUI.PropertyField(startAngleRect, property.FindPropertyRelative("startAngle"), GUIContent.none);
        EditorGUI.PropertyField(endAngleRect, property.FindPropertyRelative("endAngle"), GUIContent.none);
        EditorGUI.PropertyField(distanceFromHolderRect, property.FindPropertyRelative("distanceFromHolder"), GUIContent.none);
        EditorGUI.PropertyField(yOffsetRect, property.FindPropertyRelative("yOffset"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
