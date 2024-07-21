using System;
using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers
{
    public class HorizontalFanHandler : BaseFanHandler
    {
        [SerializeField] Quaternion _antiClippingRotation = Quaternion.Euler(new Vector3(0, -3, 0));

        public override Vector3 RelativeCardPositionInFan(float distanceFromCentre, float angle, float yOffset)
        {
            if (angle > 90) { throw new ArithmeticException("Angle for cards in hand: " + angle + " should not exceed 90"); }
            if (angle == 0) { return new Vector3(0, yOffset, distanceFromCentre); }
            double angleRad = (double)angle * (Math.PI / 180.0f);
            float x = (float)(distanceFromCentre * Math.Sin(angleRad));
            float z = (float)(distanceFromCentre * Math.Cos(angleRad));
            return new Vector3(x, yOffset, z);
        }

        public override void TransformCardsIntoFan(IList<CardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null )
        {
            if (cards.Count == 0) { return; }

            if (fanPhysicsInfo != null) { _fanPhysicsInfo = fanPhysicsInfo; }

            float startAngle = _fanPhysicsInfo.startAngle;
            float endAngle = _fanPhysicsInfo.endAngle;
            float distanceFromHolder = _fanPhysicsInfo.distanceFromHolder;
            float yOffset = _fanPhysicsInfo.yOffset;

            Vector3 holderPosition = _holder.position;

            if (cards.Count == 1)
            {
                CardObject cardObject = cards[0];
                float middleAngle = (startAngle + endAngle) / 2;
                Vector3 cardPosition = _holder.TransformPoint(RelativeCardPositionInFan(distanceFromHolder, middleAngle, yOffset));
                Vector3 lookVector = holderPosition - cardPosition;

                Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
                if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

                cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
                return;
            }

            float angleStepSize = (endAngle - startAngle) / (cards.Count - 1);

            int j = 0;
            foreach (CardObject cardObject in cards)
            {
                float angle = startAngle + j * angleStepSize;
                Vector3 cardPosition = _holder.TransformPoint(RelativeCardPositionInFan(distanceFromHolder, angle, yOffset));
                Vector3 lookVector = holderPosition - cardPosition;

                Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
                if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

                cardRotation *= _antiClippingRotation;  // Slight Y anticlockwise rotation to prevent clipping.
                cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
                j++;
            }
        }
    }
}
