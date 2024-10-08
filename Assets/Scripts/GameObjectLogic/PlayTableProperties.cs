using KarmaLogic.Board;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTableProperties : MonoBehaviour
{
    public Vector3 centre;

    public List<GameObject> boardHolders;
    public GameObject drawPile;
    public GameObject burnPile;
    public GameObject playPile;

    CardObjectPileHandler drawPileHandler;
    CardObjectPileHandler burnPileHandler;
    CardObjectPileHandler playPileHandler;

    void Awake()
    {
        drawPileHandler = drawPile.GetComponent<CardObjectPileHandler>();
        burnPileHandler = burnPile.GetComponent<CardObjectPileHandler>();
        playPileHandler = playPile.GetComponent<CardObjectPileHandler>();
    }

    public void CreateCardObjectPilesFromBoard(IBoard board)
    {
        drawPileHandler.CreatePile(board.DrawPile);
        burnPileHandler.CreatePile(board.BurnPile);
        playPileHandler.CreatePile(board.PlayPile);
    }

    public void MoveCardsToTopOfPlayPile(List<CardObject> cardObjects)
    {
        playPileHandler.MoveCardsToTopOfPile(cardObjects);
    }

    public List<CardObject> DrawCards(int numberOfCards)
    {
        return drawPileHandler.PopCardsFromTop(numberOfCards);
    }

    public List<CardObject> PopAllFromPlayPile()
    {
        return playPileHandler.PopAllCards();
    }

    public void MoveEntirePlayPileToBurnPile()
    {
        List<CardObject> cardObjects = playPileHandler.PopAllCards();
        Debug.Log("Burning " + cardObjects.Count + " many cards");
        burnPileHandler.MoveCardsToTopOfPile(cardObjects);
    }

    public void MoveTopCardsFromPlayPileToBurnPile(int numberOfCards)
    {
        List<CardObject> cardObjects = playPileHandler.PopCardsFromTop(numberOfCards);
        burnPileHandler.MoveCardsToTopOfPile(cardObjects);
    }
}
