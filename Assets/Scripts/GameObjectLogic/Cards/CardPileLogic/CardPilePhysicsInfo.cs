using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardPilePhysicsInfo 
{
    public float _maxZRotation;

    public float _maxXOffsetPercent;
    public float _maxZOffsetPercent;

    public CardPilePhysicsInfo(float maxZRotation=80.0f, float maxXOffsetPercent=45.0f, float maxZOffsetPercent=20.0f)
    {
        _maxZRotation = maxZRotation;
        _maxXOffsetPercent = maxXOffsetPercent;
        _maxZOffsetPercent = maxZOffsetPercent;
    }

    public static CardPilePhysicsInfo Messy { get => new(); }

    public static CardPilePhysicsInfo Tidy { get => new(maxZRotation: 10.0f, maxXOffsetPercent: 10.0f, maxZOffsetPercent: 5.0f); }

    public static CardPilePhysicsInfo PerfectlyVertical { get => new(maxZRotation: 0.0f, maxXOffsetPercent: 0.0f, maxZOffsetPercent: 0.0f); }

    public override string ToString()
    {
        return "CardPilePhysicsInfo(" + _maxZRotation + ", " + _maxXOffsetPercent + ", " + _maxZOffsetPercent + ")";
    }
}
