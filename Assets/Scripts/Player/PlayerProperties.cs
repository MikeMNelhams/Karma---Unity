using Karma.Board;
using Karma.Controller;
using Karma.Cards;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class PlayerProperties : MonoBehaviour
{
    [SerializeField] PlayerMovementController _playerMovementController;
    [SerializeField] float _rayCastDistanceCutoff = 30f;
    [SerializeField] CardHandPhysicsInfo _handPhysicsInfo = CardHandPhysicsInfo.Default;

    public Camera playerCamera;
    public GameObject cardHolder;

    public Button confirmSelectionButton;
    public Button pickupPlayPileButton;

    public IController Controller { get; set; }
    public CardSelector CardSelector { get; protected set; }

    public bool IsRotationEnabled {  get; set; }
    public bool IsPointingEnabled { get; set; }

    public int Index { get; set; } = -1;
    public List<CardObject> CardsInHand { get; set; }
    public CardObject PickedUpCard { get; set; }

    public delegate void OnLeftClickRayCastListener(int giverIndex, int targetIndex);
    event OnLeftClickRayCastListener PickedUpCardOnClick;
    event OnLeftClickRayCastListener OnPointingAtPlayer;

    public delegate Dictionary<Card, List<int>> CardSorter(int playerIndex);
    CardSorter _handSorter;

    protected static System.Random rng = new();

    int _layerAsLayerMask;
    PlayerProperties _targetPlayerProperties;

    void Awake()
    {
        CardsInHand = new();
        CardSelector = new();
        _layerAsLayerMask = 1 << LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1)) { return; }

        if (Controller.State is SelectingPlayPileGiveAwayPlayerIndex && IsPointingEnabled)
        {
            _playerMovementController.TogglePointing();
            if (_playerMovementController.IsPointing)
            {
                _playerMovementController.RegisterPlayerPointingEventListener(ChoosePointedPlayerIfValid);
            }
            else
            {
                _playerMovementController.UnRegisterPlayerPointingEventListener(ChoosePointedPlayerIfValid);
            }
        }
        else if (IsRotationEnabled)
        {
            _playerMovementController.ToggleRotation();
            if (_playerMovementController.IsRotating) 
            {
                _playerMovementController.RegisterPlayerRotationEventListener(MovePickedUpCardIfValid);
            }
            else 
            {
                _playerMovementController.UnRegisterPlayerRotationEventListener(MovePickedUpCardIfValid);
            }
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
        confirmSelectionButton.gameObject.SetActive(true);
    }

    public void ExitCardGiveAwaySelectionMode()
    {
        confirmSelectionButton.gameObject.SetActive(false);
    }

    public void EnterCardGiveAwayPlayerSelectionMode()
    {
        confirmSelectionButton.gameObject.SetActive(false);
        _playerMovementController.SetRotating(true);
        _playerMovementController.RegisterPlayerRotationEventListener(MovePickedUpCardIfValid);
    }

    public void ExitCardGiveAwayPlayerSelectionMode()
    {

    }
    
    public void EnterPlayPileGiveAwaySelectionMode()
    {
        confirmSelectionButton.gameObject.SetActive(false);
        _playerMovementController.SetPointing(true);
        _playerMovementController.RegisterPlayerPointingEventListener(ChoosePointedPlayerIfValid);
    }

    public void ExitPlayPileGiveAwaySelectionMode()
    {
        _playerMovementController.SetPointing(false);
    }

    public void SetControllerState(ControllerState newState)
    {
        Controller.SetState(newState);
    }

    public void SetHandSorter(CardSorter handSorter)
    {
        _handSorter = handSorter;
    }

    public void AddCardObjectsToHand(List<CardObject> cardsToAdd)
    {
        if (cardsToAdd.Count == 0) { return; }
        int n = cardsToAdd.Count + CardsInHand.Count;
        if (n == 1) { PopulateHand(cardsToAdd); return; }

        List<CardObject> combinedHandCardObjects = CardsInHand;
        combinedHandCardObjects.AddRange(cardsToAdd);

        foreach (CardObject cardObject in cardsToAdd)
        {
            SetCardObjectOnMouseDownEvent(cardObject);
        }

        Dictionary<Card, List<int>> cardPositions = _handSorter(Index);

        CardObject[] handCorrectOrder = new CardObject[n];
        foreach (CardObject cardObject in combinedHandCardObjects)
        {
            int position = cardPositions[cardObject.CurrentCard].Last();
            handCorrectOrder[position] = cardObject;
            Debug.Log("Moving card: " + cardObject.CurrentCard + cardObject + " to position: " + position);
            List<int> positions = cardPositions[cardObject.CurrentCard];
            positions.RemoveAt(positions.Count - 1);
            cardObject.transform.SetParent(cardHolder.transform);
        }

        List<CardObject> finalHandCardObjects = new();
        for (int i = 0; i < handCorrectOrder.Length; i++)
        {
            if (handCorrectOrder[i] != null)
            {
                finalHandCardObjects.Add(handCorrectOrder[i]);
            }
        }

        string handCardObjectsMessage = "Hand (CardObjects):";
        foreach (CardObject cardObject in finalHandCardObjects)
        {
            handCardObjectsMessage += " " + cardObject.name;
        }
        Debug.Log(handCardObjectsMessage);
        PopulateHand(finalHandCardObjects);
    }

    public void PopulateHand(List<CardObject> cardObjects, CardHandPhysicsInfo cardHandPhysicsInfo = null)
    {
        CardsInHand = cardObjects;
        PopulateHand(cardHandPhysicsInfo);
    }
    
    public void PopulateHand(CardHandPhysicsInfo cardHandPhysicsInfo = null)
    {
        if (cardHandPhysicsInfo != null) { _handPhysicsInfo = cardHandPhysicsInfo; }

        bool handIsFlipped = KarmaGameManager.Instance.Board.HandsAreFlipped;
        if (handIsFlipped) { ShuffleHand(); }

        Transform holderTransform = cardHolder.transform;

        Vector3 holderPosition = holderTransform.position;

        if (CardsInHand.Count == 0) { return; }
        float startAngle = _handPhysicsInfo.startAngle;
        float endAngle = _handPhysicsInfo.endAngle;
        float distanceFromHolder = _handPhysicsInfo.distanceFromHolder;
        float yOffset = _handPhysicsInfo.yOffset;

        if (CardsInHand.Count == 1)
        {
            CardObject cardObject = CardsInHand[0];
            float middleAngle = (startAngle + endAngle) / 2;
            Vector3 cardPosition = holderTransform.TransformPoint(RelativeCardPositionInHand(distanceFromHolder, middleAngle, yOffset));
            Vector3 lookVector = holderPosition - cardPosition;

            Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
            if (handIsFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

            cardObject.transform.SetPositionAndRotation(cardPosition, cardRotation);
            return;
        }

        float angleStepSize = (endAngle - startAngle) / (CardsInHand.Count - 1);

        int j = 0;
        foreach (CardObject cardObject in CardsInHand)
        {
            float angle = startAngle + j * angleStepSize;
            Vector3 cardPosition = holderTransform.TransformPoint(RelativeCardPositionInHand(distanceFromHolder, angle, yOffset));
            Vector3 lookVector = holderPosition - cardPosition;

            Quaternion cardRotation = lookVector.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(lookVector);
            if (handIsFlipped) { cardRotation *= Quaternion.Euler(new Vector3(0, 180, 0)); }

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

        PopulateHand();
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

    public void ReceivePickedUpCard(PlayerProperties giverPlayerProperties)
    {
        if (giverPlayerProperties.PickedUpCard == null) { throw new NullReferenceException();  }
        AddCardObjectsToHand(new List<CardObject>() { giverPlayerProperties.PickedUpCard });
        giverPlayerProperties.RemoveCardObjectOnMouseDownEvent(giverPlayerProperties.PickedUpCard);
        giverPlayerProperties.CardsInHand.Remove(giverPlayerProperties.PickedUpCard); // TODO this should be PlayableCards.Remove(), but KU and KD aren't physically interactable yet
        giverPlayerProperties.PickedUpCard = null;
        giverPlayerProperties.PopulateHand();
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

    void MovePickedUpCardIfValid()
    {
        if (!IsRotationEnabled || PickedUpCard == null) { return; }

        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        MovePickedUpCard();
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            TriggerPickedUpCardOnLeftClick();
        }
    }

    void MovePickedUpCard()
    {
        if (PickedUpCard == null) { return; }

        float distanceFromHolder = 0.75f;
        Vector3 cardPosition = cardHolder.transform.TransformPoint(playerCamera.transform.forward * distanceFromHolder);
        
        PickedUpCard.transform.position = cardPosition;

        Quaternion cardRotation; 
        if (_targetPlayerProperties != null)
        {
            Quaternion lookDirection = Quaternion.LookRotation(cardPosition - playerCamera.transform.position);
            cardRotation = lookDirection; // Quaternion.Euler(90, 180, 0) *
            cardRotation *= Quaternion.Euler(75, 180, 0);
        }
        else
        {
            cardRotation = Quaternion.LookRotation(playerCamera.transform.position - cardPosition);
        }
        
        PickedUpCard.transform.rotation = cardRotation;
    }

    void ChoosePointedPlayerIfValid()
    {
        if (!IsPointingEnabled || !Input.GetMouseButtonDown(0)) { return; }
 
        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        TriggerTargetPickUpPlayPile();
    }

    public void RegisterPickedUpCardOnClickEventListener(OnLeftClickRayCastListener eventListener)
    {
        PickedUpCardOnClick += eventListener;
    }

    void TriggerPickedUpCardOnLeftClick()
    {
        if (_targetPlayerProperties != null)
        {
            PickedUpCardOnClick?.Invoke(Index, _targetPlayerProperties.Index);
        }
    }

    public void RegisterTargetPickUpPlayPileEventListener(OnLeftClickRayCastListener eventListener)
    {
        OnPointingAtPlayer += eventListener;
    }

    void TriggerTargetPickUpPlayPile()
    {
        if (_targetPlayerProperties != null)
        {
            OnPointingAtPlayer?.Invoke(Index, _targetPlayerProperties.Index);
        }
    }

    public PlayerProperties TargetPlayerInFrontOfPlayer
    {
        get
        {
            RaycastHit firstHit;
            Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out firstHit, _rayCastDistanceCutoff, _layerAsLayerMask);

            if (firstHit.transform == null) { return null; }
            if (firstHit.transform.parent == null) { return null; }
            PlayerProperties playerProperties = firstHit.transform.parent.gameObject.GetComponent<PlayerProperties>();
            return playerProperties;
        }
    }
}

[System.Serializable]
public class CardHandPhysicsInfo
{
    public float startAngle;
    public float endAngle;
    public float distanceFromHolder;
    public float yOffset;

    public CardHandPhysicsInfo(float startAngle = -20.0f, float endAngle = 20.0f, float distanceFromHolder = 0.75f, float yOffset = -0.25f)
    {
        this.startAngle = startAngle;
        this.endAngle = endAngle;
        this.distanceFromHolder = distanceFromHolder;
        this.yOffset = yOffset;
    }

    public static CardHandPhysicsInfo Default { get => new (); }
}