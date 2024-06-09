using Karma.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaCardPileManager : MonoBehaviour
{
    public void CreatePile(CardPile pile)
    {
        if (pile.Count == 0) { return; }
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        float yStep = gameManager.cardPrefab.transform.localScale.z;
        float halfHeight = transform.localScale.y;

        int j = 0;
        foreach (Card card in pile)
        {
            float h = (j+1) * yStep;
            Vector3 cardPosition = transform.TransformPoint(new Vector3(0, 0, 0));
            Quaternion cardRotation = Quaternion.Euler(90, 0, 0);

            GameObject cardObject = Instantiate(gameManager.cardPrefab, cardPosition, cardRotation);
            cardObject.transform.SetParent(transform);
            cardObject.transform.position += new Vector3(0, -halfHeight+h, 0);
            gameManager.SetCardObjectProperties(card, cardObject);
            Vector3 correctedScale = cardObject.transform.localScale;
            j++;
        }
    }
}
