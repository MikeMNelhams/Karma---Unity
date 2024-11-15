using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.GameExceptions;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Board.BoardEvents;
using KarmaLogic.Players;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using StateMachines.CharacterStateMachines;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataStructures;
using KarmaPlayerMode;

public class KarmaGameManager : MonoBehaviour
{
    private static KarmaGameManager _instance;
    public static KarmaGameManager Instance { get { return _instance; } }

    public GameObject _cardPrefab;
    public GameObject _playerPrefab;
    MeshRenderer _cardPrefabRenderer;

    [SerializeField] GameObject _currentPlayerArrow;
    [SerializeField] GameObject _playOrderArrow;
    [SerializeField] PlayTableProperties _playTable;

    [Header("Gameplay Settings")]

    [SerializeField] int _whichPlayerStarts = 0;

    [SerializeField] KarmaPlayerModeSelector _playerModeSelector;
    public KarmaPlayerMode.KarmaPlayerMode SelectedKarmaPlayerMode { get; private set; }

    public List<PlayerProperties> PlayersProperties { get; protected set; }

    public IBoard Board { get; protected set; }
    public PickupPlayPile PickUpAction { get; set; } = new ();
    public PlayCardsCombo PlayCardsComboAction { get; set; } = new ();

    public Transform CardTransform { get { return  _cardPrefab.transform; } }

    ArrowHandler _currentPlayerArrowHandler;
    ArrowHandler _playOrderArrowHandler;

