using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.GameExceptions;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Board.BoardEvents;
using KarmaLogic.Players;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using StateMachineV2;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataStructures;

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

    [SerializeField] int _turnLimit = 100;
    [SerializeField] KarmaPlayerStartInfo[] _playersStartInfo;

    [SerializeField] int _whichPlayerStarts = 0;
    [SerializeField] bool _isDebuggingMode = false;
    int _numberOfPlayableCharacters;

    public List<PlayerProperties> PlayersProperties { get; protected set; }

    public IBoard Board { get; protected set; }
    public PickupPlayPile PickUpAction { get; set; } = new ();
    public PlayCardsCombo PlayCardsComboAction { get; set; } = new ();

    public HashSet<int> ValidPlayerIndicesForVoting { get; set; } = new ();
    Dictionary<int, int> PlayerJokerCounts { get; set; } = new ();
    Dictionary<int, int> GameRanks { get; set; }
    Dictionary<int, int> VotesForWinners { get; set; }

    int _totalAvailableVotesForWinners = -1;

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

        // BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, whoStarts: _whichPlayerStarts);
        Board = BoardTestFactory.BotQueenCombo();
        //Board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, whoStarts: _whichPlayerStarts);
        //int numberOfPlayers = _playersStartInfo.Length;
        //Board = BoardFactory.RandomStart(numberOfPlayers, numberOfJokers: 1, whoStarts: _whichPlayerStarts);

        CheckIfGameTurnTimerExceeded(Board);
        InitializeGameRanks();
        RegisterBoardEvents();
        CreatePlayerObjects(_playersStartInfo);
        CreatePlayerCardObjectsFromBoard();
        
        _playTable.CreateCardObjectPilesFromBoard(Board);

        AssignButtonEvents();
        _currentPlayerArrowHandler.SetArrowVisibility(true);
        _playOrderArrowHandler.SetArrowVisibility(true);
        _numberOfPlayableCharacters = _playersStartInfo.Where(x => x.isPlayableCharacter).Count();
        Board.StartTurn();
    }

    void RegisterBoardEvents()
    {
        Board.EventSystem.RegisterOnTurnStartEventListener(new BoardEventSystem.BoardEventListener(StartTurn));

        Board.EventSystem.RegisterPlayerDrawEventListener(new BoardEventSystem.PlayerDrawEventListener(DrawCards));
        Board.EventSystem.RegisterHandsFlippedEventListener(new BoardEventSystem.BoardEventListener(FlipHands));
        Board.EventSystem.RegisterHandsRotatedListener(new BoardEventSystem.BoardHandsRotationEventListener(RotateHandsInTurnOrder));
        Board.EventSystem.RegisterStartCardGiveAwayListener(new BoardEventSystem.BoardOnStartCardGiveAwayListener(StartGiveAwayCards));
        Board.EventSystem.RegisterPlayPileGiveAwayListener(new BoardEventSystem.BoardOnStartPlayPileGiveAwayListener(StartGiveAwayPlayPile));

        Board.EventSystem.RegisterOnBurnEventListener(new BoardEventSystem.BoardBurnEventListener(BurnCards));

        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(IfWinnerVoteOrEndGame));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckPotentialWinner));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckIfGameTurnTimerExceeded));
        Board.EventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(NextTurn));
    }

    void CreatePlayerObjects(KarmaPlayerStartInfo[] playersStartInfo)
    {
        PlayersProperties = new ();
        int botNameIndex = 0;
        bool isGameWonWithVoting = IsGameWonWithVoting(Board);
        for (int playerIndex = 0; playerIndex < playersStartInfo.Length; playerIndex++)
        {
            Vector3 tableDirection = _playTable.centre - playersStartInfo[playerIndex].startPosition;
            tableDirection.y = 0;

            GameObject player = Instantiate(_playerPrefab, playersStartInfo[playerIndex].startPosition, Quaternion.LookRotation(tableDirection));
            PlayerProperties playerProperties = player.GetComponent<PlayerProperties>();
            player.name = "Player " + playerIndex;
            playerProperties.Index = playerIndex;
            playerProperties.RegisterPickedUpCardOnClickEventListener(AttemptGiveAwayPickedUpCard);
            playerProperties.RegisterTargetPickUpPlayPileEventListener(AttemptGiveAwayPlayPile);
            playerProperties.SetHandSorter(BoardPlayerHandSorter);
            Board.Players[playerIndex].Hand.RegisterHandOrderChangeEvent(playerProperties.SortHand);
            PlayersProperties.Add(playerProperties);

            if (playersStartInfo[playerIndex].isPlayableCharacter) 
            { 
                playerProperties.StateMachine = new PlayerStateMachine(playerProperties, Board);
            }
            else 
            {
                string botName = "Bot " + botNameIndex;
                IntegrationTestBot bot = new (botName, 0.1f);
                playerProperties.StateMachine = new BotStateMachine(bot, playerProperties, Board);
                playerProperties.name = botName;
                playerProperties.DisableCamera();
                botNameIndex++;
            } 
        }

        if (isGameWonWithVoting) { SetupPlayerActionStatesForVotingForWinner(); }
        else { SetupPlayerActionStateForBasicStart(); }
        
        SetupPlayerMovementControllers(playersStartInfo);
    }

    async void SetupPlayerActionStateForBasicStart()
    {
        for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
        {

            PlayerProperties playerProperties = PlayersProperties[playerIndex];
            if (playerIndex == Board.CurrentPlayerIndex) 
            {
                await playerProperties.ProcessStateCommand(Command.TurnStarted);
            }
            else 
            {
                await playerProperties.ProcessStateCommand(Command.TurnEnded);
            }
        }
    }

    async void SetupPlayerActionStatesForVotingForWinner()
    {
        bool setFirstVotingPlayerForDebugMode = false;
        for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
        {
            PlayerProperties playerProperties = PlayersProperties[playerIndex];
            bool playerHasVotes = Board.Players[playerIndex].CountValue(CardValue.JOKER) > 0;
            if (!Board.Players[playerIndex].HasCards)
            {
                await playerProperties.ProcessStateCommand(Command.HasNoCards);
                continue;
            }

            if (!playerHasVotes) 
            { 
                await playerProperties.ProcessStateCommand(Command.TurnEnded);
                continue;
            }

            if (_isDebuggingMode && setFirstVotingPlayerForDebugMode) 
            { 
                await playerProperties.ProcessStateCommand(Command.TurnEnded); 
                continue; 
            }
            setFirstVotingPlayerForDebugMode = true;
            await playerProperties.ProcessStateCommand(Command.VotingStarted); 

        }
    }

    void SetupPlayerMovementControllers(KarmaPlayerStartInfo[] playersStartInfo)
    {
        bool isGameWonWithVoting = IsGameWonWithVoting(Board);

        for (int i = 0; i < playersStartInfo.Length; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            if (!playersStartInfo[i].isPlayableCharacter) { continue; }

            if (!_isDebuggingMode || isGameWonWithVoting) 
            {
                playerProperties.EnablePlayerMovement();
                continue;
            }
        }

        if (_isDebuggingMode)
        {
            PlayersProperties[Board.CurrentPlayerIndex].EnablePlayerMovement();
        }
    }

    void CreatePlayerCardObjectsFromBoard()
    {
        for (int i = 0; i < Board.Players.Count; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            playerProperties.InstantiatePlayerHandFan(Board.Players[i].Hand);
            if (!_playersStartInfo[i].isPlayableCharacter ) { PlayersProperties[i].TurnOffLegalMoveHints(); }
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

    public void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement=null)
    {
        if (!_isDebuggingMode) { return; }
        if (_numberOfPlayableCharacters <= 1) { return; }

        for (int playerIndex = playerCameraDisabledIndex + 1; playerIndex < Board.Players.Count; playerIndex++)
        {
            if (!_playersStartInfo[playerIndex].isPlayableCharacter) { continue; }
            if (stateRequirement != null && !stateRequirement(PlayersProperties[playerIndex].StateMachine.CurrentState)) { continue; }
            PlayersProperties[playerIndex].EnableCamera();
            return;
        }

        for (int playerIndex = 0; playerIndex < playerCameraDisabledIndex; playerIndex++)
        {
            if (!_playersStartInfo[playerIndex].isPlayableCharacter) { continue; }
            if (stateRequirement != null && !stateRequirement(PlayersProperties[playerIndex].StateMachine.CurrentState)) { continue; }
            PlayersProperties[playerIndex].EnableCamera();
            return;
        }
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

    async void StartGiveAwayPlayPile(int giverIndex)
    {
        print("START PLAY PILE GIVE AWAY");
        await PlayersProperties[giverIndex].ProcessStateCommand(Command.PlayPileGiveAwayComboPlayed);
    }

    void GiveAwayCard(Card card, int giverIndex, int receiverIndex)
    {
        PlayersProperties[receiverIndex].ReceivePickedUpCard(PlayersProperties[giverIndex]);
        print("Player received card!");
    }

    void InitializeGameRanks()
    {
        GameRanks = new Dictionary<int, int>();
        for (int i = 0; i < Board.Players.Count; i++)
        {
            GameRanks[i] = Board.Players[i].Length;
        }
        VotesForWinners = new ();
    }

    bool IsGameWonWithVoting(IBoard board)
    {
        return board.PotentialWinnerIndices.Count >= 2 && board.CardValuesInPlayCounts[CardValue.JOKER] > 0;
    }

    bool IsGameWonWithoutVoting(IBoard board)
    {
        return board.PotentialWinnerIndices.Count >= 1 && board.CardValuesInPlayCounts[CardValue.JOKER] == 0;
    }

    void IfWinnerVoteOrEndGame(IBoard board)
    {
        UpdateGameRanks();

        if (IsGameWonWithVoting(board))
        {
            VoteForWinners();
        }

        if (IsGameWonWithoutVoting(board))
        {
            throw new GameWonException(GameRanks);
        }
    }

    void CheckIfGameTurnTimerExceeded(IBoard board)
    {
        if (board.TurnsPlayed >= _turnLimit)
        {
            throw new GameTurnLimitExceededException(GameRanks, _turnLimit);
        }
    }

    async void VoteForWinners()
    {
        UpdatePlayerJokerCounts();
        UpdateValidTargetPlayersForWinVoting();

        _totalAvailableVotesForWinners = Enumerable.Sum(PlayerJokerCounts.Values);
        HashSet<int> playerIndicesToExclude = new();
        playerIndicesToExclude.UnionWith(Enumerable.Range(0, Board.Players.Count).ToList<int>());
        playerIndicesToExclude.ExceptWith(Board.PotentialWinnerIndices);

        
        if (_isDebuggingMode)
        {
            int firstValidIndex = PlayerJokerCounts.Keys.Min(); // Where(x => x > Board.CurrentPlayerIndex)
            PlayersProperties[firstValidIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
            await PlayersProperties[firstValidIndex].ProcessStateCommand(Command.VotingStarted);
            return;
        }
        else
        {
            List<Task> tasks = new ();

            // Voting is asynchronous
            foreach (int playerIndex in PlayerJokerCounts.Keys)
            {
                Board.CurrentPlayerIndex = playerIndex;
                PlayersProperties[playerIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
                tasks.Add(PlayersProperties[playerIndex].ProcessStateCommand(Command.VotingStarted));
            }

            await Task.WhenAll(tasks);
        } 
    }

    void UpdateGameRanks()
    {
        Dictionary<int, HashSet<int>> cardCounts = new();
        for (int i = 0; i < Board.Players.Count; i++)
        {
            if (!cardCounts.ContainsKey(Board.Players[i].Length)) { cardCounts[Board.Players[i].Length] = new HashSet<int>(); }
            cardCounts[Board.Players[i].Length].Add(i);
        }

        List<Tuple<int, HashSet<int>>> ranks = new();
        foreach (int key in cardCounts.Keys)
        {
            HashSet<int> playerIndices = cardCounts[key];
            ranks.Add(Tuple.Create(key, playerIndices));
        }

        ranks.Sort((x, y) => x.Item1.CompareTo(y.Item1));
        GameRanks = new Dictionary<int, int>();

        for (int rank = 0; rank < ranks.Count; rank++)
        {
            Tuple<int, HashSet<int>> pair = ranks[rank];
            foreach (int playerIndex in pair.Item2)
            {
                GameRanks[playerIndex] = rank;
            }
        }
    }

    void UpdateValidTargetPlayersForWinVoting()
    {
        ValidPlayerIndicesForVoting.Clear();

        for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
        {
            if (!Board.Players[playerIndex].HasCards)
            {
                ValidPlayerIndicesForVoting.Add(playerIndex);
            }
        }
    }

    void UpdatePlayerJokerCounts()
    {
        PlayerJokerCounts.Clear();
        for (int i = 0; i < Board.Players.Count; i++)
        {
            Player player = Board.Players[i];
            int jokerCount = player.CountValue(CardValue.JOKER);
            if (jokerCount > 0)
            {
                PlayerJokerCounts[i] = jokerCount;
            }
        }
    }

    async void StartTurn(IBoard board)
    {
        RotatePlayOrderArrow();
        MoveCurrentPlayerArrow();
        Board.Print();

        bool alreadyVoting = ValidPlayerIndicesForVoting.Count > 0;
        bool gameIsWon = IsGameWonWithoutVoting(board) || IsGameWonWithVoting(board);

        if (!alreadyVoting && gameIsWon) { IfWinnerVoteOrEndGame(board); return; }

        if (PlayersProperties[board.CurrentPlayerIndex].StateMachine.CurrentState is State.PotentialWinner)
        {
            Board.EndTurn();
            return;
        }

        if (!Board.CurrentPlayer.HasCards) { await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.HasNoCards); }

        if (_playersStartInfo[board.CurrentPlayerIndex].isPlayableCharacter) { IfDebugModeEnableCurrentPlayerMovement(); }
        if (PlayerJokerCounts.Keys.Contains(Board.CurrentPlayerIndex))
        {
            PlayersProperties[board.CurrentPlayerIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
            await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.VotingStarted);
            return;
        }
        await PlayersProperties[board.CurrentPlayerIndex].ProcessStateCommand(Command.TurnStarted);
    }

    async void NextTurn(IBoard board)
    {
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

        if (board.HasBurnedThisTurn && board.Players[board.PlayerIndexWhoStartedTurn].HasCards)
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
        if (_playersStartInfo[Board.CurrentPlayerIndex].isPlayableCharacter) { IfDebugModeDisableStartingPlayerMovement(); }
        Board.StepPlayerIndex(1);
        print("Starting Turn. Current active player: " + Board.CurrentPlayerIndex);
        Board.StartTurn();
    }

    void PlayTurnAgain()
    {
        Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;
        Board.StartTurn();
    }

    void IfDebugModeDisableStartingPlayerMovement()
    {
        if (_isDebuggingMode && _playersStartInfo[Board.PlayerIndexWhoStartedTurn].isPlayableCharacter)
        {
            PlayersProperties[Board.PlayerIndexWhoStartedTurn].DisablePlayerMovement();
        }
    }

    void IfDebugModeEnableCurrentPlayerMovement()
    {
        if (_isDebuggingMode && _playersStartInfo[Board.PlayerIndexWhoStartedTurn].isPlayableCharacter)
        {
            PlayersProperties[Board.PlayerIndexWhoStartedTurn].EnablePlayerMovement();
        }
    }

    async void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
    {
        print("Triggered a vote for player: " + voteTargetIndex + " by " + votingPlayerIndex);
        
        if (!VotesForWinners.ContainsKey(voteTargetIndex)) { VotesForWinners[voteTargetIndex] = 0; }
        VotesForWinners[voteTargetIndex] += PlayerJokerCounts[votingPlayerIndex];
        int totalVotes = Enumerable.Sum(VotesForWinners.Values);
        print("There are " + totalVotes + " out of " + _totalAvailableVotesForWinners);
        if (totalVotes == _totalAvailableVotesForWinners) 
        { 
            DecideWinners(); 
            throw new GameWonException(GameRanks); 
        }
        await PlayersProperties[votingPlayerIndex].ProcessStateCommand(Command.GameEnded);
        EnableNextPlayableCamera(voteTargetIndex, IsWaitingTurn);
        Board.EndTurn();
    }
    
    bool IsWaitingTurn(State state)
    {
        return state == State.WaitingForTurn;
    }

    void DecideWinners()
    {
        if (VotesForWinners.Count > 0)
        {
            int mostVotes = Enumerable.Max(VotesForWinners.Values);
            List<int> mostVotedPlayerIndices = new();
            foreach (int playerIndex in VotesForWinners.Keys)
            {
                if (VotesForWinners[playerIndex] == mostVotes)
                {
                    mostVotedPlayerIndices.Add(playerIndex);
                }
            }
            HashSet<int> loserIndices = new();
            loserIndices.UnionWith(Enumerable.Range(0, Board.Players.Count));
            loserIndices.ExceptWith(mostVotedPlayerIndices);
            foreach (int playerIndex in loserIndices)
            {
                GameRanks[playerIndex]++;
            }
        }
    }

    void AssignButtonEvents()
    {
        for (int i = 0; i < PlayersProperties.Count; i++)
        {
            int index = i;
            async Task triggerCardSelectionConfirmed() => await TriggerCardSelectionConfirmed(index);
            async Task triggerClearSelection() => await TriggerClearSelection(index);
            async Task triggerPickupActionSelected() => await TriggerPickupActionSelected(index);
            PlayersProperties[i].ConfirmSelectionButton.onClick.AddListener(triggerCardSelectionConfirmed);
            PlayersProperties[i].ClearSelectionButton.onClick.AddListener(triggerClearSelection);
            PlayersProperties[i].PickupPlayPileButton.onClick.AddListener(triggerPickupActionSelected);
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
            await PlayersProperties[giverIndex].StateMachine.MoveNext(Command.TurnEnded);
            return;
        }

        await PlayersProperties[giverIndex].StateMachine.MoveNext(Command.CardGiveAwayUnfinished);
    }

    async void AttemptGiveAwayPlayPile(int giverIndex, int targetIndex)
    {
        if (targetIndex == giverIndex) { return; }

        print("Giving away PLAY PILE (board) to player: " + targetIndex);
        Board.Players[targetIndex].Pickup(Board.PlayPile);

        PlayersProperties[targetIndex].AddCardObjectsToHand(_playTable.PopAllFromPlayPile());

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
