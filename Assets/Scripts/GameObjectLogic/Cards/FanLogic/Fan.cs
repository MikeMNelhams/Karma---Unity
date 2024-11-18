using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers 
{
    public abstract class Fan
    {
        
        public abstract void TransformCardsIntoFan(IList<SelectableCardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null);
        public void FlipFan(IList<SelectableCardObject> cards)
        {
            foreach (SelectableCardObject cardObject in cards)
            {
                cardObject.transform.Rotate(new Vector3(0, 180, 0));
            }
        }
    }
}