using Karma.Board;
using Karma.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaBoardManager : MonoBehaviour
{
    public void CreateKarmaCards(CardsList karmaUp, CardsList karmaDown)
    {
        float width = transform.localScale.x;
        float middleDepth = transform.localScale.z / 2;
        float left = -width / 2;
        float right = width / 2;
        float karmaDownOffset = 0.01f;
        float xStepSize = (right - left) / 2;

        KarmaGameManager gameManager = KarmaGameManager.Instance;

        int j = 0;
        foreach (Card card in karmaUp)
        {
            float x = left + j * xStepSize;  
            Vector3 cardPosition = transform.TransformPoint(new Vector3(x, 0, middleDepth));
            Quaternion cardRotation = Quaternion.Euler(-90, -transform.rotation.eulerAngles.y, 0);
            GameObject cardObject = Instantiate(gameManager.cardPrefab, cardPosition, cardRotation);
            gameManager.SetCardObjectProperties(card, cardObject);
            cardObject.transform.SetParent(transform);
            cardObject.transform.position += new Vector3(0, cardObject.transform.localScale.z - transform.localScale.y, 0);
            Vector3 correctedScale = cardObject.transform.localScale;
            j++;
        }

        j = 0;
        foreach (Card card in karmaDown)
        {
            float x = left + karmaDownOffset + j * xStepSize;
            Vector3 cardPosition = transform.TransformPoint(new Vector3(x, 0, middleDepth));
            Quaternion cardRotation = Quaternion.Euler(90, -transform.rotation.eulerAngles.y, 0);
            GameObject cardObject = Instantiate(gameManager.cardPrefab, cardPosition, cardRotation);
            cardObject.name = card.ToString();
            cardObject.transform.SetParent(transform);
            cardObject.transform.position += new Vector3(0, -transform.localScale.y, 0);
            CardObject cardFrontBackRenderer = cardObject.GetComponent<CardObject>();
            cardFrontBackRenderer.SetCard(card);
            Vector3 correctedScale = cardObject.transform.localScale;
            j++;
        }
    }
}
