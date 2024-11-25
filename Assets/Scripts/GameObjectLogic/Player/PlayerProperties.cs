using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using DataStructures;
using CardVisibility;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using StateMachine;
using StateMachine.CharacterStateMachines;
using TMPro;

public class PlayerProperties : MonoBehaviour, ICardVisibilityHandler
{
    [SerializeField] PlayerMovementController _playerMovementController;
    [SerializeField] FanHandler _fanHandler;

    [SerializeField] Camera _playerCamera;
    [SerializeField] PhysicsRaycaster _playerCameraPhysicsRaycaster;
    [SerializeField] Canvas _playerCanvas;
    [SerializeField] GameObject _cardHolder;
     
    [SerializeField] float _rayCastCutoff = 30f;
    
    public ButtonAwaitable PickupPlayPileButton { get => _pickupPlayPileButton; }
    public ButtonAwaitable ConfirmSelectionButton { get => _confirmSelectionButton; }
    public ButtonAwaitable ClearSelectionButton { get => _clearSelectionButton; }
    public ButtonAwaitable FinishMulliganButton { get => _finishMulliganButton; }

    [Header("Buttons")]
    [SerializeField] ButtonAwaitable _confirmSelectionButton;
    [SerializeField] ButtonAwaitable _clearSelectionButton;
    [SerializeField] ButtonAwaitable _pickupPlayPileButton;
    [SerializeField] ButtonAwaitable _finishMulliganButton;

    [Header("Player UI other")]
    [SerializeField] Image _nextPlayerLeftArrow;
    [SerializeField] Image _nextPlayerRightArrow;

    [SerializeField] HoverToolTipHandler _hoverTipHandler;

    [SerializeField] TextMeshProUGUI _currentStateTextBox;

    public HoverToolTipHandler HoverTipHandler { get => _hoverTipHandler; }
    public StateMachine<State, Command> StateMachine { get; set; }
    public CardSelector CardSelector { get; protected set; }

    public bool IsRotationEnabled { get; set; }
    public bool IsPointingEnabled { get; set; }

    public int Index { get; set; } = -1;

    public ListWithConstantContainsCheck<SelectableCardObject> CardsInHand { get; set; }
    
    public ListWithConstantContainsCheck<SelectableCardObject> CardsInKarmaUp { get; set; }

    public ListWithConstantContainsCheck<SelectableCardObject> CardsInKarmaDown { get; set; }
    
    public SelectableCardObject PickedUpCard { get; set; }

    public delegate void OnLeftClickRayCastListener(int giverIndex, int targetIndex);
    public delegate Task OnLeftClickRayCastAwaitableListener(int giverIndex, int targetIndex);
    event OnLeftClickRayCastListener PickedUpCardOnClick;
    event OnLeftClickRayCastListener OnVoteForTarget;

    List<OnLeftClickRayCastAwaitableListener> _onPointingAtPlayerAwaitableListeners;

    public delegate Dictionary<Card, List<int>> CardSorter(int playerIndex);
    CardSorter _handSorter;

    int _layerAsLayerMask;
    PlayerProperties _targetPlayerProperties;

    public bool IsKarmaDownFlippedUp { get; protected set; } = false;
    public bool IsToolTipsEnabled { get; set; } = true;

    RaycastHit[] _hits;
    bool _isLeftButtonMouseDown = false;
    bool _isRightButtonMouseDown = false;
    Vector2 _mousePosition;
    bool _areLegalMovesHighlighted = true;

