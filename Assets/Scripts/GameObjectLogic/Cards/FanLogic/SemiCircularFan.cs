using System;
using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers
{
    public class SemiCircularFan : Fan
    {
        readonly float _antiClippingAmount;
        readonly float _radiusWidth = 0.25f;
        readonly float _radiusHeight = 0.25f;

        readonly Transform _holder;

        public SemiCircularFan(Transform holder, float antiClippingRotation = -3)
        {
            _holder = holder;
            _antiClippingAmount = antiClippingRotation;
        }

        Tuple<Vector3, Quaternion> RelativeCardPositionAndRotationInFan(int cardIndex, int numberOfCards, FanPhysicsInfo fanPhysicsInfo)
        {
            if (numberOfCards == 0) { throw new ArgumentException("The number of cards must not be zero!"); }
            if (numberOfCards == 1) 
            {
                Vector3 directPosition = new (0, fanPhysicsInfo.yOffset, fanPhysicsInfo.distanceFromHolder);
                return new Tuple<Vector3, Quaternion>(directPosition, Quaternion.Euler(new Vector3(0, 180, 0))); 
            }

            float degreesToRadians = (float)(Math.PI / 180f);
            float theta = fanPhysicsInfo.endAngle - (cardIndex * fanPhysicsInfo.totalAngle) / (numberOfCards - 1);
            float x = (float)(_radiusWidth * Math.Cos(theta * degreesToRadians + Math.PI / 2));
            float y = (float)((_radiusHeight * Math.Sin(theta * degreesToRadians + Math.PI / 2)) - _radiusHeight / 2);
            float z = fanPhysicsInfo.distanceFromHolder;
            Vector3 cardPosition = _holder.transform.TransformPoint(new (x, y + fanPhysicsInfo.yOffset, z));

            Quaternion cardRotation = Quaternion.Euler(new Vector3(0, 180 + _antiClippingAmount, -theta));
            return new Tuple<Vector3, Quaternion> (cardPosition, cardRotation);
        }

        public override void TransformCardsIntoFan(IList<CardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null)
        {
            if (cards.Count == 0) { return; }
            if (fanPhysicsInfo != null) { fanPhysicsInfo = FanPhysicsInfo.SemiCircularFan; }

            CardObject cardObject = cards[0];

            Tuple<Vector3, Quaternion> cardPositionAndRotation = RelativeCardPositionAndRotationInFan(0, cards.Count, fanPhysicsInfo);
            Vector3 cardPosition = _holder.transform.TransformPoint(cardPositionAndRotation.Item1);
            Quaternion cardRotation = cardPositionAndRotation.Item2;

            if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }
            cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);

            if (cards.Count == 1) { return; }

            for (int i = 1; i < cards.Count; i++)
            {
                cardObject = cards[i];
                cardPositionAndRotation = RelativeCardPositionAndRotationInFan(i, cards.Count, fanPhysicsInfo);

                cardPosition = cardPositionAndRotation.Item1;
                cardRotation = cardPositionAndRotation.Item2;

                if (isFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }
                cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
            }
        }
    }
}
