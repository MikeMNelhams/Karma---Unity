using System;
using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers
{
    public class HorizontalFan : Fan
    {
        [SerializeField] Quaternion _antiClippingRotation = Quaternion.Euler(new Vector3(0, -3, 0));
        readonly float _maxHandAngle = 90f;
        readonly Transform _holder;

        public HorizontalFan(Transform holder, float maxHandAngle = 90f, float antiClippingRotation = -3)
        {
            _holder = holder;
            _maxHandAngle = maxHandAngle;
            _antiClippingRotation = Quaternion.Euler(new Vector3(0, antiClippingRotation, 0));
        }

        Vector3 RelativeCardPositionInFan(int cardIndex, int numberOfCards, FanPhysicsInfo fanPhysicsInfo)
        {
            
            if (numberOfCards == 1) { return new Vector3(0, fanPhysicsInfo.yOffset, fanPhysicsInfo.distanceFromHolder); }
            float angle = fanPhysicsInfo.startAngle + (cardIndex * fanPhysicsInfo.totalAngle) / (numberOfCards - 1);

            if (angle > _maxHandAngle) { throw new ArithmeticException("Angle for cards in hand: " + angle + " should not exceed " + _maxHandAngle); }

            double angleRad = angle * (Math.PI / 180.0f);
            float x = (float)(fanPhysicsInfo.distanceFromHolder * Math.Sin(angleRad));
            return new Vector3(x, fanPhysicsInfo.yOffset, fanPhysicsInfo.distanceFromHolder);
        }

        public override void TransformCardsIntoFan(IList<CardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null )
        {
            if (cards.Count == 0) { return; }

            if (fanPhysicsInfo != null) { fanPhysicsInfo = FanPhysicsInfo.Default; }

            Vector3 holderPosition = _holder.position;

            CardObject cardObject = cards[0];
            Vector3 cardPosition = _holder.TransformPoint(RelativeCardPositionInFan(0, cards.Count, fanPhysicsInfo));
            Vector3 lookVector = holderPosition - cardPosition;

            Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
            if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

            cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);

            if (cards.Count == 1) { return; }

            for (int i = 1; i < cards.Count; i++)
            {
                cardObject = cards[i];

                cardPosition = _holder.TransformPoint(RelativeCardPositionInFan(i, cards.Count, fanPhysicsInfo));
                lookVector = holderPosition - cardPosition;

                cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
                if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

                cardRotation *= _antiClippingRotation;  // Slight Y anticlockwise rotation to prevent clipping.
                cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
            }
        }
    }
}
