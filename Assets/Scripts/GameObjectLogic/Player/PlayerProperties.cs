using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KarmaLogic.Board;
using KarmaLogic.Controller;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using DataStructures;
using CardVisibility;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerProperties : MonoBehaviour, ICharacterProperties, ICardVisibilityHandler
{
    [SerializeField] PlayerMovementController _playerMovementController;
    [SerializeField] FanHandler _fanHandler;

    [SerializeField] Camera _playerCamera;
    [SerializeField] PhysicsRaycaster _playerCameraPhysicsRaycaster;
    [SerializeField] Canvas _playerCanvas;
    [SerializeField] GameObject _cardHolder;
     
    [SerializeField] float _rayCastCutoff = 30f;

    public Button PickupPlayPileButton { get => _pickupPlayPileButton; }
    public Button ConfirmSelectionButton { get => _confirmSelectionButton; }
    public Button ClearSelectionButton { get => _clearSelectionButton; }

    [SerializeField] Button _confirmSelectionButton;
    [SerializeField] Button _clearSelectionButton;
    [SerializeField] Button _pickupPlayPileButton;

    [SerializeField] Image nextPlayerLeftArrow;
    [SerializeField] Image nextPlayerRightArrow;

    [SerializeField] HoverToolTipHandler _hoverTipHandler;

    public HoverToolTipHandler HoverTipHandler { get => _hoverTipHandler; }
    public Controller Controller { get; set; }
    public CardSelector CardSelector { get; protected set; }

    public bool IsRotationEnabled { get; set; }
    public bool IsPointingEnabled { get; set; }

    public int Index { get; set; } = -1;

    public ListWithConstantContainsCheck<SelectableCard> CardsInHand { get; set; }
    
    public ListWithConstantContainsCheck<SelectableCard> CardsInKarmaUp { get; set; }

    public ListWithConstantContainsCheck<SelectableCard> CardsInKarmaDown { get; set; }
    
    public SelectableCard PickedUpCard { get; set; }

    public delegate void OnLeftClickRayCastListener(int giverIndex, int targetIndex);
    event OnLeftClickRayCastListener PickedUpCardOnClick;
    event OnLeftClickRayCastListener OnPointingAtPlayer;
    event OnLeftClickRayCastListener OnVoteForTarget;

    public delegate Dictionary<Card, List<int>> CardSorter(int playerIndex);
    CardSorter _handSorter;

    int _layerAsLayerMask;
    PlayerProperties _targetPlayerProperties;

    bool _isKarmaDownFlippedUp = false;
    public bool IsToolTipsEnabled { get; set; } = true;

    RaycastHit[] _hits;
    bool _isLeftButtonMouseDown = false;
    Vector2 _mousePosition;
    bool _areLegalMovesHighlighted = true;

    void Awake()
    {
        CardsInHand = new();
        CardsInKarmaUp = new();
        CardsInKarmaDown = new();
        CardSelector = new();
        _layerAsLayerMask = 1 << LayerMask.NameToLayer("Player");
        _hits = new RaycastHit[5];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1)) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            _isLeftButtonMouseDown = true;
            _mousePosition = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            TogglePlayerMovementListeners();
            return;
        }
    }

    void FixedUpdate()
    {
        if (!_isLeftButtonMouseDown) { return; }
        if (Controller.State is not WaitForTurn) { TrySelectCardObject(); }
        _isLeftButtonMouseDown = false;
    }

    void TrySelectCardObject()
    {
        int numberOfHits = Physics.RaycastNonAlloc(_playerCamera.ScreenPointToRay(_mousePosition), _hits, _rayCastCutoff);

        if (numberOfHits == 1) { return; }
        System.Array.Sort(_hits, (a, b) => (a.distance.CompareTo(b.distance)));

        CardObject cardObject = null;
        foreach (var hit in _hits)
        {
            if (hit.transform == null) { continue; }
            if (hit.transform.TryGetComponent<CardObject>(out cardObject)) { break; }
        }

        if (cardObject == null) { return; }

        TryToggleCardSelect(cardObject);

        for (int i = 0; i < _hits.Length; i++)
        {
            _hits[i] = new RaycastHit();
        }
    }

    public void TogglePlayerMovementListeners()
    {
        if (Controller.State is SelectingPlayPileGiveAwayPlayerIndex && IsPointingEnabled)
        {
            _playerMovementController.TogglePointing();
            if (_playerMovementController.IsPointing)
            {
                _playerMovementController.RegisterPlayerPointingEventListener(ChoosePointedPlayerToPickUpPlayPileIfValid);
            }
            else
            {
                _playerMovementController.UnRegisterPlayerPointingEventListener(ChoosePointedPlayerToPickUpPlayPileIfValid);
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

    public void TryToggleCardSelect(SelectableCard cardObject)
    {
        // Replace the callback system with just using the existing raycast in update loop. This way we can access the camera and WAY more info
        if (!CardIsSelectable(cardObject)) { return; }
        if (!cardObject.IsVisible(Index)) { return; }
        cardObject.ToggleSelectShader();
        CardSelector.Toggle(cardObject);
        TryColorLegalCards();
    }

    public void EnableCamera()
    {
        print("Enabling camera for player: " + Index);
        _playerCamera.enabled = true;
        _playerCamera.tag = "MainCamera";
        _playerCameraPhysicsRaycaster.enabled = true;
        _playerCanvas.enabled = true;
    }

    public void DisableCamera()
    {
        print("Disabling camera for player: " + Index);
        _playerCamera.tag = "Untagged";
        _playerCamera.enabled = false;
        _playerCameraPhysicsRaycaster.enabled = false;
        _playerCanvas.enabled = false;
    }

    public void HideUI()
    {
        ConfirmSelectionButton.gameObject.SetActive(false);
        ClearSelectionButton.gameObject.SetActive(false);
        PickupPlayPileButton.gameObject.SetActive(false);
        nextPlayerLeftArrow.gameObject.SetActive(false);
        nextPlayerRightArrow.gameObject.SetActive(false);
    }

    public void EnablePlayerMovement()
    {
        IsRotationEnabled = true;
        IsPointingEnabled = true;
    }

    public void DisablePlayerMovement()
    {
        IsRotationEnabled = false;
        IsPointingEnabled = false;
    }

    public PlayingFrom SelectingFrom { 
        get 
        { 
            if (CardsInHand.Count > 0) { return PlayingFrom.Hand; }
            if (CardsInKarmaUp.Count > 0) { return PlayingFrom.KarmaUp; }
            if (CardsInKarmaDown.Count > 0) { return PlayingFrom.KarmaDown; }
            return PlayingFrom.Empty;
        }
    }

    public ListWithConstantContainsCheck<SelectableCard> SelectableCardObjects
    {
        get
        {
            return SelectingFrom switch
            {
                PlayingFrom.Hand => CardsInHand,
                PlayingFrom.KarmaUp => CardsInKarmaUp,
                PlayingFrom.KarmaDown => CardsInKarmaDown,
                _ => new ListWithConstantContainsCheck<SelectableCard>(),
            };
        }
    }

    public bool CardIsSelectable(SelectableCard card)
    {
        if (card == null) { throw new NullReferenceException(); }
        return SelectableCardObjects.Contains(card);
    }

    public void InstantiatePlayerHandFan(Hand hand)
    {
        ListWithConstantContainsCheck<SelectableCard> cardObjects = new();
        KarmaGameManager gameManager = KarmaGameManager.Instance;

        foreach (Card card in hand)
        {
            CardObject cardObject = gameManager.InstantiateCard(card, Vector3.zero, Quaternion.identity, _cardHolder).GetComponent<CardObject>();
            ParentCardToThis(cardObject);
            cardObjects.Add(cardObject);
        }

        UpdateHand(cardObjects);
        TryColorLegalCards();
    }

    public void TryColorLegalCards()
    {
        if (!_areLegalMovesHighlighted) { ColorSelectableCardsAsDefault(); return; }
        KarmaGameManager karmaGameManager = KarmaGameManager.Instance;

        foreach (SelectableCard cardObject in SelectableCardObjects)
        {
            karmaGameManager.ColorLegalCard(cardObject, CardSelector);
        }
    }

    void ColorSelectableCardsAsDefault()
    {
        foreach (SelectableCard cardObject in SelectableCardObjects)
        {
            cardObject.ResetCardBorder();
        }
    }

    public void EnterPickingActionUpdateUI()
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        HashSet<BoardPlayerAction> legalActions = gameManager.Board.CurrentLegalActions;
        if (legalActions.Count == 0) { return; }
        if (legalActions.Contains(gameManager.PickUpAction))
        {
            PickupPlayPileButton.gameObject.SetActive(true);
        }
        if (legalActions.Contains(gameManager.PlayCardsComboAction))
        {
            ConfirmSelectionButton.gameObject.SetActive(true);
            ClearSelectionButton.gameObject.SetActive(true);
        }
        if (gameManager.Board.TurnOrder == BoardTurnOrder.RIGHT)
        {
            nextPlayerRightArrow.gameObject.SetActive(true);
        }
        if (gameManager.Board.TurnOrder == BoardTurnOrder.LEFT)
        {
            nextPlayerLeftArrow.gameObject.SetActive(true);
        }
        TryColorLegalCards();
    }

    public void ExitPickingActionUpdateUI()
    {
        PickupPlayPileButton.gameObject.SetActive(false);
    }

    public void EnterVotingForWinner()
    {
        HideUI();
        _playerMovementController.SetPointing(true);
        _playerMovementController.RegisterPlayerPointingEventListener(VoteForPointedPlayerToWinIfValid);
    }

    public void ExitVotingForWinner()
    {
        _playerMovementController.SetPointing(false);
        _playerMovementController.UnRegisterPlayerPointingEventListener(VoteForPointedPlayerToWinIfValid);
    }

    public void EnterCardGiveAwaySelection()
    {
        ConfirmSelectionButton.gameObject.SetActive(true);
        ClearSelectionButton.gameObject.SetActive(true);
    }

    public void ExitCardGiveAwaySelection()
    {
        ConfirmSelectionButton.gameObject.SetActive(false);
        ClearSelectionButton.gameObject.SetActive(false);
    }

    public void EnterCardGiveAwayPlayerIndexSelection()
    {
        HideUI();
        _playerMovementController.SetRotating(true);
        _playerMovementController.RegisterPlayerRotationEventListener(MovePickedUpCardIfValid);
    }

    public void ExitCardGiveAwayPlayerIndexSelection()
    {
        _playerMovementController.UnRegisterPlayerRotationEventListener(MovePickedUpCardIfValid);
    }
    
    public void EnterPlayPileGiveAwayPlayerIndexSelection()
    {
        ConfirmSelectionButton.gameObject.SetActive(false);
        ClearSelectionButton.gameObject.SetActive(false);
        _playerMovementController.SetPointing(true);
        _playerMovementController.RegisterPlayerPointingEventListener(ChoosePointedPlayerToPickUpPlayPileIfValid);
    }

    public void ExitPlayPileGiveAwayPlayerIndexSelection()
    {
        _playerMovementController.SetPointing(false);
        _playerMovementController.UnRegisterPlayerPointingEventListener(ChoosePointedPlayerToPickUpPlayPileIfValid);
    }

    /// <summary>
    /// Asynchronous
    /// </summary>
    /// <param name="newState"></param>
    public void SetControllerState(ControllerState newState)
    {
        if (Controller is BotController)
        {
            StartCoroutine(SetBotControllerState(newState));
        }
        else
        {
            Controller.SetState(newState);
            print("STATE for player: " + Index + ": " + Controller.State.GetHashCode());
        }
    }

    IEnumerator SetBotControllerState(ControllerState newState)
    {
        BotController controller = Controller as BotController;
        yield return new WaitForSeconds(controller.DelaySeconds);
        controller.SetState(newState);
        print("STATE for player: " + Index + ": " + Controller.State.GetHashCode());
    }

    public void SetHandSorter(CardSorter handSorter)
    {
        _handSorter = handSorter;
    }

    public void AddCardObjectsToHand(List<SelectableCard> cardsToAdd)
    {
        if (cardsToAdd.Count == 0) { return; }
        int n = cardsToAdd.Count + CardsInHand.Count;
        if (n == 1) { UpdateHand(new ListWithConstantContainsCheck<SelectableCard>(cardsToAdd)); return; }

        ListWithConstantContainsCheck<SelectableCard> combinedHandCardObjects = CardsInHand;
        combinedHandCardObjects.AddRange(cardsToAdd);

        foreach (SelectableCard cardObject in cardsToAdd)
        {
            ParentCardToThis(cardObject);
        }

        Dictionary<Card, List<int>> cardPositions = _handSorter(Index);

        SelectableCard[] handCorrectOrder = new CardObject[n];
        foreach (SelectableCard cardObject in combinedHandCardObjects)
        {
            int position = cardPositions[cardObject.CurrentCard].Last();
            handCorrectOrder[position] = cardObject;
            List<int> positions = cardPositions[cardObject.CurrentCard];
            positions.RemoveAt(positions.Count - 1);
        }

        ListWithConstantContainsCheck<SelectableCard> finalHandCardObjects = new();
        for (int i = 0; i < handCorrectOrder.Length; i++)
        {
            if (handCorrectOrder[i] != null)
            {
                finalHandCardObjects.Add(handCorrectOrder[i]);
            }
        }

        UpdateHand(finalHandCardObjects);
    }

    public void SortHand(int[] sortOrder)
    {
        ListWithConstantContainsCheck<SelectableCard> sortedHand = new();
        for (int i = 0; i < sortOrder.Length; i++)
        {
            sortedHand.Add(CardsInHand[sortOrder[i]]);
        }
        
        UpdateHand(sortedHand);
    }

    public void UpdateHand(ListWithConstantContainsCheck<SelectableCard> cardObjects, FanPhysicsInfo fanPhysicsInfo = null)
    {
        CardsInHand = cardObjects;
        TryColorLegalCards();
        UpdateHand(fanPhysicsInfo);
    }

    public void UpdateHand(FanPhysicsInfo fanPhysicsInfo = null)
    {
        bool fanIsFlipped = KarmaGameManager.Instance.Board.HandsAreFlipped;
        fanPhysicsInfo ??= FanPhysicsInfo.Default;
        _fanHandler.TransformCardsIntoFan(CardsInHand, fanIsFlipped, fanPhysicsInfo);
    }

    [ContextMenu("Turn Off Legal Move Hints")]
    public void TurnOffLegalMoveHints()
    {
        _areLegalMovesHighlighted = false;
        foreach (SelectableCard cardObject in SelectableCardObjects)
        {
            cardObject.ResetCardBorder();
        }
    }

    [ContextMenu("Turn On Legal Move Hints")]
    public void TurnOnLegalMoveHints()
    {
        _areLegalMovesHighlighted = true;
        TryColorLegalCards();
    }

    [ContextMenu("Redraw Fan")]
    public void UpdateHand()
    {
        UpdateHand(null);
    }

    public void FlipHand()
    {
        _fanHandler.FlipFan(CardsInHand);
    }

    public void FlipKarmaDownCardsUp()
    {
        _isKarmaDownFlippedUp = true;
        foreach (SelectableCard cardObject in CardsInKarmaDown)
        {
            cardObject.transform.rotation = Quaternion.Euler(-90, -transform.rotation.eulerAngles.y, 0);
        }

        FrozenMultiSet<CardValue> selectedCards = CardSelector.SelectionCardValues;

        TryColorLegalCards();
    }

    public List<SelectableCard> PopSelectedCardsFromSelection()
    {
        List<SelectableCard> cardObjects = CardSelector.CardObjects.ToList<SelectableCard>();
        foreach (SelectableCard cardObject in cardObjects)
        {
            cardObject.DisableSelectShader();
            CardSelector.Remove(cardObject);
            cardObject.ResetCardBorder();
        }

        SelectableCardObjects.RemoveRange(cardObjects);

        if (SelectingFrom == PlayingFrom.Hand) { UpdateHand(); }
        if (!_isKarmaDownFlippedUp && SelectingFrom == PlayingFrom.KarmaUp) { FlipKarmaDownCardsUp(); }

        TryColorLegalCards();

        return cardObjects;
    }

    public void ParentCardToThis(SelectableCard cardObject)
    {
        cardObject.SetParent(this, _cardHolder.transform);
    }

    public void ReceivePickedUpCard(PlayerProperties giverPlayerProperties)
    {
        if (giverPlayerProperties.PickedUpCard == null) { throw new NullReferenceException();  }

        AddCardObjectsToHand(new List<SelectableCard>() { giverPlayerProperties.PickedUpCard });
        giverPlayerProperties.PickedUpCard = null;
        giverPlayerProperties.UpdateHand();
        TryColorLegalCards();
    }

    void MovePickedUpCardIfValid()
    {
        if (!IsRotationEnabled || PickedUpCard == null) { return; }

        MovePickedUpCard();
        if (Input.GetMouseButtonDown(0) && _targetPlayerProperties != null)
        {
            TriggerTargetReceivePickedUpCard(_targetPlayerProperties.Index);
        }
    }

    void MovePickedUpCard()
    {
        if (PickedUpCard == null) { return; }

        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        float distanceFromHolder = 0.75f;
        Vector3 cardPosition = _playerCamera.transform.position + _playerCamera.transform.forward * distanceFromHolder;
        
        PickedUpCard.transform.position = cardPosition;

        Quaternion cardRotation;
        if (_targetPlayerProperties != null)
        {
            Quaternion lookDirection = Quaternion.LookRotation(cardPosition - _playerCamera.transform.position);
            cardRotation = lookDirection; // Quaternion.Euler(90, 180, 0) *
            cardRotation *= Quaternion.Euler(75, 180, 0);
        }
        else
        {
            cardRotation = Quaternion.LookRotation(_playerCamera.transform.position - cardPosition);
        }

        PickedUpCard.transform.rotation = cardRotation;
    }

    void ChoosePointedPlayerToPickUpPlayPileIfValid()
    {
        if (!IsPointingEnabled || !Input.GetMouseButtonDown(0)) { return; }
 
        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        if (_targetPlayerProperties == null) { return; }

        TriggerTargetPickUpPlayPile(_targetPlayerProperties.Index);
    }

    void VoteForPointedPlayerToWinIfValid()
    {
        if (!IsPointingEnabled || !Input.GetMouseButtonDown(0)) { return; }
        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        if (_targetPlayerProperties == null) { return; }
        if (!KarmaGameManager.Instance.ValidPlayerIndicesForVoting.Contains(TargetPlayerInFrontOfPlayer.Index)) { return; }
        TriggerVoteForPlayer(TargetPlayerInFrontOfPlayer.Index);
    }

    public void RegisterPickedUpCardOnClickEventListener(OnLeftClickRayCastListener eventListener)
    {
        PickedUpCardOnClick += eventListener;
    }

    public void TriggerTargetReceivePickedUpCard(int targetIndex)
    {
        PickedUpCardOnClick?.Invoke(Index, targetIndex);
    }

    public void RegisterTargetPickUpPlayPileEventListener(OnLeftClickRayCastListener eventListener)
    {
        OnPointingAtPlayer += eventListener;
    }

    public void TriggerTargetPickUpPlayPile(int targetIndex)
    {
        OnPointingAtPlayer?.Invoke(Index, targetIndex);
    }

    public void RegisterVoteForTargetEventListener(OnLeftClickRayCastListener eventListener)
    {
        OnVoteForTarget += eventListener;
    }

    public void TriggerVoteForPlayer(int targetIndex)
    {
        OnVoteForTarget?.Invoke(Index, targetIndex);
    }

    public bool IsVisible(int observerPlayerIndex)
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        if (gameManager.Board.HandsAreFlipped)
        {
            return observerPlayerIndex != Index;
        }

        return observerPlayerIndex == Index;
    }

    public PlayerProperties TargetPlayerInFrontOfPlayer
    {
        get
        {
            Physics.Raycast(PickedUpCard.transform.position, _playerCamera.transform.forward, out RaycastHit firstHit, _rayCastCutoff, _layerAsLayerMask);

            if (firstHit.transform == null) { return null; }
            if (firstHit.transform.parent == null) { return null; }
            PlayerProperties playerProperties = firstHit.transform.parent.gameObject.GetComponent<PlayerProperties>();
            return playerProperties;
        }
    }
}