    public static System.Random RNG = new(); // Uniform, system default. Not thread-safe. Great pseudo-random. Volatile in-memory.

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
    }

    void InitialiseHandlers()
    {
        _currentPlayerArrowHandler = new ArrowHandler(_currentPlayerArrow);
        _playOrderArrowHandler = new ArrowHandler(_playOrderArrow);
    }

    void Start()
    {
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 12, 13, 14, 15}, new() { 2, 2, 2 }, new() { 3, 3, 3 } },
            new() { new() { 2, 5, 12, 12 }, new() { 3, 3, 3}, new() { } },
            new() { new() { 2, 4, 5, 12, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
            new() { new() { 2, 4, 5, 12, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
        };

        List<int> drawCardValues = new() { 10, 11, 12};
        List<int> playCardValues = new() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 7 };
        List<int> burnCardValues = new() { };

        // Board = BoardTestFactory.BotJokerCombo();
        //Board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, whoStarts: _whichPlayerStarts);
        //Board = BoardFactory.RandomStart(numberOfPlayers, numberOfJokers: 1, whoStarts: _whichPlayerStarts);

        SelectedKarmaPlayerMode = _playerModeSelector.Mode();

        Board = SelectedKarmaPlayerMode.Board;
        PlayersProperties = SelectedKarmaPlayerMode.PlayersProperties;

        RegisterPlayerBoardListeners(SelectedKarmaPlayerMode.PlayersStartInfo);
        RegisterBoardEvents();
        CreatePlayerCardObjectsFromBoard();
        
        _playTable.CreateCardObjectPilesFromBoard(Board);

        AssignButtonEvents();
        _currentPlayerArrowHandler.SetArrowVisibility(true);
        _playOrderArrowHandler.SetArrowVisibility(true);
        
        Board.StartTurn();
    }

    void RegisterBoardEvents()
    {
        Board.EventSystem.RegisterOnTurnStartEventListener(new BoardEventSystem.BoardEventListener(StartTurn));

        Board.EventSystem.RegisterPlayerDrawEventListener(new BoardEventSystem.PlayerDrawEventListener(DrawCards));
        Board.EventSystem.RegisterHandsFlippedEventListener(new BoardEventSystem.BoardEventListener(FlipHands));
        Board.EventSystem.RegisterHandsRotatedListener(new BoardEventSystem.BoardHandsRotationEventListener(RotateHandsInTurnOrder));
        Board.EventSystem.RegisterStartCardGiveAwayListener(new BoardEventSystem.BoardOnStartCardGiveAwayListener(StartGiveAwayCards));

        Board.EventSystem.RegisterOnBurnEventListener(new BoardEventSystem.BoardBurnEventListener(BurnCards));

        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(IfWinnerVoteOrEndGame));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckPotentialWinner));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckIfGameTurnTimerExceeded));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(NextTurn));
    }

    public GameObject InstantiatePlayer(List<KarmaPlayerStartInfo> playersStartInfo, int playerIndex)
    {
        Vector3 tableDirection = _playTable.centre - playersStartInfo[playerIndex].startPosition;
        tableDirection.y = 0;
        return Instantiate(_playerPrefab, playersStartInfo[playerIndex].startPosition, Quaternion.LookRotation(tableDirection));
    }

    void RegisterPlayerBoardListeners(List<KarmaPlayerStartInfo> playersStartInfo)
    {
        for (int playerIndex = 0; playerIndex < playersStartInfo.Count; playerIndex++)
        {
            PlayerProperties playerProperties = PlayersProperties[playerIndex];
            playerProperties.RegisterPickedUpCardOnClickEventListener(AttemptGiveAwayPickedUpCard);
            playerProperties.RegisterTargetPickUpPlayPileEventListener(AttemptGiveAwayPlayPile);
            playerProperties.SetHandSorter(BoardPlayerHandSorter);
            Board.Players[playerIndex].RegisterOnSwapHandWithPlayableEvent(playerProperties.SwapHandWithKarmaUp);
            Board.Players[playerIndex].Hand.RegisterHandOrderChangeEvent(playerProperties.SortHand);
        }
    }

    void CreatePlayerCardObjectsFromBoard()
    {
        for (int i = 0; i < Board.Players.Count; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            playerProperties.InstantiatePlayerHandFan(Board.Players[i].Hand);
            if (!SelectedKarmaPlayerMode.PlayersStartInfo[i].isPlayableCharacter ) { PlayersProperties[i].TurnOffLegalMoveHints(); }
            Player player = Board.Players[i];
            if (i >= _playTable.boardHolders.Count) { break; }
            if (_playTable.boardHolders[i] == null) { continue; }
            GameObject boardHolder = _playTable.boardHolders[i];

            KarmaUpPilesHandler karmaUpPilesHandler = boardHolder.GetComponent<KarmaUpPilesHandler>();
            KarmaDownPilesHandler karmaDownPilesHandler = boardHolder.GetComponent<KarmaDownPilesHandler>();
            
            playerProperties.CardsInKarmaUp = new ListWithConstantContainsCheck<SelectableCard>(karmaUpPilesHandler.CreateKarmaUpCards(player.KarmaUp, i));
            playerProperties.CardsInKarmaDown = new ListWithConstantContainsCheck<SelectableCard>(karmaDownPilesHandler.CreateKarmaDownCards(player.KarmaDown, i));
        }   
    }

    public GameObject InstantiateCard(Card card, Vector3 cardPosition, Quaternion cardRotation, GameObject parent)
    {
        GameObject cardObject = Instantiate(_cardPrefab, cardPosition, cardRotation, parent.transform);
        CardObject cardFrontBackRenderer = cardObject.GetComponent<CardObject>();
        cardFrontBackRenderer.SetCard(card);
        return cardObject;
    }

    public void IfWinnerVoteOrEndGame(IBoard board)
    {
        SelectedKarmaPlayerMode.IfWinnerVoteOrEndGame();
    }

    public void CheckIfGameTurnTimerExceeded(IBoard board)
    {
        SelectedKarmaPlayerMode.CheckIfGameTurnTimerExceeded();
    }

    public void FlipHands(IBoard board)
    {
        for (int i = 0; i < board.Players.Count; i++)
        {
            PlayersProperties[i].FlipHand();
        }
    }

    public void RotateHandsInTurnOrder(int numberOfRotations, IBoard board) 
    {
        int k = numberOfRotations % board.Players.Count;
        if (k == 0) { return; }
        DeselectAllCards();
        RotateHands(k * ((int) board.TurnOrder), board);
        return;
    }

    void RotateHands(int numberOfRotations, IBoard board)
    {
        List<ListWithConstantContainsCheck<SelectableCard>> beginHands = new();

        for (int i = 0; i < board.Players.Count; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            ListWithConstantContainsCheck<SelectableCard> hand = playerProperties.CardsInHand;
            beginHands.Add(hand);
        }

        Deque<ListWithConstantContainsCheck<SelectableCard>> hands = new (beginHands);

        hands.Rotate(numberOfRotations);
        
        for (int i = 0; i < board.Players.Count; i++)
        {
            ListWithConstantContainsCheck<SelectableCard> hand = hands[i];
            PlayerProperties playerProperties = PlayersProperties[i];
            foreach (SelectableCard cardObject in hand)
            {
                playerProperties.ParentCardToThis(cardObject);
            }
            PlayersProperties[i].UpdateHand(hand);
        }
    }

    void DeselectAllCards()
    {
        foreach (PlayerProperties playerProperties in PlayersProperties)
        {
            playerProperties.CardSelector.Clear();
        }
    }

    public void BurnCards(int jokerCount)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlayBurnSFX();

        if (jokerCount == 0)
        {
            _playTable.MoveEntirePlayPileToBurnPile();
            return;
        }
        
        _playTable.MoveTopCardsFromPlayPileToBurnPile(jokerCount);
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
        PlayerProperties playerProperties = PlayersProperties[Board.CurrentPlayerIndex];

        Vector3 playerPosition = playerProperties.transform.position;
        Vector3 tablePosition = _playTable.transform.position;

        _currentPlayerArrowHandler.MoveArrow(playerPosition, tablePosition);
    }

    void MoveCardsFromSelectionToPlayPile(int playerIndex)
    {
        List<SelectableCard> cardObjects = PlayersProperties[playerIndex].PopSelectedCardsFromSelection();
        _playTable.MoveCardsToTopOfPlayPile(cardObjects);
    }

    void DrawCards(int numberOfCards, int playerIndex)
    {
        List<SelectableCard> cardsDrawn = _playTable.DrawCards(numberOfCards);
        PlayersProperties[playerIndex].AddCardObjectsToHand(cardsDrawn);
    }

    async void StartGiveAwayCards(int numberOfCards, int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        // Each giveaway is a separate CardGiveAwayHandler, which also automatically removes its listeners on completion, so no memory leaks.

        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnCardGiveAwayListener(new CardGiveAwayHandler.OnCardGiveAwayListener(GiveAwayCard));
        print("GIVE AWAY STARTED");
        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnFinishCardGiveAwayListener(PrintGiveAwayEnded);
        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnFinishCardGiveAwayListener(Board.EndTurn);
        await playerProperties.ProcessStateCommand(Command.CardGiveAwayComboPlayed);
    }

    void PrintGiveAwayEnded()
    {
        print("GIVE AWAY ENDED!!!");
    }

    void GiveAwayCard(Card card, int giverIndex, int receiverIndex)
    {
        PlayersProperties[receiverIndex].ReceivePickedUpCard(PlayersProperties[giverIndex]);
        print("Player received card!");
    }

    async void StartTurn(IBoard board)
    {
        RotatePlayOrderArrow();
        if (PlayersProperties[board.CurrentPlayerIndex].StateMachine.CurrentState is not State.Mulligan) { MoveCurrentPlayerArrow(); }
        
        Board.Print();

        bool alreadyVoting = SelectedKarmaPlayerMode.ValidPlayerIndicesForVoting.Count > 0;
        bool gameIsWon = SelectedKarmaPlayerMode.IsGameWonWithoutVoting || SelectedKarmaPlayerMode.IsGameWonWithVoting;

        if (!alreadyVoting && gameIsWon) { SelectedKarmaPlayerMode.IfWinnerVoteOrEndGame(); return; }

        if (PlayersProperties[board.CurrentPlayerIndex].StateMachine.CurrentState is State.PotentialWinner)
        {
            Board.EndTurn();
            return;
        }

        if (!Board.CurrentPlayer.HasCards) { await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.HasNoCards); }

        if (SelectedKarmaPlayerMode.PlayersStartInfo[board.CurrentPlayerIndex].isPlayableCharacter) { SelectedKarmaPlayerMode.IfPlayableEnableCurrentPlayerMovement(); }
        if (SelectedKarmaPlayerMode.PlayerJokerCounts.Keys.Contains(Board.CurrentPlayerIndex))
        {
            PlayersProperties[board.CurrentPlayerIndex].RegisterVoteForTargetEventListener(SelectedKarmaPlayerMode.TriggerVoteForPlayer);
            await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.VotingStarted);
            return;
        }
        await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.TurnStarted);
    }

    async void NextTurn(IBoard board)
    {
        // WARNING: Using PlayerProperties.StateMachine.CurrentState is unstable, since this is async VOID!
        PlayerProperties activePlayer = PlayersProperties[Board.PlayerIndexWhoStartedTurn];
        State activePlayerState = activePlayer.StateMachine.CurrentState;
        print("While end of turn CURRENT PLAYER STATE: " + activePlayer.StateMachine.CurrentState);

        // They can != only by PP: K, K, K BP: 9, You play K -> Q. It's incredibly RARE, but requires this reset. 
        Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;

        if (activePlayerState is State.PotentialWinner || activePlayerState is State.GameOver)
        {
            StepToNextPlayer();
            return;
        }

        if (Board.CurrentPlayer.PlayPileGiveAwayHandler != null && !Board.CurrentPlayer.PlayPileGiveAwayHandler.IsFinished)
        {
            print("The good ending!");
            await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.PlayPileGiveAwayComboPlayed);
            return;
        }

        if (Board.CurrentPlayer.PlayPileGiveAwayHandler != null && Board.CurrentPlayer.PlayPileGiveAwayHandler.IsFinished)
        {
            await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.TurnStarted);
            PlayTurnAgain();
            return;
        }

        if (board.HasBurnedThisTurn && board.Players[board.CurrentPlayerIndex].HasCards)
        {
            print("BURN BABY BURN!");
            PlayTurnAgain();
            return;
        }

        if (Board.CurrentPlayer.CardGiveAwayHandler != null && !Board.CurrentPlayer.CardGiveAwayHandler.IsFinished)
        {
            return;
        }

        if (activePlayerState is not State.WaitingForTurn)
        {
            await activePlayer.ProcessStateCommand(Command.TurnEnded);
        }

        StepToNextPlayer();
        return;

        throw new NotImplementedException("Impossible game state: player is in unknown state at the end of the turn: " + activePlayerState);
    }

    void StepToNextPlayer()
    {
        if (SelectedKarmaPlayerMode.PlayersStartInfo[Board.CurrentPlayerIndex].isPlayableCharacter) { SelectedKarmaPlayerMode.IfPlayableDisableStartingPlayerMovement(); }
        Board.StepPlayerIndex(1);
        print("Starting Turn. Current active player: " + Board.CurrentPlayerIndex);
        Board.StartTurn();
    }

    void PlayTurnAgain()
    {
        Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;
        Board.StartTurn();
    }

    void AssignButtonEvents()
    {
        for (int i = 0; i < PlayersProperties.Count; i++)
        {
            int index = i;
            async Task triggerCardSelectionConfirmed() => await TriggerCardSelectionConfirmed(index);
            async Task triggerClearSelection() => await TriggerClearSelection(index);
            async Task triggerPickupActionSelected() => await TriggerPickupActionSelected(index);
            async Task triggerFinishMulligan() => await TriggerFinishMulligan(index);
            PlayersProperties[i].ConfirmSelectionButton.onClick.AddListener(triggerCardSelectionConfirmed);
            PlayersProperties[i].ClearSelectionButton.onClick.AddListener(triggerClearSelection);
            PlayersProperties[i].PickupPlayPileButton.onClick.AddListener(triggerPickupActionSelected);
            PlayersProperties[i].FinishMulliganButton.onClick.AddListener(triggerFinishMulligan);
        }
    }

    async Task TriggerCardSelectionConfirmed(int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        if (playerProperties.StateMachine.CurrentState is State.PickingAction) 
        {
            if (playerProperties.CardSelector.CardObjects.Count == 0) { return; }
            await AttemptToPlayCardSelection(playerIndex); 
            return; 
        }
        if (playerProperties.StateMachine.CurrentState is State.SelectingCardGiveAwayIndex) 
        { 
            if (playerProperties.CardSelector.CardObjects.Count == 0) { return; }
            await AttemptToSelectCardForGiveAway(playerIndex); 
            return; 
        }
        if (playerProperties.StateMachine.CurrentState is State.Mulligan)
        {
            await playerProperties.AttemptMulliganSwap(Board);
            return;
        }
        print("Bot is in state: " + PlayersProperties[playerIndex].StateMachine.CurrentState);
        throw new NotImplementedException();
    }

    async Task TriggerClearSelection(int playerIndex)
    {
        State state = PlayersProperties[playerIndex].StateMachine.CurrentState;
        if (state is State.PickingAction || state is State.SelectingCardGiveAwayIndex) { await AttemptClearCardSelection(playerIndex); return; }
        throw new NotImplementedException();
    }

    Task TriggerPickupActionSelected(int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        PickUpAction.Apply(Board, playerProperties.CardSelector.Selection);
        List<SelectableCard> playPileCards = _playTable.PopAllFromPlayPile();
        PlayersProperties[playerIndex].AddCardObjectsToHand(playPileCards);
        Board.EndTurn();
        return Task.CompletedTask;
    }

    async Task TriggerFinishMulligan(int playerIndex)
    {
        await SelectedKarmaPlayerMode.TriggerFinishMulligan(playerIndex);
    }

    Task AttemptClearCardSelection(int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        CardSelector cardSelector = playerProperties.CardSelector;
        cardSelector.Clear();
        playerProperties.TryColorLegalCards();
        return Task.CompletedTask;
    }

    Task AttemptToPlayCardSelection(int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        CardSelector cardSelector = playerProperties.CardSelector;
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

            CardsList cardSelection = playerProperties.CardSelector.Selection;
            MoveCardsFromSelectionToPlayPile(playerIndex);
            
            PlayCardsComboAction.RegisterOnFinishListener(playerProperties.TryColorLegalCards);
            PlayCardsComboAction.RegisterOnFinishListener(Board.EndTurn);
            PlayCardsComboAction.Apply(Board, cardSelection);
        }

        return Task.CompletedTask;
    }

    async void CheckPotentialWinner(IBoard board)
    {
        if (board.CurrentPlayer.HasCards) { return; }

        await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.HasNoCards);
    }

    async Task AttemptToSelectCardForGiveAway(int playerIndex)
    {
        print("Attempting to give away card selection: " + PlayersProperties[playerIndex].CardSelector.CardObjects.First());
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

        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        HashSet<SelectableCard> selectedCards = playerProperties.CardSelector.CardObjects;

        if (selectedCards.Count != 1) { return; }

        SelectableCard selectedCard = selectedCards.First();
        if (!validCardValues.Contains(selectedCard.CurrentCard.Value)) { return; }

        playerProperties.PickedUpCard = selectedCard;
        playerProperties.CardSelector.Remove(selectedCard);
        playerProperties.SelectableCardObjects.Remove(selectedCard);
        playerProperties.PickedUpCard.ResetCardBorder();
        playerProperties.PickedUpCard.DisableSelectShader();

        await playerProperties.StateMachine.MoveNext(Command.CardGiveAwayIndexSelected);
    }

    async void AttemptGiveAwayPickedUpCard(int giverIndex, int targetIndex)
    {
        HashSet<int> excludedTargetIndices = Board.PotentialWinnerIndices;
        excludedTargetIndices.Add(Board.CurrentPlayerIndex);
        if (excludedTargetIndices.Count == Board.Players.Count) { throw new Exception("An error occurred, there is NO valid card giveaway player target"); }
        if (excludedTargetIndices.Contains(targetIndex)) { print("The target player: " + targetIndex + " is invalid as they are either YOU or they have no cards"); return; }

        Player giver = Board.Players[giverIndex];
        print("Giving away picked up card: " + PlayersProperties[giverIndex].PickedUpCard + " to player " + targetIndex);
        giver.CardGiveAwayHandler.GiveAway(PlayersProperties[giverIndex].PickedUpCard.CurrentCard, targetIndex);
        Board.DrawUntilFull(giverIndex);

        if (giver.CardGiveAwayHandler.IsFinished)
        {
            giver.CardGiveAwayHandler = null;
            await PlayersProperties[giverIndex].StateMachine.MoveNext(Command.TurnEnded);
            return;
        }

        await PlayersProperties[giverIndex].StateMachine.MoveNext(Command.CardGiveAwayUnfinished);
    }

    async Task AttemptGiveAwayPlayPile(int giverIndex, int targetIndex)
    {
        if (targetIndex == giverIndex) { return; }
        Player giver = Board.Players[giverIndex];
        giver.PlayPileGiveAwayHandler.GiveAway(targetIndex);
        print(giverIndex + " is giving away PLAY PILE (board) to player: " + targetIndex);
        Board.Players[targetIndex].Pickup(Board.PlayPile); // TODO Register this at the beginning!

        PlayersProperties[targetIndex].AddCardObjectsToHand(_playTable.PopAllFromPlayPile());

        giver.PlayPileGiveAwayHandler = null;

        await PlayersProperties[targetIndex].StateMachine.MoveNext(Command.GotJokered);
        await PlayersProperties[giverIndex].StateMachine.MoveNext(Command.TurnEnded);
        Board.EndTurn();
        return;
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

    public void ColorLegalCard(SelectableCard cardObject, CardSelector cardSelector)
    {
        LegalCombos legalCombos = Board.CurrentLegalCombos;

        FrozenMultiSet<CardValue> combinedSelection = new();
        FrozenMultiSet<CardValue> selectionCardValues = cardSelector.SelectionCardValues;

        foreach (CardValue cardValue in selectionCardValues)
        {
            combinedSelection.Add(cardValue, selectionCardValues[cardValue]);
        }

        if (!cardSelector.CardObjects.Contains(cardObject))
        {
            combinedSelection.Add(cardObject.CurrentCard.Value);
        }

        if (legalCombos.IsLegal(combinedSelection))
        {
            cardObject.ColorCardBorder(Color.green);
            return;
        }

        if (legalCombos.IsSubsetLegal(combinedSelection))
        {
            cardObject.ColorCardBorder(Color.blue);
            return;
        }

        cardObject.ColorCardBorder(Color.red);
        return;
    }

    public Bounds CardBounds
    {
        get 
        {
            return _cardPrefabRenderer.bounds;
        }
    }
}
