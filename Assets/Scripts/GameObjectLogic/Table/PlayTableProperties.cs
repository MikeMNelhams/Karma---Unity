using KarmaLogic.Board;
using System.Collections.Generic;
using UnityEngine;

public class PlayTableProperties : MonoBehaviour
{
    [SerializeField] Vector3 _centre;
    
    [SerializeField] CardObjectPileHandler _drawPileHandler;
    [SerializeField] CardObjectPileHandler _burnPileHandler;
    [SerializeField] CardObjectPileHandler _playPileHandler;

    [SerializeField] List<GameObject> _boardHolders;

    public Vector3 Centre { get => _centre; }

    public List<GameObject> KarmaUpDownHolders { get => _boardHolders; }
    public GameObject DrawPile { get => _drawPileHandler.gameObject; }
    public GameObject BurnPile { get => _burnPileHandler.gameObject; }
    public GameObject PlayPile { get => _playPileHandler.gameObject; }

    public void CreateCardObjectPilesFromBoard(IBoard board)
    {
        _drawPileHandler.CreatePile(board.DrawPile);
        _burnPileHandler.CreatePile(board.BurnPile);
        _playPileHandler.CreatePile(board.PlayPile);
    }

    public void MoveCardsToTopOfPlayPile(List<SelectableCardObject> cardObjects)
    {
        _playPileHandler.MoveCardsToTopOfPile(cardObjects);
    }

    public List<SelectableCardObject> DrawCards(int numberOfCards)
    {
        return _drawPileHandler.PopCardsFromTop(numberOfCards);
    }

    public List<SelectableCardObject> PopAllFromPlayPile()
    {
        return _playPileHandler.PopAllCards();
    }

    public void MoveEntirePlayPileToBurnPile()
    {
        List<SelectableCardObject> cardObjects = _playPileHandler.PopAllCards();
        Debug.Log("Burning " + cardObjects.Count + " many cards");
        _burnPileHandler.MoveCardsToTopOfPile(cardObjects);
    }

    public void MoveTopCardsFromPlayPileToBurnPile(int numberOfCards)
    {
        List<SelectableCardObject> cardObjects = _playPileHandler.PopCardsFromTop(numberOfCards);
        _burnPileHandler.MoveCardsToTopOfPile(cardObjects);
    }
}
