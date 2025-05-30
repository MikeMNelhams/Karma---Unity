using CardVisibility;
using KarmaLogic.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CardObjectPileHandler : MonoBehaviour, ICardVisibilityHandler
{
    [SerializeField] Transform _pileDebugCube;
    [SerializeField] bool _isFaceUp = false;
    [SerializeField] CardPileMode _cardPileMode = CardPileMode.TIDY;

    CardPilePhysicsInfo _pilePhysicsInfo;
    CardPositionAndRotationParams _cardPositionAndRotationParams;

    public List<SelectableCardObject> CardObjects { get; protected set; }

    void Awake()
    {
        CardObjects = new();
    }

    public void CreatePile(CardPile pile)
    {
        UpdateCardPileMode();
        _cardPositionAndRotationParams = new CardPositionAndRotationParams(_pilePhysicsInfo, BottomY, CardRotation());
        
        if (pile.Count == 0) { return; }
        KarmaGameManager gameManager = KarmaGameManager.Instance;

        for (int i = 0; i < pile.Count; i++)
        {
            Card card = pile[i];
            Tuple<Vector3, Quaternion> positionAndRotation = CardPositionAndRotation(i, _cardPositionAndRotationParams);
            GameObject cardGameObject = gameManager.InstantiateCard(card, positionAndRotation.Item1, positionAndRotation.Item2, gameObject);
            CardObject cardObject = cardGameObject.GetComponent<CardObject>();

            cardObject.SetParent(this, this.transform);

            CardObjects.Add(cardObject.GetComponent<CardObject>());
        }
    }

    public void MoveCardsToTop(List<SelectableCardObject> cardObjects)
    {
        int cardIndex = CardObjects.Count;
        for (int i = 0; i < cardObjects.Count; i++) 
        {
            SelectableCardObject cardObject = cardObjects[i];
            cardObject.DisableSelectShader();
            cardObject.transform.SetParent(transform);
            
            CardObjects.Add(cardObject);
            Tuple<Vector3, Quaternion> positionAndRotation = CardPositionAndRotation(cardIndex, _cardPositionAndRotationParams);
            cardObject.transform.SetPositionAndRotation(positionAndRotation.Item1, positionAndRotation.Item2);
            cardIndex++;
        }
    }

    public List<SelectableCardObject> PopCardsFromTop(int numberOfCards)
    {
        List<SelectableCardObject> removedCardObjects = new();
        for (int i = 0; i < numberOfCards; i++)
        {
            removedCardObjects.Add(CardObjects[^(i+1)]);
        }
        CardObjects.RemoveRange(CardObjects.Count - numberOfCards, numberOfCards);

        return removedCardObjects;
    }

    public List<SelectableCardObject> PopCardsFromBottom(int numberOfCards)
    {
        List<SelectableCardObject> removedCardObjects = new();
        for (int i = 0; i < numberOfCards; i++)
        {
            removedCardObjects.Add(CardObjects[i]);
        }
        CardObjects.RemoveRange(0, numberOfCards);
        return removedCardObjects;
    }

    public List<SelectableCardObject> PopAllCards()
    {
        List<SelectableCardObject> removedCardObjects = new();
        foreach (SelectableCardObject cardObject in CardObjects)
        {
            removedCardObjects.Add(cardObject);
        }
        CardObjects = new List<SelectableCardObject>();
        return removedCardObjects;
    }

    public void DestroyCards()
    {
        foreach (SelectableCardObject cardObject in CardObjects)
        {
            Destroy(cardObject.gameObject);
        }

        CardObjects.Clear();
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

    void RedrawCardPile()
    {
        for (int i = 0; i < CardObjects.Count; i++)
        {
            SelectableCardObject cardObject = CardObjects[i];
            Tuple<Vector3, Quaternion> positionAndRotation = CardPositionAndRotation(i, _cardPositionAndRotationParams);

            cardObject.gameObject.transform.SetPositionAndRotation(positionAndRotation.Item1, positionAndRotation.Item2);
        }
    }

    [ContextMenu("Redraw CardPile")]
    void UpdateCardPileMode()
    {
        _pilePhysicsInfo = _cardPileMode switch
        {
            CardPileMode.MESSY => CardPilePhysicsInfo.Messy,
            CardPileMode.TIDY => CardPilePhysicsInfo.Tidy,
            _ => CardPilePhysicsInfo.PerfectlyVertical,
        };
        _cardPositionAndRotationParams = new CardPositionAndRotationParams(_pilePhysicsInfo, BottomY, CardRotation());
        if (CardObjects.Count > 0) { RedrawCardPile(); }
    }

    enum CardPileMode : sbyte
    {
        MESSY = 0,
        TIDY = 1,
        PERFECTLY_VERTICAL = 2
    }

    Tuple<Vector3, Quaternion> CardPositionAndRotation(int index, CardPositionAndRotationParams physicsParams)
    {
        // The card centre will never exceed the boundary of the card below it! This way the cards are always 'balancing' on other cards for realistic piles.
        // Uses 3 RNG normal distribution samples per card.
        float h = (index + 1) * physicsParams.yStep;

        float xRNG = UnityEngine.Random.value; // 0 <= xRNG <= 1
        float x = physicsParams.xMax * (2 * xRNG - 1);  // -xMax <= x <= xMax

        float zRNG = UnityEngine.Random.value; // 0 <= zRNG <= 1
        float z = physicsParams.zMax * (2 * zRNG - 1); // -zMax <= z <= zMax

        float zRotationRNG = UnityEngine.Random.value; // 0 <= zRotationRNG <= 1
        float zRotation = physicsParams.zRotationMax * (2 * zRotationRNG - 1); // -zRotationMax <= z <= zRotationMax

        Vector3 cardPosition = transform.TransformPoint(new(x, -physicsParams.yOffset + h, z));
        Quaternion rotationOffset = Quaternion.Euler(new Vector3(0, 0, zRotation));
        return new Tuple<Vector3, Quaternion>(cardPosition, physicsParams.defaultCardRotation * rotationOffset);
    }

    public bool IsVisible(int observerPlayerIndex)
    {
        return _isFaceUp;
    }

    public bool IsOwnedBy(int observerPlayerIndex)
    {
        return false;
    }

    class CardPositionAndRotationParams
    {
        // Inner class intermediate between CardPilePhysicsInfo and the CardObjectPileHandler. 
        public float yStep;
        public float yOffset;

        public float cardXscale;
        public float cardZscale;

        public float xMax;
        public float zMax;
        public float zRotationMax;
        public Quaternion defaultCardRotation;

        public CardPositionAndRotationParams(CardPilePhysicsInfo pilePhysicsInfo, float halfPileCubeHeight, Quaternion defaultCardRotation)
        {
            KarmaGameManager gameManager = KarmaGameManager.Instance;
            cardXscale = gameManager.CardBounds.size.x;
            cardZscale = gameManager.CardBounds.size.y;
            yOffset = halfPileCubeHeight;
            yStep = gameManager.CardBounds.size.z;

            this.defaultCardRotation = defaultCardRotation;
            xMax = cardXscale * (pilePhysicsInfo._maxXOffsetPercent / 100.0f);
            zMax = cardZscale * (pilePhysicsInfo._maxZOffsetPercent / 100.0f);
            zRotationMax = pilePhysicsInfo._maxZRotation;
        }
    }
}