    void Awake()
    {
        CardsInHand = new ();
        CardsInKarmaUp = new ();
        CardsInKarmaDown = new ();
        CardSelector = new ();
        _layerAsLayerMask = 1 << LayerMask.NameToLayer("Player");
        _hits = new RaycastHit[5];
        _onPointingAtPlayerAwaitableListeners = new ();
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
            _isRightButtonMouseDown = true;
            return;
        }
    }

    void FixedUpdate()
    {
        if (!_isLeftButtonMouseDown && !_isRightButtonMouseDown) { return; }
        if (_isLeftButtonMouseDown)
        {
            if (StateMachine.CurrentState is not State.WaitingForTurn)
            {
                TrySelectCardObject();
            }
            _isLeftButtonMouseDown = false;
        }
        if (_isRightButtonMouseDown)
        {
            if (KarmaGameManager.Instance.Board.CurrentPlayerIndex == Index)
            {
                TogglePlayerMovementListeners();
            }
            _isRightButtonMouseDown = false;
        }
    }

    void TrySelectCardObject()
    {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null;
        if (isOverUI) 
        { 
            return; 
        }

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
        if (StateMachine.CurrentState is State.SelectingPlayPileGiveAwayPlayerIndex && IsPointingEnabled)
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

    public void TryToggleCardSelect(SelectableCardObject cardObject)
    {
        if (!cardObject.IsOwnedBy(Index)) { return; }
        if (StateMachine.CurrentState is not State.Mulligan && !IsCardSelectable(cardObject)) { return; }
        
        cardObject.ToggleSelectShader();
        CardSelector.Toggle(cardObject);
        TryColorLegalCards();
    }

    public Task EnableCamera()
    {
        print("Enabling camera for player: " + Index);
        _playerCamera.enabled = true;
        _playerCamera.tag = "MainCamera";
        _playerCameraPhysicsRaycaster.enabled = true;
        _playerCanvas.enabled = true;
        return Task.CompletedTask;
    }

    public Task DisableCamera()
    {
        print("Disabling camera for player: " + Index);
        _playerCamera.tag = "Untagged";
        _playerCamera.enabled = false;
        _playerCameraPhysicsRaycaster.enabled = false;
        _playerCanvas.enabled = false;
        return Task.CompletedTask;
    }

    public Task HideUI()
    {
        ConfirmSelectionButton.gameObject.SetActive(false);
        ClearSelectionButton.gameObject.SetActive(false);
        PickupPlayPileButton.gameObject.SetActive(false);
        FinishMulliganButton.gameObject.SetActive(false);
        _nextPlayerLeftArrow.gameObject.SetActive(false);
        _nextPlayerRightArrow.gameObject.SetActive(false);
        return Task.CompletedTask;
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

    public ListWithConstantContainsCheck<SelectableCardObject> SelectableCardObjects
    {
        get
        {
            return SelectingFrom switch
            {
                PlayingFrom.Hand => CardsInHand,
                PlayingFrom.KarmaUp => CardsInKarmaUp,
                PlayingFrom.KarmaDown => CardsInKarmaDown,
                _ => new ListWithConstantContainsCheck<SelectableCardObject>(),
            };
        }
    }

    public bool IsCardSelectable(SelectableCardObject card)
    {
        if (card == null) { throw new NullReferenceException(); }
        return SelectableCardObjects.Contains(card);
    }

    public void InstantiatePlayerHandFan(Hand hand)
    {
        ListWithConstantContainsCheck<SelectableCardObject> cardObjects = new();
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
        if (!_areLegalMovesHighlighted || StateMachine.CurrentState is State.Mulligan) { ColorSelectableCardsAsDefault(); return; }
        KarmaGameManager karmaGameManager = KarmaGameManager.Instance;

        if (StateMachine.CurrentState is not State.SelectingCardGiveAwayIndex)
        {
            foreach (SelectableCardObject cardObject in SelectableCardObjects)
            {
                karmaGameManager.ColorLegalPlayableCard(cardObject, CardSelector);
            }
        }
        else
        {
            foreach (SelectableCardObject cardObject in SelectableCardObjects)
            {
                karmaGameManager.ColorLegalGiveableCard(cardObject, CardSelector);
            }
        }
    }

    public async Task ProcessStateCommand(Command command)
    {
        await StateMachine.MoveNext(command);
        await UpdateDisplayedDebugInfo();
    }

    public Task UpdateDisplayedDebugInfo()
    {
        _currentStateTextBox.text = "Most recent STATE:\n" + StateMachine.CurrentState;
        return Task.CompletedTask;
    }

    public Task EnterMulligan()
    {
        ConfirmSelectionButton.gameObject.SetActive(true);
        ClearSelectionButton.gameObject.SetActive(true);
        FinishMulliganButton.gameObject.SetActive(true);
        TryColorLegalCards();
        return Task.CompletedTask;
    }

    public Task ExitMulligan()
    {
        ConfirmSelectionButton.gameObject.SetActive(false);
        ClearSelectionButton.gameObject.SetActive(false);
        FinishMulliganButton.gameObject.SetActive(false);
        return Task.CompletedTask;
    }

    public Task EnterPickingActionUpdateUI()
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        HashSet<BoardPlayerAction> legalActions = gameManager.Board.CurrentLegalActions;
        if (legalActions.Count == 0) { return Task.CompletedTask; }
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
            _nextPlayerRightArrow.gameObject.SetActive(true);
        }
        if (gameManager.Board.TurnOrder == BoardTurnOrder.LEFT)
        {
            _nextPlayerLeftArrow.gameObject.SetActive(true);
        }
        TryColorLegalCards();
        return Task.CompletedTask;
    }

    public Task ExitPickingActionUpdateUI()
    {
        PickupPlayPileButton.gameObject.SetActive(false);
        return Task.CompletedTask;
    }

    public Task EnterVotingForWinner()
    {
        HideUI();
        _playerMovementController.SetPointing(true);
        _playerMovementController.RegisterPlayerPointingEventListener(VoteForPointedPlayerToWinIfValid);
        return Task.CompletedTask;
    }

    public Task ExitVotingForWinner()
    {
        _playerMovementController.SetPointing(false);
        _playerMovementController.UnRegisterPlayerPointingEventListener(VoteForPointedPlayerToWinIfValid);
        return Task.CompletedTask;
    }

    public Task EnterCardGiveAwaySelection()
    {
        ConfirmSelectionButton.gameObject.SetActive(true);
        ClearSelectionButton.gameObject.SetActive(true);
        return Task.CompletedTask;
    }

    public Task EnterCardGiveAwayPlayerIndexSelection()
    {
        HideUI();
        _playerMovementController.SetRotating(true);
        _playerMovementController.RegisterPlayerRotationEventListener(MovePickedUpCardIfValid);

        print("Is rotating: " + _playerMovementController.IsRotating);
        print("Mouse state: " + Cursor.lockState);
        //TogglePlayerMovementListeners();
        return Task.CompletedTask;
    }

    public Task ExitCardGiveAwayPlayerIndexSelection()
    {
        _playerMovementController.UnRegisterPlayerRotationEventListener(MovePickedUpCardIfValid);
        return Task.CompletedTask;
    }
    
    public Task EnterPlayPileGiveAwayPlayerIndexSelection()
    {
        ConfirmSelectionButton.gameObject.SetActive(false);
        ClearSelectionButton.gameObject.SetActive(false);
        PickupPlayPileButton.gameObject.SetActive(false);
        _playerMovementController.SetPointing(true);
        _playerMovementController.RegisterPlayerPointingEventListener(ChoosePointedPlayerToPickUpPlayPileIfValid);
        return Task.CompletedTask;
    }

    public Task ExitPlayPileGiveAwayPlayerIndexSelection()
    {
        _playerMovementController.SetPointing(false);
        _playerMovementController.UnRegisterPlayerPointingEventListener(ChoosePointedPlayerToPickUpPlayPileIfValid);
        return Task.CompletedTask;
    }

    public void SetHandSorter(CardSorter handSorter)
    {
        _handSorter = handSorter;
    }

    public void AddCardObjectsToHand(List<SelectableCardObject> cardsToAdd)
    {
        if (cardsToAdd.Count == 0) { return; }
        int n = cardsToAdd.Count + CardsInHand.Count;
        if (n == 1) 
        {
            ParentCardToThis(cardsToAdd[0]); 
            UpdateHand(new ListWithConstantContainsCheck<SelectableCardObject>(cardsToAdd)); 
            return; 
        }

        ListWithConstantContainsCheck<SelectableCardObject> combinedHandCardObjects = CardsInHand;
        combinedHandCardObjects.AddRange(cardsToAdd);

        foreach (SelectableCardObject cardObject in cardsToAdd)
        {
            ParentCardToThis(cardObject);
        }

        Dictionary<Card, List<int>> cardPositions = _handSorter(Index);

        SelectableCardObject[] handCorrectOrder = new CardObject[n];
        foreach (SelectableCardObject cardObject in combinedHandCardObjects)
        {
            int position = cardPositions[cardObject.CurrentCard].Last();
            handCorrectOrder[position] = cardObject;
            List<int> positions = cardPositions[cardObject.CurrentCard];
            positions.RemoveAt(positions.Count - 1);
        }

        ListWithConstantContainsCheck<SelectableCardObject> finalHandCardObjects = new();
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
        ListWithConstantContainsCheck<SelectableCardObject> sortedHand = new();
        for (int i = 0; i < sortOrder.Length; i++)
        {
            sortedHand.Add(CardsInHand[sortOrder[i]]);
        }
        
        UpdateHand(sortedHand);
    }

    public void UpdateHand(ListWithConstantContainsCheck<SelectableCardObject> cardObjects, FanPhysicsInfo fanPhysicsInfo = null)
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
        foreach (SelectableCardObject cardObject in SelectableCardObjects)
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
        if (CardsInHand == null || CardsInHand.Count == 0) { return; }
        _fanHandler.FlipFan(CardsInHand);
    }

    public void FlipKarmaDownCardsUp()
    {
        IsKarmaDownFlippedUp = true;
        foreach (SelectableCardObject cardObject in CardsInKarmaDown)
        {

            cardObject.transform.rotation *= Quaternion.AngleAxis(180, cardObject.transform.forward);
        }
    }

    public void SwapHandWithKarmaUp(int handIndex, int karmaUpIndex)
    {
        SelectableCardObject handCardObject = CardsInHand[handIndex];
        SelectableCardObject karmaUpCardObject = CardsInKarmaUp[karmaUpIndex];
        (handCardObject.transform.position, karmaUpCardObject.transform.position) = (karmaUpCardObject.transform.position, handCardObject.transform.position);
        (handCardObject.transform.rotation, karmaUpCardObject.transform.rotation) = (karmaUpCardObject.transform.rotation, handCardObject.transform.rotation);
        (CardsInHand[handIndex], CardsInKarmaUp[karmaUpIndex]) = (CardsInKarmaUp[karmaUpIndex], CardsInHand[handIndex]);
    }

    public List<SelectableCardObject> PopSelectedCardsFromSelection()
    {
        List<SelectableCardObject> cardObjects = CardSelector.CardObjects.ToList<SelectableCardObject>();
        foreach (SelectableCardObject cardObject in cardObjects)
        {
            cardObject.DisableSelectShader();
            CardSelector.Remove(cardObject);
            cardObject.ResetCardBorder();
        }

        SelectableCardObjects.RemoveRange(cardObjects);

        if (SelectingFrom == PlayingFrom.Hand) { UpdateHand(); }
        if (!IsKarmaDownFlippedUp && SelectingFrom == PlayingFrom.KarmaDown) { FlipKarmaDownCardsUp(); }

        return cardObjects;
    }

    public void ParentCardToThis(SelectableCardObject cardObject)
    {
        cardObject.SetParent(this, _cardHolder.transform);
    }

    public void ReceivePickedUpCard(PlayerProperties giverPlayerProperties)
    {
        if (giverPlayerProperties.PickedUpCard == null) { throw new NullReferenceException();  }
        AddCardObjectsToHand(new List<SelectableCardObject>() { giverPlayerProperties.PickedUpCard });
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

    async void ChoosePointedPlayerToPickUpPlayPileIfValid()
    {
        if (!IsPointingEnabled || !Input.GetMouseButtonDown(0)) { return; }
 
        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        if (_targetPlayerProperties == null) { return; }

        await TriggerTargetPickUpPlayPile(_targetPlayerProperties.Index);
    }

    void VoteForPointedPlayerToWinIfValid()
    {
        if (!IsPointingEnabled || !Input.GetMouseButtonDown(0)) { return; }
        _targetPlayerProperties = TargetPlayerInFrontOfPlayer;
        if (_targetPlayerProperties == null) { return; }
        if (!KarmaGameManager.Instance.SelectedKarmaPlayerMode.ValidPlayerIndicesForVoting.Contains(TargetPlayerInFrontOfPlayer.Index)) { return; }
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

    public void RegisterTargetPickUpPlayPileEventListener(OnLeftClickRayCastAwaitableListener eventListener)
    {
        _onPointingAtPlayerAwaitableListeners.Add(eventListener);
    }

    public async Task TriggerTargetPickUpPlayPile(int targetIndex)
    {
        foreach (OnLeftClickRayCastAwaitableListener listener in _onPointingAtPlayerAwaitableListeners)
        {
            if (listener == null) { throw new NullReferenceException("TargetPickupPlayPileAwaitableListener is null!"); }
            await listener(Index, targetIndex);
        }
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

    public bool IsOwnedBy(int observerPlayerIndex)
    {
        return observerPlayerIndex == Index;
    }

    public PlayerProperties TargetPlayerInFrontOfPlayer
    {
        get
        {
            Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit firstHit, _rayCastCutoff, _layerAsLayerMask);

            if (firstHit.transform == null) { return null; }
            if (firstHit.transform.parent == null) { return null; }
            PlayerProperties playerProperties = firstHit.transform.parent.gameObject.GetComponent<PlayerProperties>();
            return playerProperties;
        }
    }

    public Task AttemptMulliganSwap(IBoard board)
    {
        if (CardSelector.Count != 2) { return Task.CompletedTask; }
        List<SelectableCardObject> selectedCards = CardSelector.CardObjects.ToList();
        SelectableCardObject card1 = selectedCards[0];
        SelectableCardObject card2 = selectedCards[1];

        bool isCard1InHand = CardsInHand.Contains(card1);
        bool isCard2InHand = CardsInHand.Contains(card2);
        if (isCard1InHand && isCard2InHand || (!isCard1InHand && !isCard2InHand)) { return Task.CompletedTask; }

        int handIndex;
        int karmaUpIndex;
        if (isCard1InHand)
        {
            handIndex = CardsInHand.IndexOf(card1);
            karmaUpIndex = CardsInKarmaUp.IndexOf(card2);
        }
        else
        {
            handIndex = CardsInHand.IndexOf(card2);
            karmaUpIndex = CardsInKarmaUp.IndexOf(card1);
        }

        CardSelector.Remove(card1);
        CardSelector.Remove(card2);
        board.Players[Index].SwapHandWithKarmaUp(handIndex, karmaUpIndex);
        return Task.CompletedTask;
    }

    void ColorSelectableCardsAsDefault()
    {
        foreach (SelectableCardObject cardObject in SelectableCardObjects)
        {
            cardObject.ResetCardBorder();
        }
    }
}