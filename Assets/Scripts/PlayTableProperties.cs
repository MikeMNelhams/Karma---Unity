using Karma.Board;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTableProperties : MonoBehaviour
{
    [SerializeField] public Vector3 centre;

    public List<GameObject> boardHolders;
    public GameObject drawPile;
    public GameObject burnPile;
    public GameObject playPile;

    KarmaCardPileHandler drawPileHandler;
    KarmaCardPileHandler burnPileHandler;
    KarmaCardPileHandler playPileHandler;

    void Awake()
    {
        drawPileHandler = drawPile.GetComponent<KarmaCardPileHandler>();
        burnPileHandler = burnPile.GetComponent<KarmaCardPileHandler>();
        playPileHandler = playPile.GetComponent<KarmaCardPileHandler>();
    }

    public void CreateCardPilesFromBoard(IBoard board)
    {
        drawPileHandler.CreatePile(board.DrawPile);
        burnPileHandler.CreatePile(board.BurnPile);
        playPileHandler.CreatePile(board.PlayPile);
    }

    public void MoveCardsToTopOfPlayPile(List<CardObject> cardObjects)
    {
        playPileHandler.MoveCardsToTopOfPile(cardObjects);
    }
}
