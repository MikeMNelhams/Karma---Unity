using KarmaLogic.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KarmaCardPileHandler : MonoBehaviour
{
    [SerializeField] Transform _pileDebugCube;
    [SerializeField] bool _isFaceUp = false;
    public List<CardObject> CardObjects { get; protected set; }

    private void Awake()
    {
        CardObjects = new();
    }

    public void CreatePile(CardPile pile)
    {
        if (pile.Count == 0) { return; }
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        float yStep = gameManager.CardTransform.localScale.z;
        float halfHeight = BottomY;

        Quaternion cardRotation = CardRotation();

        int j = 0;
        foreach (Card card in pile)
        {
            float h = (j+1) * yStep;
            Vector3 cardPosition = transform.TransformPoint(new(0, -halfHeight + h, 0));
            GameObject cardObject = gameManager.InstantiateCard(card, cardPosition, cardRotation, gameObject);

            CardObjects.Add(cardObject.GetComponent<CardObject>());
            j++;
        }
    }

    public void MoveCardsToTopOfPile(List<CardObject> cardObjects)
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        GameObject cardPrefab = gameManager.cardPrefab;
        float cardDepth = gameManager.CardTransform.localScale.z;
        float halfHeight = BottomY;
        float cardIndex = CardObjects.Count;

        Quaternion cardRotation = CardRotation();

        foreach (CardObject cardObject in cardObjects)
        {
            cardObject.DisableSelectShader();
            cardObject.transform.SetParent(transform);
            
            CardObjects.Add(cardObject);
            float h = cardIndex * cardDepth;
            cardObject.transform.SetPositionAndRotation(transform.position + new Vector3(0, -halfHeight + h, 0), cardRotation);
            cardIndex++;
        }
    }

    public List<CardObject> PopCardsFromTop(int numberOfCards)
    {
        List<CardObject> removedCardObjects = new();
        for (int i = 0; i < numberOfCards; i++)
        {
            removedCardObjects.Add(CardObjects[^(i+1)]);
        }
        CardObjects.RemoveRange(CardObjects.Count - numberOfCards, numberOfCards);

        return removedCardObjects;
    }

    public List<CardObject> PopAllCards()
    {
        List<CardObject> removedCardObjects = new();
        print("During Popping all there are: " + CardObjects.Count + " many cards");
        foreach (CardObject cardObject in CardObjects)
        {
            print("Copying cardObject: " + cardObject);
            removedCardObjects.Add(cardObject);
        }
        CardObjects = new List<CardObject>();
        return removedCardObjects;
    }

    Quaternion CardRotation()
    {
        Quaternion faceRotation = _isFaceUp ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(90, 0, 0);
        return transform.rotation * faceRotation;
    }

    float BottomY 
    { 
        get 
        { 
            return _pileDebugCube.localScale.y / 2;
        } 
    }
}
