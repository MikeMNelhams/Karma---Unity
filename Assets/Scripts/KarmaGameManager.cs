using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Karma.Game;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Players;
using Karma.Cards;
using System;
using DataStructures;

public class KarmaGameManager : MonoBehaviour
{
    private static KarmaGameManager _instance;
    public static KarmaGameManager Instance { get { return _instance; } }
    Game game;
    public GameObject cardPrefab;
    public List<GameObject> handHolders;
    public List<GameObject> boardHolders;
    public GameObject drawPile;
    public GameObject burnPile;
    public GameObject playPile;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        BasicBoard startBoard = BoardFactory.RandomStart(4, 3);
        CreatePlayerCardsFromBoard(startBoard);
        CreateCardPilesFromBoard(startBoard);
    }

    private void CreateCardPilesFromBoard(BasicBoard startBoard)
    {
        KarmaCardPileManager drawPileManager = drawPile.GetComponent<KarmaCardPileManager>();
        drawPileManager.CreatePile(startBoard.DrawPile);
        KarmaCardPileManager burnPileManager = burnPile.GetComponent<KarmaCardPileManager>();
        burnPileManager.CreatePile(startBoard.BurnPile);
        KarmaCardPileManager playPileManager = playPile.GetComponent<KarmaCardPileManager>();
        playPileManager.CreatePile(startBoard.PlayPile);
    }

    void CreatePlayerCardsFromBoard(IBoard board)
    {
        float startAngle = -20.0f;
        float endAngle = 20.0f;
        float distanceFromHolder = 0.7f;
        
        for (int i = 0; i < board.Players.Count; i++)
        {
            Player player = board.Players[i];
            if (i >= handHolders.Count) { break; }
            if (handHolders[i] is null) { continue; }
            GameObject cardHolder = handHolders[i];
            CreateCardsForHolder(player, cardHolder, startAngle, endAngle, distanceFromHolder);
        }

        for (int i = 0; i < board.Players.Count; i++)
        {
            Player player = board.Players[i];
            if (i >= boardHolders.Count) { break; }
            if (boardHolders[i] is null) { continue; }
            GameObject boardHolder = boardHolders[i];
            KarmaBoardManager karmaBoardManager = boardHolder.GetComponent<KarmaBoardManager>();
            karmaBoardManager.CreateKarmaCards(player.KarmaUp, player.KarmaDown);
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
            SetCardObjectProperties(card, cardObject);
            j++;
        }
    }

    public void SetCardObjectProperties(Card card, GameObject cardObject)
    {
        cardObject.name = card.ToString();
        CardFrontBackRenderer cardFrontBackRenderer = cardObject.GetComponent<CardFrontBackRenderer>();
        cardFrontBackRenderer.UpdateImage(card);
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
