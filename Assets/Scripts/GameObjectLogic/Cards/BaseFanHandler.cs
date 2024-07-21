using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers 
{
    public abstract class BaseFanHandler : MonoBehaviour
    {
        [SerializeField] protected Transform _holder;
        [SerializeField] protected FanPhysicsInfo _fanPhysicsInfo;

        public abstract Vector3 RelativeCardPositionInFan(float distanceFromCentre, float angle, float yOffset);
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