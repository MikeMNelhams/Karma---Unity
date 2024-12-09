using KarmaLogic.Board;
using PlayTable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayTableProperties : MonoBehaviour
{
    [SerializeField] CardObjectPileHandler _drawPileHandler;
    [SerializeField] CardObjectPileHandler _playPileHandler;
    [SerializeField] CardObjectPileHandler _burnPileHandler;
    
    [SerializeField] CircularTable _tableGeometry;

    public CardObjectPileHandler DrawPile { get => _drawPileHandler; }
    public CardObjectPileHandler PlayPile { get => _playPileHandler; }
    public CardObjectPileHandler BurnPile { get => _burnPileHandler; }
    
    public CircularTable TableGeometry { get => _tableGeometry; }

    public void CreateCardObjectPilesFromBoard(IBoard board)
    {
        _drawPileHandler.CreatePile(board.DrawPile);
        _burnPileHandler.CreatePile(board.BurnPile);
        _playPileHandler.CreatePile(board.PlayPile);
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
        _burnPileHandler.MoveCardsToTop(cardObjects);
    }

    public void MoveTopCardsFromPlayPileToBurnPile(int numberOfCards)
    {
        List<SelectableCardObject> cardObjects = _playPileHandler.PopCardsFromTop(numberOfCards);
        _burnPileHandler.MoveCardsToTop(cardObjects);
    }
}
