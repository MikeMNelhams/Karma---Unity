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
using UnityEngine.UI;
using Karma.Controller;

public class KarmaGameManager : MonoBehaviour
{
    private static KarmaGameManager _instance;
    public static KarmaGameManager Instance { get { return _instance; } }
    public GameObject cardPrefab;
    public List<GameObject> handHolders;
    public List<GameObject> boardHolders;
    public GameObject drawPile;
    public GameObject burnPile;
    public GameObject playPile;

    public List<Button> cardSelectConfirmButtons;
    public List<Button> pickupPlayPileButtons;
    List<CardSelector> _cardSelectors;

    Game game;

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
        _cardSelectors = new List<CardSelector>();
        for (int i = 0; i < startBoard.Players.Count; i++) { _cardSelectors.Add(new CardSelector()); }
        CreatePlayerCardsFromBoard(startBoard);
        CreateCardPilesFromBoard(startBoard);

        List<IController> controllers = new() { new PlayerController(), new PlayerController(), new PlayerController(), new PlayerController() };
        
        game = new Game(startBoard, controllers);
        AssignButtonEvents(game);
        
        game.PlayTurn();
    }

    public void SetCardObjectProperties(Card card, GameObject cardObject)
    {
        cardObject.name = card.ToString();
        CardObject cardRenderer = cardObject.GetComponent<CardObject>();
        cardRenderer.UpdateImage(card);
    }

    void CreateCardPilesFromBoard(BasicBoard startBoard)
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
            if (handHolders[i] == null) { continue; }
            GameObject cardHolder = handHolders[i];
            CreateCardsForHolder(player, i, cardHolder, startAngle, endAngle, distanceFromHolder);
        }

        for (int i = 0; i < board.Players.Count; i++)
        {
            Player player = board.Players[i];
            if (i >= boardHolders.Count) { break; }
            if (boardHolders[i] == null) { continue; }
            GameObject boardHolder = boardHolders[i];
            KarmaBoardManager karmaBoardManager = boardHolder.GetComponent<KarmaBoardManager>();
            karmaBoardManager.CreateKarmaCards(player.KarmaUp, player.KarmaDown);
        }   
    }

    void CreateCardsForHolder(Player player, int playerIndex, GameObject cardHolder, float startAngle=-20.0f, float endAngle=20.0f, 
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
            SetCardObjectOnMouseDownEvent(card, cardObject, playerIndex);
            j++;
        }
    }

    void SetCardObjectOnMouseDownEvent(Card card, GameObject cardObject, int playerIndex)
    {
        CardObject cardRenderer = cardObject.GetComponent<CardObject>();
        cardRenderer.OnCardClick += _cardSelectors[playerIndex].Toggle;
    }

    void AssignButtonEvents(Game game)
    {
        int j = 0;
        for (int i = 0; i < game.Board.Players.Count; i++) 
        {
            if (game.Controllers[i].GetType() == typeof(PlayerController))
            {
                if (j >= cardSelectConfirmButtons.Count) { break; }
                if (cardSelectConfirmButtons[j] == null) { continue; }
                cardSelectConfirmButtons[j].onClick.AddListener(delegate { TriggerCardsSelected(i); });
                j++;
            }
        }

        j = 0;
        for (int i = 0; i < game.Board.Players.Count; i++)
        {
            if (game.Controllers[i].GetType() == typeof(PlayerController))
            {
                if (j >= pickupPlayPileButtons.Count) { break; }
                if (pickupPlayPileButtons[j] == null) { continue; }
                pickupPlayPileButtons[j].onClick.AddListener(delegate { TriggerPickupActionSelected(i); });
                j++;
            }
        }
    }

    void TriggerCardsSelected(int playerIndex)
    {
        CardSelector cardSelector = _cardSelectors[playerIndex];
        IController controller = game.Controllers[playerIndex];
        controller.SelectedCardValues = cardSelector.SelectionCardValues;
        controller.IsAwaitingInput = false;
    }

    void TriggerPickupActionSelected(int playerIndex)
    {
        IController controller = game.Controllers[playerIndex];
        controller.SelectedAction = new PickupPlayPile();
        controller.IsAwaitingInput = false;
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
