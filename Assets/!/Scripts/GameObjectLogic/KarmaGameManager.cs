using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Board.BoardEvents;
using KarmaLogic.Players;
using KarmaLogic.Cards;
using StateMachine.CharacterStateMachines;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataStructures;
using KarmaPlayerMode;
using UnityEngine.EventSystems;

public class KarmaGameManager : MonoBehaviour
{
    private static KarmaGameManager _instance;
    public static KarmaGameManager Instance { get { return _instance; } }

    public static System.Random RNG = new(); // Uniform, system default. Not thread-safe. Great pseudo-random. Volatile in-memory.

    MeshRenderer _cardPrefabRenderer;

    [Header("Prefabs")]
    [SerializeField] GameObject _cardPrefab;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] GameObject _playerKarmaBoardHolderPrefab;

    [Header("Scene objects")]
    [SerializeField] Camera _camera;
    [SerializeField] PhysicsRaycaster _cameraRaycaster;
    [SerializeField] GameObject _currentPlayerArrow;
    [SerializeField] GameObject _playOrderArrow;
    [SerializeField] PlayTableProperties _playTableProperties;

    public Camera CameraMain {  get { return _camera; } }
    public PhysicsRaycaster CameraRaycaster { get { return _cameraRaycaster; } }
    public PlayTableProperties PlayTableHandler { get => _playTableProperties; }

    Vector3 _startingCameraPosition;

    [Header("Gameplay Settings")]
    [SerializeField][Range(0.001f, 30.0f)] float _globalBotDelayInSeconds = 0.1f;

    [SerializeField] KarmaPlayerModeSelector _playerModeSelector;

    public KarmaPlayerMode.KarmaPlayerMode SelectedKarmaPlayerMode { get; private set; }

    public float GlobalBotDelayInSeconds { get => _globalBotDelayInSeconds; set => _globalBotDelayInSeconds = value; }

    public List<PlayerHandler> PlayerHandlers { get; protected set; }
    public IBoard Board { get; protected set; }

    public PickupPlayPile PickUpAction { get; set; } = new ();
    public PlayCardsCombo PlayCardsComboAction { get; set; } = new ();

    public Transform CardTransform { get { return  _cardPrefab.transform; } }

    ArrowHandler _currentPlayerArrowHandler;
    ArrowHandler _playOrderArrowHandler;

    int _turnsSkipped = 0;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        InitialiseHandlers();
        _cardPrefabRenderer = _cardPrefab.GetComponent<MeshRenderer>();
        _startingCameraPosition = _camera.transform.position;
    }

    void InitialiseHandlers()
    {
        _currentPlayerArrowHandler = new ArrowHandler(_currentPlayerArrow);
        _playOrderArrowHandler = new ArrowHandler(_playOrderArrow);
    }

    void RegisterBoardEvents()
    {
        Board.EventSystem.RegisterOnTurnStartEventListener(new BoardEventSystem.BoardEventListener(StartTurn));

        Board.EventSystem.RegisterPlayerDrawEventListener(new BoardEventSystem.PlayerDrawEventListener(DrawCards));
        Board.EventSystem.RegisterHandsFlippedEventListener(new BoardEventSystem.BoardEventListener(SelectedKarmaPlayerMode.FlipHands));
        Board.EventSystem.RegisterHandsRotatedListener(new BoardEventSystem.BoardHandsRotationEventListener(RotateHandsInTurnOrder));
        Board.EventSystem.RegisterStartCardGiveAwayListener(new BoardEventSystem.BoardOnStartCardGiveAwayListener(StartGiveAwayCards));

        Board.EventSystem.RegisterOnBurnEventListener(new BoardEventSystem.BoardBurnEventListener(BurnCards));
        Board.EventSystem.RegisterBurnedCardsReplayedEvent(new BoardEventSystem.BurnedCardsReplayedEventListener(MoveCardsFromBurnPileBottomToPlayPile));

        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(SelectedKarmaPlayerMode.IfWinnerVoteOrEndGame));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(SelectedKarmaPlayerMode.CheckIfGameTurnTimerExceeded));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(SelectedKarmaPlayerMode.NextTurn));
    }

    public void BeginGame()
    {
        SelectedKarmaPlayerMode = _playerModeSelector.Mode();

        Board = SelectedKarmaPlayerMode.Board;
        PlayerHandlers = SelectedKarmaPlayerMode.PlayerHandlers;

        RegisterPlayerBoardListeners();
        RegisterBoardEvents();

        CreatePlayerCardObjectsFromBoard();

        _playTableProperties.CreateCardObjectPilesFromBoard(Board);

        AssignButtonEvents();
        _currentPlayerArrowHandler.SetArrowVisibility(true);
        _playOrderArrowHandler.SetArrowVisibility(true);
        _turnsSkipped = 0;

        FlipAnyNecessaryKarmaDownToUp();
        MoveCurrentPlayerArrow();
        Board.StartTurn();
    }

    [ContextMenu("End Current Game")]
    public async void EndCurrentGame()
    {
        _currentPlayerArrowHandler.SetArrowVisibility(false);
        _playOrderArrowHandler.SetArrowVisibility(false);
        _playTableProperties.DrawPile.DestroyCards();
        _playTableProperties.PlayPile.DestroyCards();
        _playTableProperties.BurnPile.DestroyCards();

        if (_camera == null)
        {
            _camera = PlayerHandlers[Board.CurrentPlayerIndex].Camera;
        }
        _camera.transform.position = _startingCameraPosition;
        if (_cameraRaycaster == null)
        {
            _cameraRaycaster = PlayerHandlers[Board.CurrentPlayerIndex].CameraRaycaster;
        }
        await PlayerHandlers[Board.CurrentPlayerIndex].DisconnectCamera();

        SelectedKarmaPlayerMode.DestroyGame();

        MenuUIManager.Instance.ReturnToStartPage();
    }

    public Quaternion RotationTowardsTableCentre(Vector3 position)
    {
        Vector3 tableDirection = _playTableProperties.TableGeometry.Centre - position;
        tableDirection.y = 0;
        return Quaternion.LookRotation(tableDirection);
    }

    public GameObject InstantiatePlayer(Vector3 position)
    {
        return Instantiate(_playerPrefab, position, RotationTowardsTableCentre(position));
    }

    public PlayerKarmaBoardHolderHandler InstantiatePlayerKarmaBoardHolder(Vector3 position, Quaternion rotation)
    {
        GameObject karmaBoardHolder = Instantiate(_playerKarmaBoardHolderPrefab, position, rotation);
        return karmaBoardHolder.GetComponent<PlayerKarmaBoardHolderHandler>();
    }

    public GameObject InstantiateCard(Card card, Vector3 cardPosition, Quaternion cardRotation, GameObject parent)
    {
        GameObject cardObject = Instantiate(_cardPrefab, cardPosition, cardRotation, parent.transform);
        CardObject cardFrontBackRenderer = cardObject.GetComponent<CardObject>();
        cardFrontBackRenderer.SetCard(card);
        return cardObject;
    }

    void RegisterPlayerBoardListeners()
    {
        for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
        {
            PlayerHandler playerHandlers = PlayerHandlers[playerIndex];
            playerHandlers.RegisterPickedUpCardOnClickEventListener(AttemptGiveAwayPickedUpCard);
            playerHandlers.RegisterTargetPickUpPlayPileEventListener(AttemptGiveAwayPlayPile);
            playerHandlers.SetHandSorter(BoardPlayerHandSorter);
            Board.Players[playerIndex].RegisterOnSwapHandWithPlayableEvent(playerHandlers.SwapHandWithKarmaUp);
            Board.Players[playerIndex].Hand.RegisterHandOrderChangeEvent(playerHandlers.SortHand);
        }
    }

    void CreatePlayerCardObjectsFromBoard()
    {
        for (int i = 0; i < Board.Players.Count; i++)
        {
            PlayerHandler playerHandler = PlayerHandlers[i];
            playerHandler.InstantiatePlayerHandFan(Board.Players[i].Hand);
            if (!SelectedKarmaPlayerMode.IsPlayableCharacter(i)) { PlayerHandlers[i].TurnOffLegalMoveHints(); }
            Player player = Board.Players[i];
            if (i > SelectedKarmaPlayerMode.PlayersBoardHolderHandlers.Count) { throw new Exception("Invalid board setup, number of KarmaUpDownHolders must = number of players!"); }
            if (SelectedKarmaPlayerMode.PlayersBoardHolderHandlers[i] == null) { throw new NullReferenceException("KarmaUpDown for player: " + i + " is null!"); }
            PlayerKarmaBoardHolderHandler boardHolderProperties = SelectedKarmaPlayerMode.PlayersBoardHolderHandlers[i];

            KarmaUpPilesHandler karmaUpPilesHandler = boardHolderProperties.UpPilesHandler;
            KarmaDownPilesHandler karmaDownPilesHandler = boardHolderProperties.DownPilesHandler;
            
            playerHandler.CardsInKarmaUp = new ListWithConstantContainsCheck<SelectableCardObject>(karmaUpPilesHandler.CreateKarmaUpCards(player.KarmaUp, i));
            playerHandler.CardsInKarmaDown = new ListWithConstantContainsCheck<SelectableCardObject>(karmaDownPilesHandler.CreateKarmaDownCards(player.KarmaDown, i));
        }   
    }
    
    public void SetMultiplayer()
    {
        SetPlayerMode(PlayerMode.Multiplayer);
        SetSelectedBoardPreset(0);
    }

    public void SetSelectedBoardPreset(int presetIndex)
    {
        // Used for unit testing. Must be called BEFORE Start()
        _playerModeSelector.SetBoardPresetIndex(presetIndex);
    }

    public void SetPlayerMode(PlayerMode mode)
    {
        // For Play Test compatibility.
        _playerModeSelector.SetPlayerMode(mode);
    }

    public void SetPlayerSubMode(int subMode)
    {
        // For Play Test compatibility.
        _playerModeSelector.SetPlayerSubMode(subMode);
    }

    public void SetIsUsingBoardPresets(bool isUsingPresets)
    {
        // Used for unit testing. Must be called BEFORE Start()
        _playerModeSelector.SetIsUsingBoardPresets(isUsingPresets);
    }

    public void RotateHandsInTurnOrder(int numberOfRotations, IBoard board) 
    {
        if (numberOfRotations == 0) { return; }

        PlayingFrom[] playingFromAtStart = new PlayingFrom[board.Players.Count];

        DeselectAllCards();
        RotateHands(numberOfRotations, board);
        for (int i = 0; i < board.Players.Count; i++)
        {
            PlayerHandlers[i].TryColorLegalCards();
            PlayerHandlers[i].IfChangedToHandSelectionResetPreviousLegalHints(playingFromAtStart[i]);
        }
        return;
    }

    void RotateHands(int numberOfRotations, IBoard board)
    {
        List<ListWithConstantContainsCheck<SelectableCardObject>> beginHands = new();

        for (int i = 0; i < board.Players.Count; i++)
        {
            PlayerHandler playerHandler = PlayerHandlers[i];
            ListWithConstantContainsCheck<SelectableCardObject> hand = playerHandler.CardsInHand;
            beginHands.Add(hand);
        }

        Deque<ListWithConstantContainsCheck<SelectableCardObject>> hands = new (beginHands);

        hands.Rotate(numberOfRotations);
        
        for (int i = 0; i < board.Players.Count; i++)
        {
            ListWithConstantContainsCheck<SelectableCardObject> hand = hands[i];
            PlayerHandler playerHandler = PlayerHandlers[i];
            foreach (SelectableCardObject cardObject in hand)
            {
                playerHandler.ParentCardToThis(cardObject);
            }
            PlayerHandlers[i].UpdateHand(hand);
        }
    }

    public void BurnCards(int jokerCount)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlayBurnSFX();

        if (jokerCount == 0)
        {
            _playTableProperties.MoveEntirePlayPileToBurnPile();
            return;
        }

        _playTableProperties.MoveTopCardsFromPlayPileToBurnPile(jokerCount);
    }

    public Bounds CardBounds
    {
        get
        {
            return _cardPrefabRenderer.bounds;
        }
    }

    void DeselectAllCards()
    {
        foreach (PlayerHandler playerHandler in PlayerHandlers)
        {
            playerHandler.CardSelector.Clear();
        }
    }

    void RotatePlayOrderArrow()
    {
        if (Board.PlayOrder == BoardPlayOrder.UP)
        {
            _playOrderArrowHandler.PointUp();
        }
        else
        {
            _playOrderArrowHandler.PointDown();
        }
    }

    void MoveCurrentPlayerArrow()
    {
        PlayerHandler playerHandler = PlayerHandlers[Board.CurrentPlayerIndex];

        Vector3 playerPosition = playerHandler.transform.position;

        _currentPlayerArrowHandler.MoveArrow(playerPosition, _playTableProperties.TableGeometry.Centre);
    }

    void FlipAnyNecessaryKarmaDownToUp()
    {
        int i = 0;
        foreach (PlayerHandler playerHandler in PlayerHandlers)
        {
            if (playerHandler.SelectingFrom == PlayingFrom.KarmaDown)
            {
                playerHandler.FlipKarmaDownCardsUp();
            }
            i++;
        }
    }

    void MoveCardObjectsFromSelectionToPlayPile(int playerIndex)
    {
        List<SelectableCardObject> cardObjects = PlayerHandlers[playerIndex].PopSelectedCardsFromSelection();
        PlayTableHandler.PlayPile.MoveCardsToTop(cardObjects);
    }

    void MoveCardsFromBurnPileBottomToPlayPile(int numberOfCards)
    {
        List<SelectableCardObject> cardObjects = PlayTableHandler.BurnPile.PopCardsFromBottom(numberOfCards);
        PlayTableHandler.PlayPile.MoveCardsToTop(cardObjects);
    }

    void DrawCards(int numberOfCards, int playerIndex)
    {
        List<SelectableCardObject> cardsDrawn = _playTableProperties.DrawCards(numberOfCards);
        AudioManager.Instance.PlayCardPickup();
        PlayerHandlers[playerIndex].AddCardObjectsToHand(cardsDrawn);
    }

    async void StartGiveAwayCards(int numberOfCards, int playerIndex)
    {
        PlayerHandler playerHandler = PlayerHandlers[playerIndex];
        // Each giveaway is a separate CardGiveAwayHandler, which also automatically removes its listeners on completion, so no memory leaks.

        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnCardGiveAwayListener(new CardGiveAwayHandler.OnCardGiveAwayListener(GiveAwayCard));
        print("GIVE AWAY STARTED");
        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnFinishCardGiveAwayListener(PrintGiveAwayEnded);
        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnFinishCardGiveAwayListener(Board.EndTurn);
        await playerHandler.ProcessStateCommand(Command.CardGiveAwayComboPlayed);
    }

    void PrintGiveAwayEnded()
    {
        print("GIVE AWAY ENDED!!!");
    }

    void GiveAwayCard(Card card, int giverIndex, int receiverIndex)
    {
        PlayerHandlers[receiverIndex].ReceivePickedUpCard(PlayerHandlers[giverIndex]);
        print("Player received card!");
    }

    async void StartTurn(IBoard board)
    {
        if (SelectedKarmaPlayerMode.IsGameOver) { return; }

        RotatePlayOrderArrow();
        if (PlayerHandlers[board.CurrentPlayerIndex].StateMachine.CurrentState is not State.Mulligan) { MoveCurrentPlayerArrow(); }
        
        Board.Print();

        bool alreadyVoting = SelectedKarmaPlayerMode.ValidPlayerIndicesForVoting.Count > 0;
        bool gameIsWon = SelectedKarmaPlayerMode.IsGameWonWithoutVoting || SelectedKarmaPlayerMode.IsGameWonWithVoting;

        if (!alreadyVoting && gameIsWon) { SelectedKarmaPlayerMode.IfWinnerVoteOrEndGame(Board); return; }

        if (PlayerHandlers[board.CurrentPlayerIndex].StateMachine.CurrentState is State.PotentialWinner)
        {
            Board.EndTurn();
            return;
        }

        if (!Board.CurrentPlayer.HasCards) { await PlayerHandlers[board.CurrentPlayerIndex].ProcessStateCommand(Command.HasNoCards); return; }

        if (SelectedKarmaPlayerMode.IsPlayableCharacter(board.CurrentPlayerIndex)) { SelectedKarmaPlayerMode.IfPlayableEnableCurrentPlayerMovement(); }
        if (SelectedKarmaPlayerMode.PlayerJokerCounts.Keys.Contains(Board.CurrentPlayerIndex))
        {
            PlayerHandlers[board.CurrentPlayerIndex].RegisterVoteForTargetEventListener(SelectedKarmaPlayerMode.TriggerVoteForPlayer);
            await PlayerHandlers[board.CurrentPlayerIndex].ProcessStateCommand(Command.VotingStarted);
            return;
        }

        if (Board.CurrentLegalActions.Count > 0)
        {
            _turnsSkipped = 0;
            await PlayerHandlers[Board.CurrentPlayerIndex].ProcessStateCommand(Command.TurnStarted);
            return;
        }

        _turnsSkipped += 1;
        if (_turnsSkipped == Board.Players.Count)
        {
            SelectedKarmaPlayerMode.EndGameEarlyDueToNoLegalActions();
            return;
        }

        Board.EndTurn();
    }

    void AssignButtonEvents()
    {
        for (int i = 0; i < PlayerHandlers.Count; i++)
        {
            int index = i;
            async Task triggerCardSelectionConfirmed() => await TriggerCardSelectionConfirmed(index);
            async Task triggerClearSelection() => await TriggerClearSelection(index);
            async Task triggerPickupActionSelected() => await TriggerPickupActionSelected(index);
            async Task triggerFinishMulligan() => await TriggerFinishMulligan(index);
            PlayerHandlers[i].ConfirmSelectionButton.onClick.AddListener(triggerCardSelectionConfirmed);
            PlayerHandlers[i].ClearSelectionButton.onClick.AddListener(triggerClearSelection);
            PlayerHandlers[i].PickupPlayPileButton.onClick.AddListener(triggerPickupActionSelected);
            PlayerHandlers[i].FinishMulliganButton.onClick.AddListener(triggerFinishMulligan);
        }
    }

    async Task TriggerCardSelectionConfirmed(int playerIndex)
    {
        PlayerHandler playerHandler = PlayerHandlers[playerIndex];
        if (playerHandler.StateMachine.CurrentState is State.PickingAction) 
        {
            if (playerHandler.CardSelector.CardObjects.Count == 0) { return; }
            await AttemptToPlayCardSelection(playerIndex); 
            return; 
        }
        if (playerHandler.StateMachine.CurrentState is State.SelectingCardGiveAwayIndex) 
        { 
            if (playerHandler.CardSelector.CardObjects.Count == 0) { return; }
            await AttemptToSelectCardForGiveAway(playerIndex); 
            return; 
        }
        if (playerHandler.StateMachine.CurrentState is State.Mulligan)
        {
            await playerHandler.AttemptMulliganSwap(Board);
            return;
        }
        print("Bot is in state: " + PlayerHandlers[playerIndex].StateMachine.CurrentState);
        throw new NotImplementedException();
    }

    async Task TriggerClearSelection(int playerIndex)
    {
        State state = PlayerHandlers[playerIndex].StateMachine.CurrentState;
        
        if (state is State.Mulligan || state is State.PickingAction || state is State.SelectingCardGiveAwayIndex) 
        { 
            await AttemptClearCardSelection(playerIndex); 
            return; 
        }
        throw new NotImplementedException();
    }

    Task TriggerPickupActionSelected(int playerIndex)
    {
        PlayerHandler playerHandler = PlayerHandlers[playerIndex];
        PickUpAction.Apply(Board, playerHandler.CardSelector.Selection);
        List<SelectableCardObject> playPileCards = _playTableProperties.PopAllFromPlayPile();
        PlayerHandlers[playerIndex].AddCardObjectsToHand(playPileCards);
        Board.EndTurn();
        return Task.CompletedTask;
    }

    async Task TriggerFinishMulligan(int playerIndex)
    {
        await SelectedKarmaPlayerMode.TriggerFinishMulligan(playerIndex);
    }

    Task AttemptClearCardSelection(int playerIndex)
    {
        PlayerHandler playerHandler = PlayerHandlers[playerIndex];
        CardSelector cardSelector = playerHandler.CardSelector;
        cardSelector.Clear();
        playerHandler.TryColorLegalCards();
        return Task.CompletedTask;
    }

    Task AttemptToPlayCardSelection(int playerIndex)
    {
        PlayerHandler playerHandler = PlayerHandlers[playerIndex];
        CardSelector cardSelector = playerHandler.CardSelector;
        print("Attempting to play cards for player: " + playerIndex + " " + cardSelector.Selection + " values: " + cardSelector.SelectionCardValues);
        string currentLegalCombos = "Currently legal on top of: ";

        Card topCard = Board.PlayPile.VisibleTopCard;
        if (topCard is not null)
        {
            currentLegalCombos += Board.PlayPile.VisibleTopCard + " ";
        }
        else
        {
            currentLegalCombos += "Nothing! ";
        }

        foreach (FrozenMultiSet<CardValue> combo in Board.CurrentLegalCombos)
        {
            currentLegalCombos += combo + ", ";
        }
        print(currentLegalCombos + "]");

        if (Board.CurrentLegalCombos.Contains(cardSelector.SelectionCardValues))
        {
            print("Card combo is LEGAL!! Proceeding to play " + cardSelector.SelectionCardValues);

            AudioManager.Instance.PlayCardAddedToPileSFX();

            CardsList cardSelection = playerHandler.CardSelector.Selection;
            MoveCardObjectsFromSelectionToPlayPile(playerIndex);
            
            PlayCardsComboAction.RegisterOnFinishListener(playerHandler.TryColorLegalCards);
            PlayCardsComboAction.RegisterOnFinishListener(Board.EndTurn);
            PlayCardsComboAction.Apply(Board, cardSelection);
        }

        return Task.CompletedTask;
    }

    async Task AttemptToSelectCardForGiveAway(int playerIndex)
    {
        if (PlayerHandlers[playerIndex].CardSelector.Count > 1) { return; }
        print("Attempting to give away card selection: " + PlayerHandlers[playerIndex].CardSelector.CardObjects.First());
        Player player = Board.Players[playerIndex];
        HashSet<int> jokerIndices = new();
        for (int i = 0; i < player.PlayableCards.Count; i++)
        {
            Card card = player.PlayableCards[i];
            if (card.Value == CardValue.JOKER) { jokerIndices.Add(i); }
        }
        
        HashSet<CardValue> validCardValues = player.PlayableCards.CardValues.ToHashSet();
        validCardValues.Remove(CardValue.JOKER);

        if (validCardValues.Count == 0) { return; }

        PlayerHandler playerHandler = PlayerHandlers[playerIndex];
        HashSet<SelectableCardObject> selectedCards = playerHandler.CardSelector.CardObjects;

        if (selectedCards.Count != 1) { return; }

        SelectableCardObject selectedCard = selectedCards.First();
        if (!validCardValues.Contains(selectedCard.CurrentCard.Value)) { return; }

        playerHandler.PickedUpCard = selectedCard;
        playerHandler.CardSelector.Remove(selectedCard);
        playerHandler.SelectableCardObjects.Remove(selectedCard);
        playerHandler.PickedUpCard.ResetCardBorder();
        playerHandler.PickedUpCard.DisableSelectShader();

        await playerHandler.ProcessStateCommand(Command.CardGiveAwayIndexSelected);
    }

    async void AttemptGiveAwayPickedUpCard(int giverIndex, int targetIndex)
    {
        HashSet<int> excludedTargetIndices = Board.PotentialWinnerIndices;
        excludedTargetIndices.Add(Board.CurrentPlayerIndex);
        if (excludedTargetIndices.Count == Board.Players.Count) { throw new Exception("An error occurred, there is NO valid card giveaway player target"); }
        if (excludedTargetIndices.Contains(targetIndex)) { print("The target player: " + targetIndex + " is invalid as they are either YOU or they have no cards"); return; }

        Player giver = Board.Players[giverIndex];
        print("Giving away picked up card: " + PlayerHandlers[giverIndex].PickedUpCard + " to player " + targetIndex);
        giver.CardGiveAwayHandler.GiveAway(PlayerHandlers[giverIndex].PickedUpCard.CurrentCard, targetIndex);
        Board.DrawUntilFull(giverIndex);

        if (giver.CardGiveAwayHandler.IsFinished)
        {
            giver.CardGiveAwayHandler = null;
            await PlayerHandlers[giverIndex].StateMachine.MoveNext(Command.TurnEnded);
            return;
        }

        await PlayerHandlers[giverIndex].StateMachine.MoveNext(Command.CardGiveAwayUnfinished);
    }

    async Task AttemptGiveAwayPlayPile(int giverIndex, int targetIndex)
    {
        if (targetIndex == giverIndex) { return; }
        Player giver = Board.Players[giverIndex];
        giver.PlayPileGiveAwayHandler.GiveAway(targetIndex);
        print(giverIndex + " is forcing pickup of PLAY PILE (board) to player: " + targetIndex);
        TargetPickupPlayPile(giverIndex, targetIndex);

        List<SelectableCardObject> playPileCardObjects = _playTableProperties.PopAllFromPlayPile();

        foreach (SelectableCardObject cardObject in playPileCardObjects)
        {
            print("Player: " + targetIndex + " is receiving play pile card: " + cardObject);
        }

        PlayerHandlers[targetIndex].AddCardObjectsToHand(playPileCardObjects);

        giver.PlayPileGiveAwayHandler = null;

        await PlayerHandlers[targetIndex].StateMachine.MoveNext(Command.GotJokered);
        await PlayerHandlers[giverIndex].StateMachine.MoveNext(Command.TurnEnded);
        Board.EndTurn();
        return;
    }

    void TargetPickupPlayPile(int giverIndex, int targetIndex)
    {
        if (targetIndex == giverIndex) { return; }
        Board.Players[targetIndex].Pickup(Board.PlayPile);
    }

    Dictionary<Card, List<int>> BoardPlayerHandSorter(int playerIndex)
    {
        Dictionary<Card, List<int>> cardPositions = new();
        int n = Board.Players[playerIndex].Hand.Count;
        for (int i = 0; i < n; i++)
        {
            Card card = Board.Players[playerIndex].Hand[i];
            if (!cardPositions.ContainsKey(card)) { cardPositions[card] = new List<int>(); }
            cardPositions[card].Add(i);
        }

        return cardPositions;
    }
}
