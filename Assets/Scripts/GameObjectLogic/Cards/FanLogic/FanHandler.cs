using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FanHandlers;

public class FanHandler : MonoBehaviour
{
    [SerializeField] FanMode _fanMode = FanMode.HORIZONTAL;
    Fan _fan;

    void UpdateFanMode()
    {
        _fan = _fanMode switch
        {
            FanMode.SEMI_CIRCULAR => new SemiCircularFan(transform),
            _ => new HorizontalFan(transform),
        };
    }

    public void TransformCardsIntoFan(IList<CardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null)
    {
        fanPhysicsInfo ??= FanPhysicsInfo.Default;
        UpdateFanMode();
        _fan.TransformCardsIntoFan(cards, isFlipped, fanPhysicsInfo);
    }

    public void FlipFan(IList<CardObject> cards)
    {
        UpdateFanMode(); 
        _fan.FlipFan(cards);
    }
}

public enum FanMode : byte
{
    HORIZONTAL = 0,
    SEMI_CIRCULAR = 1
}
