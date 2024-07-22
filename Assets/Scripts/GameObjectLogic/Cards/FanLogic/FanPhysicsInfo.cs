using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FanPhysicsInfo
{
    public float startAngle;
    public float endAngle;
    public float distanceFromHolder;
    public float yOffset;

    public float totalAngle;

    public FanPhysicsInfo(float startAngle = -25.0f, float endAngle = 25.0f, float distanceFromHolder = 0.75f, float yOffset = -0.25f)
    {
        if (startAngle > endAngle) { throw new ArgumentException("The start angle: \'" + startAngle + "\' must be greater or equal to the end angle: \'" + endAngle + "\'"); }

        this.startAngle = startAngle;
        this.endAngle = endAngle;
        this.distanceFromHolder = distanceFromHolder;
        this.yOffset = yOffset;
        totalAngle = endAngle - startAngle;
    }

    public static FanPhysicsInfo Default { get => new(); }
}
