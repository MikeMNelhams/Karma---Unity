using Karma.Board;
using Karma.Controller;
using Karma.Cards;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerProperties : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject head;
    public GameObject cardHolder;

    public Button confirmSelectionButton;
    public Button pickupPlayPileButton;

    public List<CardObject> CardsInHand { get; set; }

    public IController Controller {  get; set; }
    public CardSelector CardSelector { get; protected set; }

    protected static System.Random rng = new();

    // Start is called before the first frame update
    void Awake()
    {
        CardSelector = new();
    }

    public void EnableCamera()
    {
        playerCamera.enabled = true;
        playerCamera.tag = "MainCamera";
    }

    public void DisableCamera()
    {
        playerCamera.tag = "Untagged";
        playerCamera.enabled = false;
    }

    public void HideUI()
    {
        confirmSelectionButton.gameObject.SetActive(false);
        pickupPlayPileButton.gameObject.SetActive(false);
    }

    public void EnterPickingActionMode()
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        HashSet<BoardPlayerAction> legalActions = gameManager.Board.CurrentLegalActions;
        if (legalActions.Count == 0) { return; }
        if (legalActions.Contains(gameManager.PickUpAction)) 
        { 
            pickupPlayPileButton.gameObject.SetActive(true); 
        }
        if (legalActions.Contains(gameManager.PlayCardsComboAction)) 
        { 
            confirmSelectionButton.gameObject.SetActive(true); 
        }
    }

    public void ExitPickingActionMode()
    {
        pickupPlayPileButton.gameObject.SetActive(false);
    }

    public void EnterVotingForWinnerMode()
    {

    }

    public void ExitVotingForWinnerMode()
    {

    }

    public void SetControllerState(ControllerState newState)
    {
        Controller.SetState(newState);
    }

    public void PopulateHand(List<CardObject> cardObjects, float startAngle=-20.0f, float endAngle=20.0f, float distanceFromHolder=0.75f)
    {
        CardsInHand = cardObjects;

        bool handIsFlipped = KarmaGameManager.Instance.Board.HandsAreFlipped;
        if (handIsFlipped) { ShuffleHand(); }

        Transform holderTransform = cardHolder.transform;
        Vector3 holderPosition = holderTransform.position;
        float angleStepSize = (endAngle - startAngle) / (cardObjects.Count - 1);

        int j = 0;
        foreach (CardObject cardObject in cardObjects)
        {
            float angle = startAngle + j * angleStepSize;
            Vector3 cardPosition = holderTransform.TransformPoint(RelativeCardPositionInHand(distanceFromHolder, angle));
            Quaternion cardRotation = Quaternion.LookRotation(holderPosition - cardPosition);

            if (handIsFlipped )
            {
                cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0));
            }

            cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
            j++;
        }
    }
    
    public void FlipHand()
    {
        bool handIsFlipped = KarmaGameManager.Instance.Board.HandsAreFlipped;
        if (handIsFlipped ) { ShuffleHand(); }

        foreach (CardObject cardObject in CardsInHand)
        {
            cardObject.transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    public List<CardObject> PopSelectedCardsFromHand()
    {
        List<CardObject> cardObjects = CardSelector.CardObjects.ToList<CardObject>();
        foreach (CardObject cardObject in cardObjects)
        {
            cardObject.DisableSelectShader();
            RemoveCardObjectOnMouseDownEvent(cardObject);
            CardsInHand.Remove(cardObject);
        }
        return cardObjects;
    }

    public void SetCardObjectOnMouseDownEvent(CardObject cardObject)
    {
        cardObject.OnCardClick += CardSelector.Toggle;
    }

    public void RemoveCardObjectOnMouseDownEvent(CardObject cardObject)
    {
        CardSelector.Remove(cardObject);
        cardObject.OnCardClick -= CardSelector.Toggle;
    }

    public void ShuffleHand()
    {
        // Fisher-Yates Shuffle: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        int n = CardsInHand.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (CardsInHand[n], CardsInHand[k]) = (CardsInHand[k], CardsInHand[n]);
        }
    }

    Vector3 RelativeCardPositionInHand(float distanceFromCentre, float angle)
    {
        if (angle > 90) { throw new ArithmeticException("Angle: " + angle + " should not exceed 90"); }
        if (angle == 0) { return new Vector3(0, 0, 1) * distanceFromCentre; }
        double angleRad = (double)angle * (Math.PI / 180.0f);
        float x = (float)(distanceFromCentre * Math.Sin(angleRad));
        float z = (float)(distanceFromCentre * Math.Cos(angleRad));
        return new Vector3(x, 0, z);
    }
}
