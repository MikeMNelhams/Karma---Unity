using Karma.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaCardPileHandler : MonoBehaviour
{
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
        float yStep = CardDepth;
        float halfHeight = transform.localScale.y;

        Quaternion cardRotation = CardRotation();
        Vector3 cardPosition = transform.TransformPoint(new Vector3(0, 0, 0));

        int j = 0;
        foreach (Card card in pile)
        {
            float h = (j+1) * yStep;
            GameObject cardObject = Instantiate(gameManager.cardPrefab, cardPosition, cardRotation);
            cardObject.transform.SetParent(transform);
            cardObject.transform.position += new Vector3(0, -halfHeight+h, 0);
            gameManager.SetCardObjectProperties(card, cardObject);
            Vector3 correctedScale = cardObject.transform.localScale;

            CardObjects.Add(cardObject.GetComponent<CardObject>());
            j++;
        }
    }

    public void MoveCardsToTopOfPile(List<CardObject> cardObjects)
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        GameObject cardPrefab = gameManager.cardPrefab;
        float cardDepth = CardDepth;
        float halfHeight = transform.localScale.y;
        float cardIndex = CardObjects.Count;

        Vector3 cardStartPosition = transform.TransformPoint(new Vector3(0, 0, 0));
        Quaternion cardRotation = CardRotation();

        foreach (CardObject cardObject in cardObjects)
        {
            cardObject.DisableSelectShader();
            CardObjects.Add(cardObject);
            float h = cardIndex * cardDepth;
            
            cardObject.transform.SetPositionAndRotation(transform.position + new Vector3(0, -halfHeight + h, 0), cardRotation);
            cardIndex++;
        }
    }

    Quaternion CardRotation()
    {
        Quaternion faceRotation = _isFaceUp ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(90, 0, 0);
        return transform.rotation * faceRotation;
    }

    float CardDepth
    {
        get
        {
            return KarmaGameManager.Instance.cardPrefab.transform.localScale.z;
        }
    }
}
