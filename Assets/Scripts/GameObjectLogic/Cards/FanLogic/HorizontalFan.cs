using System;
using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers
{
    public class HorizontalFan : Fan
    {
        readonly float _antiClippingAmount = -3.0f;
        readonly float _maxHandAngle = 90f;
        readonly Transform _holder;

        public HorizontalFan(Transform holder, float maxHandAngle = 90f, float antiClippingAmount = -3)
        {
            _holder = holder;
            _maxHandAngle = maxHandAngle;
            _antiClippingAmount = antiClippingAmount;
        }

        Tuple<Vector3, Quaternion> RelativeCardTransformInFan(int cardIndex, int numberOfCards, FanPhysicsInfo fanPhysicsInfo)
        {
            Vector3 cardPosition;
            Vector3 lookVector;
            Quaternion cardRotation;

            if (numberOfCards == 1) 
            {
                cardPosition = _holder.TransformPoint(new (0, fanPhysicsInfo.yOffset, fanPhysicsInfo.distanceFromHolder));
                lookVector = _holder.position - cardPosition;
                cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
                return new Tuple<Vector3, Quaternion> (cardPosition, cardRotation); 
            }
            float angle = fanPhysicsInfo.startAngle + (cardIndex * fanPhysicsInfo.totalAngle) / (numberOfCards - 1);

            if (angle > _maxHandAngle) { throw new ArithmeticException("Angle for cards in hand: " + angle + " should not exceed " + _maxHandAngle); }

            double angleRad = angle * (Math.PI / 180.0f);
            float x = (float)(fanPhysicsInfo.distanceFromHolder * Math.Sin(angleRad));
            float z = (float)(fanPhysicsInfo.distanceFromHolder * Math.Cos(angleRad));
            cardPosition = _holder.TransformPoint(new(x, fanPhysicsInfo.yOffset, z));
            
            lookVector = _holder.position - cardPosition;
            cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
            cardRotation *= Quaternion.Euler(0, _antiClippingAmount, 0);  // Slight Y anticlockwise rotation to prevent clipping.
            return new Tuple<Vector3, Quaternion> (cardPosition, cardRotation);
        }

        public override void TransformCardsIntoFan(IList<SelectableCardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null )
        {
            if (cards.Count == 0) { return; }

            if (fanPhysicsInfo != null) { fanPhysicsInfo = FanPhysicsInfo.HorizontalFan; }

            SelectableCardObject cardObject = cards[0];
            Tuple<Vector3, Quaternion> cardPositionAndRotation = RelativeCardTransformInFan(0, cards.Count, fanPhysicsInfo);

            Vector3 cardPosition = cardPositionAndRotation.Item1;
            Quaternion cardRotation = cardPositionAndRotation.Item2;

            if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }
            cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);

            if (cards.Count == 1) { return; }

            for (int i = 1; i < cards.Count; i++)
            {
                cardObject = cards[i];
                cardPositionAndRotation = RelativeCardTransformInFan(i, cards.Count, fanPhysicsInfo);

                cardPosition = cardPositionAndRotation.Item1;
                cardRotation = cardPositionAndRotation.Item2;

                if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }
                cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
            }
        }
    }
}
