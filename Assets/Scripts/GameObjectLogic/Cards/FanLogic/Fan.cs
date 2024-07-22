using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers 
{
    public abstract class Fan
    {
        
        public abstract void TransformCardsIntoFan(IList<CardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null);
        public void FlipFan(IList<CardObject> cards)
        {
            foreach (CardObject cardObject in cards)
            {
                cardObject.transform.Rotate(new Vector3(0, 180, 0));
            }
        }
    }
}