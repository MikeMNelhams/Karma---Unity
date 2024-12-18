using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FanHandlers;

public class FanHandler : MonoBehaviour
{
    [SerializeField] FanMode _fanMode = FanMode.HORIZONTAL;
    Fan _fan;

    readonly Dictionary<FanMode, Fan> _fanMap = new ();

    void UpdateFanMode()
    {
        // Help out the Garbage collector by never making more than 1 of each mode :D
        if (_fanMap.ContainsKey(_fanMode)) { _fan = _fanMap[_fanMode]; return; }

        _fan = _fanMode switch
        {
            FanMode.SEMI_CIRCULAR => new SemiCircularFan(transform),
            FanMode.VERTICAL_SEMI_CIRCULAR => new VerticalSemiCircularFan(transform),
            _ => new HorizontalFan(transform),
        };

        _fanMap[_fanMode] = _fan;
    }

    public void TransformCardsIntoFan(IList<SelectableCardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null)
    {
        fanPhysicsInfo ??= FanPhysicsInfo.Default;
        UpdateFanMode();
        _fan.TransformCardsIntoFan(cards, isFlipped, fanPhysicsInfo);
    }

    public void FlipFan(IList<SelectableCardObject> cards)
    {
        UpdateFanMode(); 
        _fan.FlipFan(cards);
    }
}

public enum FanMode : byte
{
    HORIZONTAL = 0,
    SEMI_CIRCULAR = 1,
    VERTICAL_SEMI_CIRCULAR = 2
}
