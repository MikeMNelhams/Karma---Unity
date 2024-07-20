using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardHandPhysicsInfo
{
    public float startAngle;
    public float endAngle;
    public float distanceFromHolder;
    public float yOffset;

    public CardHandPhysicsInfo(float startAngle = -25.0f, float endAngle = 25.0f, float distanceFromHolder = 0.75f, float yOffset = -0.25f)
    {
        this.startAngle = startAngle;
        this.endAngle = endAngle;
        this.distanceFromHolder = distanceFromHolder;
        this.yOffset = yOffset;
    }

    public static CardHandPhysicsInfo Default { get => new(); }
}
