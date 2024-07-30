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

    public FanPhysicsInfo(float startAngle = -30.0f, float endAngle = 30.0f, float distanceFromHolder = 0.75f, float yOffset = -0.25f)
    {
        if (startAngle > endAngle) { throw new ArgumentException("The start angle: \'" + startAngle + "\' must be greater or equal to the end angle: \'" + endAngle + "\'"); }

        this.startAngle = startAngle;
        this.endAngle = endAngle;
        this.distanceFromHolder = distanceFromHolder;
        this.yOffset = yOffset;
        totalAngle = endAngle - startAngle;
    }

    public static FanPhysicsInfo Default { get => new(); }

    public static FanPhysicsInfo HorizontalFan
    {
        get
        {
            return new(startAngle: -25.0f, endAngle: 25.0f);
        }
    }

    public static FanPhysicsInfo SemiCircularFan 
    { 
        get
        {
            return new(startAngle: -75.0f, endAngle: 75.0f);
        }
    }

    public static FanPhysicsInfo VerticalSemiCircularFan
    {
        get
        {
            return new(startAngle: -35.0f, endAngle: 35.0f);
        }
    }
}
