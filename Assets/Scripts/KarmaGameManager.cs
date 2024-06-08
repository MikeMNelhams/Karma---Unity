using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Karma.Game;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Players;
using Karma.Cards;
using System;

public class KarmaGameManager : MonoBehaviour 
{
    Game game;
    public GameObject cardPrefab;
    public List<GameObject> cardHolders;

    // Start is called before the first frame update
    void Start()
    {
        BasicBoard startBoard = BoardFactory.RandomStart(4, 3);
        CreateCardsFromBoard(startBoard);
    }

    void CreateCardsFromBoard(IBoard board)
    {
        float startAngle = -20.0f;
        float endAngle = 20.0f;
        float distanceFromHolder = 0.7f;
        
        int i = 0;
        foreach (Player player in board.Players)
        {
            if (i >= cardHolders.Count) { break; }
            if (cardHolders[i] is null) { continue; }
            GameObject cardHolder = cardHolders[i]; 
            CreateCardsForHolder(player, cardHolder, startAngle, endAngle, distanceFromHolder);
            i++;
        }
    }

    void CreateCardsForHolder(Player player, GameObject cardHolder, float startAngle=-20.0f, float endAngle=20.0f, 
        float distanceFromHolder=0.75f)
    {
        Transform holderTransform = cardHolder.transform;
        Vector3 holderPosition = holderTransform.position;

        float angleStepSize = (endAngle - startAngle) / (player.Hand.Count - 1);

        int j = 0;
        foreach (Card card in player.Hand)
        {
            float angle = startAngle + j * angleStepSize;
            Vector3 cardPosition = holderTransform.TransformPoint(RelativeCardPosition(distanceFromHolder, angle));
            Quaternion cardRotation = Quaternion.LookRotation(holderPosition - cardPosition);
            GameObject cardObject = Instantiate(cardPrefab, cardPosition, cardRotation, cardHolder.transform);
            cardObject.name = card.ToString();
            CardFrontBackRenderer cardFrontBackRenderer = cardObject.GetComponent<CardFrontBackRenderer>();
            cardFrontBackRenderer.UpdateImage(card);
            j++;
        }
    }

    Vector3 RelativeCardPosition(float distanceFromCentre, float angle)
    {
        if (angle > 90) { throw new ArithmeticException("Angle: " + angle + " should not exceed 90"); }
        if (angle == 0) { return new Vector3(0, 0, 1) * distanceFromCentre; }
        double angleRad = (double)angle * (Math.PI / 180.0f);
        float x = (float)(distanceFromCentre * Math.Sin(angleRad));
        float z = (float)(distanceFromCentre * Math.Cos(angleRad));
        return new Vector3(x, 0, z);
    }
}
