using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FanPhysicsInfo
{
    public float startAngle;
    public float endAngle;
    public float distanceFromHolder;
    public float yOffset;

    public FanPhysicsInfo(float startAngle = -25.0f, float endAngle = 25.0f, float distanceFromHolder = 0.75f, float yOffset = -0.25f)
    {
        this.startAngle = startAngle;
        this.endAngle = endAngle;
        this.distanceFromHolder = distanceFromHolder;
        this.yOffset = yOffset;
    }

    public static FanPhysicsInfo Default { get => new(); }
}
