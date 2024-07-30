using System;
using System.Collections.Generic;
using UnityEngine;

namespace FanHandlers
{
    public class VerticalSemiCircularFan : Fan
    {

        readonly float _antiClippingAmount;
        readonly float _radiusInner = 0.25f;
        readonly int _maxCardsPerRow;

        readonly float _halfCardHeight;
        readonly float _cardDepth;

        readonly Transform _holder;

        public VerticalSemiCircularFan(Transform holder, float antiClippingRotation = -3, int maxCardsPerRow = 7)
        {
            if (maxCardsPerRow <= 0) { throw new ArithmeticException("The max number of cards per row must be greater or equal to 1!"); }

            _holder = holder;
            _antiClippingAmount = antiClippingRotation;
            _maxCardsPerRow = maxCardsPerRow;
            _halfCardHeight = KarmaGameManager.Instance.CardTransform.lossyScale.y;
            _cardDepth = KarmaGameManager.Instance.CardTransform.lossyScale.z;
        }

        Tuple<Vector3, Quaternion> RelativeCardPositionAndRotationInFan(int cardIndex, int numberOfCards, FanPhysicsInfo fanPhysicsInfo)
        {
            if (numberOfCards == 0) { throw new ArgumentException("The number of cards must not be zero!"); }
            if (numberOfCards == 1)
            {
                Vector3 directPosition = new(0, fanPhysicsInfo.yOffset, fanPhysicsInfo.distanceFromHolder);
                return new Tuple<Vector3, Quaternion>(directPosition, Quaternion.Euler(new Vector3(0, 180, 0)));
            }

            int rowNumber = 0;
            int rowSize = _maxCardsPerRow;
            int previousRowSize = 0;
            while (cardIndex + 1 > rowSize)
            {
                previousRowSize = rowSize;
                rowNumber++;
                rowSize = RowMaxSize(rowNumber);
            }

            int indexInRow = cardIndex - previousRowSize;

            float radius = _radiusInner + _halfCardHeight * rowNumber;
            //UnityEngine.Debug.Log("Number of cards: " + numberOfCards + " index horizontal: " + cardIndex + " row number: " + rowNumber + " indexInRow: " + indexInRow + " previous row size: " + previousRowSize + " radius: " + radius);
            float degreesToRadians = (float)(Math.PI / 180f);
            float theta = fanPhysicsInfo.endAngle - (indexInRow * fanPhysicsInfo.totalAngle) / (_maxCardsPerRow - 1 + rowNumber);
            float x = (float)(radius * Math.Cos(theta * degreesToRadians + Math.PI / 2));
            float y = (float)((radius * Math.Sin(theta * degreesToRadians + Math.PI / 2)) - radius / 2);
            float z = fanPhysicsInfo.distanceFromHolder + rowNumber * _cardDepth;
            Vector3 cardPosition = _holder.transform.TransformPoint(new(x, y + fanPhysicsInfo.yOffset, z));

            Quaternion cardRotation = Quaternion.Euler(new Vector3(0, 180 + _antiClippingAmount, -theta));
            return new Tuple<Vector3, Quaternion>(cardPosition, cardRotation);
        }

        int RowMaxSize(int rowNumber)
        {
            return ((_maxCardsPerRow * 2 + rowNumber) * (rowNumber + 1)) / 2;
        }

        public override void TransformCardsIntoFan(IList<CardObject> cards, bool isFlipped, FanPhysicsInfo fanPhysicsInfo = null)
        {
            if (cards.Count == 0) { return; }
            if (fanPhysicsInfo != null) { fanPhysicsInfo = FanPhysicsInfo.VerticalSemiCircularFan; }

            CardObject cardObject = cards[0];

            Tuple<Vector3, Quaternion> cardPositionAndRotation = RelativeCardPositionAndRotationInFan(0, cards.Count, fanPhysicsInfo);
            Vector3 cardPosition = cardPositionAndRotation.Item1;
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
