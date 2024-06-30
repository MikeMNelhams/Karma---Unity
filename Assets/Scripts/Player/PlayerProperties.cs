using Karma.Board;
using Karma.Controller;
using Karma.Cards;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class PlayerProperties : MonoBehaviour
{
    [SerializeField] PlayerMovementController _playerMovementController;

    public Camera playerCamera;
    public GameObject cardHolder;

    public Button confirmSelectionButton;
    public Button pickupPlayPileButton;

    public IController Controller { get; set; }
    public CardSelector CardSelector { get; protected set; }

    public bool IsRotationEnabled {  get; set; }

    public List<CardObject> CardsInHand { get; set; }
    public CardObject PickedUpCard { get; set; } 

    protected static System.Random rng = new();

    void Awake()
    {
        CardSelector = new();
    }

    private void Update()
    {
        if (IsRotationEnabled && Input.GetMouseButtonDown(1))
        {
            _playerMovementController.ToggleRotation();
            if (_playerMovementController.IsRotating) 
            { 
                _playerMovementController.PlayerRotationEvent += MovePickedUpCard;
            }
            else 
            { 
                _playerMovementController.PlayerRotationEvent -= MovePickedUpCard;
            }
        }

        if (IsRotationEnabled && PickedUpCard != null)
        {
            MovePickedUpCard();
        }
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
        throw new NotImplementedException();
    }

    public void ExitVotingForWinnerMode()
    {
        throw new NotImplementedException();
    }

    public void EnterCardGiveAwaySelectionMode()
    {
        
    }

    public void ExitCardGiveAwaySelectionMode()
    {
        
    }

    public void EnterCardGiveAwayPlayerSelectionMode()
    {
        confirmSelectionButton.gameObject.SetActive(false);
    }

    public void ExitCardGiveAwayPlayerSelectionMode()
    {

    }

    public void SetControllerState(ControllerState newState)
    {
        Controller.SetState(newState);
    }

    public void PopulateHand(List<CardObject> cardObjects, float startAngle=-20.0f, float endAngle=20.0f, float distanceFromHolder=0.75f, float yOffset=-0.25f)
    {
        CardsInHand = cardObjects;

        bool handIsFlipped = KarmaGameManager.Instance.Board.HandsAreFlipped;
        if (handIsFlipped) { ShuffleHand(); }

        Transform holderTransform = cardHolder.transform;
        
        Vector3 holderPosition = holderTransform.position;

        if (cardObjects.Count == 0) { return; }
        if (cardObjects.Count == 1)
        {
            CardObject cardObject = cardObjects[0];
            float middleAngle = (startAngle + endAngle) / 2;
            Vector3 cardPosition = holderTransform.TransformPoint(RelativeCardPositionInHand(distanceFromHolder, middleAngle, yOffset));
            Vector3 lookVector = holderPosition - cardPosition;

            Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
            if (handIsFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

            cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
            return;
        }

        float angleStepSize = (endAngle - startAngle) / (cardObjects.Count - 1);

        int j = 0;
        foreach (CardObject cardObject in cardObjects)
        {
            float angle = startAngle + j * angleStepSize;
            Vector3 cardPosition = holderTransform.TransformPoint(RelativeCardPositionInHand(distanceFromHolder, angle, yOffset));
            Vector3 lookVector = holderPosition - cardPosition;

            Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
            if (handIsFlipped ) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

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

        PopulateHand(CardsInHand);
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

    Vector3 RelativeCardPositionInHand(float distanceFromCentre, float angle, float yOffset)
    {
        if (angle > 90) { throw new ArithmeticException("Angle for cards in hand: " + angle + " should not exceed 90"); }
        if (angle == 0) { return new Vector3(0, yOffset, distanceFromCentre); }
        double angleRad = (double)angle * (Math.PI / 180.0f);
        float x = (float)(distanceFromCentre * Math.Sin(angleRad));
        float z = (float)(distanceFromCentre * Math.Cos(angleRad));
        return new Vector3(x, yOffset, z);
    }

    void MovePickedUpCard()
    {
        float distanceFromHolder = 0.75f;
        Vector3 cardPosition = cardHolder.transform.TransformPoint(playerCamera.transform.forward * distanceFromHolder);
        Quaternion cardRotation = Quaternion.LookRotation(playerCamera.transform.position - cardPosition);

        PickedUpCard.transform.SetPositionAndRotation(cardPosition, cardRotation);
        IsInFrontOnLeftClick();
    }

    void IsInFrontOnLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerProperties playerProperties = PlayerInFrontOfPickedUpCard;
            if (playerProperties != null)
            {
                print("Player has been HIT by raycast!!");
            }
        }
    }

    public PlayerProperties PlayerInFrontOfPickedUpCard
    {
        get
        {
            print("Performing raycast from: " + PickedUpCard.transform.position + " along " + playerCamera.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(PickedUpCard.transform.position, playerCamera.transform.forward);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.parent == null) { continue; }
                print("Hit SOMETHING: " + hit.transform.parent.name);
                PlayerProperties playerProperties = hit.transform.parent.gameObject.GetComponent<PlayerProperties>();
                if (playerProperties)
                {
                    return playerProperties;
                }
            }
            return null;
        }
    }
}
