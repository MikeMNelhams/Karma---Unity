using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.GameExceptions;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Board.BoardEvents;
using KarmaLogic.Players;
using KarmaLogic.Cards;
using KarmaLogic.Controller;
using System;
using System.Linq;
using DataStructures;

public class KarmaGameManager : MonoBehaviour
{
    private static KarmaGameManager _instance;
    public static KarmaGameManager Instance { get { return _instance; } }

    public GameObject _cardPrefab;
    public GameObject _playerPrefab;

    [SerializeField] GameObject _currentPlayerArrow;
    [SerializeField] PlayTableProperties _playTable;

    [SerializeField] int _turnLimit = 100;
    [SerializeField] KarmaPlayerStartInfo[] _playersStartInfo;

    [SerializeField] int _whichPlayerStarts = 0;
    [SerializeField] bool _isDebuggingMode = false;

    public List<PlayerProperties> PlayersProperties { get; protected set; }

    public IBoard Board { get; protected set; }
    public PickupPlayPile PickUpAction { get; set; } = new ();
    public PlayCardsCombo PlayCardsComboAction { get; set; } = new ();

    public HashSet<int> ValidPlayerIndicesForVoting { get; set; } = new ();
    Dictionary<int, int> PlayerJokerCounts { get; set; } = new ();
    Dictionary<int, int> GameRanks { get; set; }
    Dictionary<int, int> VotesForWinners { get; set; }

    int _totalAvailableVotesForWinners = -1;

    public Transform CardTransform { get { return _cardPrefab.transform; } }

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
    }

    void Start()
    {
        //List<List<List<int>>> playerCardValues = new()
        //{
        //    new() { new() { }, new() { }, new() { } },
        //    new() { new() { }, new() { }, new() { } },
        //    new() { new() { 4, 5, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
        //    new() { new() { 4, 5, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
        //};

        //List<int> drawCardValues = new() { };
        //List<int> playCardValues = new() { 9, 10, 11 };
        //List<int> burnCardValues = new() { };

        //Board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, whoStarts: _whichPlayerStarts);
        int numberOfPlayers = _playersStartInfo.Length;
        Board = BoardFactory.RandomStart(numberOfPlayers, numberOfJokers: 1, whoStarts: _whichPlayerStarts);

        CheckIfGameTurnTimerExceeded(Board);
        InitializeGameRanks();
        RegisterBoardEvents();
        CreatePlayerObjects(_playersStartInfo);
        CreatePlayerCardObjectsFromBoard();
        
        _playTable.CreateCardObjectPilesFromBoard(Board);

        AssignButtonEvents();
        _currentPlayerArrow.SetActive(true);
        Board.StartTurn();
    }

    void RegisterBoardEvents()
    {
        Board.BoardEventSystem.RegisterOnTurnStartEventListener(new BoardEventSystem.BoardEventListener(StartTurn));
        Board.BoardEventSystem.RegisterOnTurnStartEventListener(new BoardEventSystem.BoardEventListener(CheckIfWinner));

        Board.BoardEventSystem.RegisterPlayerDrawEventListener(new BoardEventSystem.PlayerDrawEventListener(DrawCards));
        Board.BoardEventSystem.RegisterHandsFlippedEventListener(new BoardEventSystem.BoardEventListener(FlipHandsAnimation));
        Board.BoardEventSystem.RegisterHandsRotatedListener(new BoardEventSystem.BoardHandsRotationEventListener(RotateHandsInTurnOrderAnimation));
        Board.BoardEventSystem.RegisterStartCardGiveAwayListener(new BoardEventSystem.BoardOnStartCardGiveAwayListener(StartGiveAwayCards));
        Board.BoardEventSystem.RegisterPlayPileGiveAwayListener(new BoardEventSystem.BoardOnStartPlayPileGiveAwayListener(StartGiveAwayPlayPile));

        Board.BoardEventSystem.RegisterOnBurnEventListener(new BoardEventSystem.BoardBurnEventListener(BurnCards));

        Board.BoardEventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckIfWinner));
        Board.BoardEventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckIfGameTurnTimerExceeded));
        Board.BoardEventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(NextTurn));
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
            PlayersProperties.Add(playerProperties);
            
            if (playerIndex != Board.CurrentPlayerIndex) { playerProperties.EnableCamera(); }
            else { playerProperties.DisableCamera(); }

            if (playersStartInfo[playerIndex].isPlayableCharacter) { playerProperties.Controller = new PlayerController(); }
            else 
            {
                IntegrationTestBot bot = new ("Bot" + botNameIndex, 0.0f);
                playerProperties.Controller = new BotController(bot);
                botNameIndex++;
            } 
        }

        if (isGameWonWithVoting) { SetupPlayerActionStatesForVotingForWinner(); }
        else { SetupPlayerActionStateForBasicStart(); }

        SetupPlayerMovementControllers(playersStartInfo);
    }

    void SetupPlayerActionStateForBasicStart()
    {
        for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
        {
            PlayerProperties playerProperties = PlayersProperties[playerIndex];
            if (playerIndex != Board.CurrentPlayerIndex) { playerProperties.SetControllerState(new WaitForTurn(Board, playerProperties)); }
            else { playerProperties.EnableCamera(); }
        }
    }

    void SetupPlayerActionStatesForVotingForWinner()
    {
        for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
        {
            PlayerProperties playerProperties = PlayersProperties[playerIndex];
            bool playerHasVotes = Board.Players[playerIndex].CountValue(CardValue.JOKER) > 0;
            if (!playerHasVotes) { playerProperties.SetControllerState(new WaitForTurn(Board, playerProperties)); }
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
            if (playerProperties.cardHolder == null) { continue; }
            GameObject cardHolder = playerProperties.cardHolder;
            CreatePlayerHandCards(i, cardHolder);
        }

        for (int i = 0; i < Board.Players.Count; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            Player player = Board.Players[i];
            if (i >= _playTable.boardHolders.Count) { break; }
            if (_playTable.boardHolders[i] == null) { continue; }
            GameObject boardHolder = _playTable.boardHolders[i];
            KarmaBoardHandler karmaBoardManager = boardHolder.GetComponent<KarmaBoardHandler>();
            
            playerProperties.CardsInKarmaUp = new ListWithConstantContainsCheck<CardObject>(karmaBoardManager.CreateKarmaUpCards(player.KarmaUp));

            foreach (CardObject card in playerProperties.CardsInKarmaUp)
            {
                playerProperties.SetCardObjectOnMouseDownEvent(card);
            }

            playerProperties.CardsInKarmaDown = new ListWithConstantContainsCheck<CardObject>(karmaBoardManager.CreateKarmaDownCards(player.KarmaDown));

            foreach (CardObject card in playerProperties.CardsInKarmaDown)
            {
                playerProperties.SetCardObjectOnMouseDownEvent(card);
            }
        }   
    }

    void CreatePlayerHandCards(int playerIndex, GameObject cardHolder)
    {
        ListWithConstantContainsCheck<CardObject> cardObjects = new ();
        foreach (Card card in Board.Players[playerIndex].Hand)
        {
            CardObject cardObject = InstantiateCard(card, Vector3.zero, Quaternion.identity, cardHolder).GetComponent<CardObject>();
            PlayersProperties[playerIndex].SetCardObjectOnMouseDownEvent(cardObject);
            cardObjects.Add(cardObject);
        }

        PlayersProperties[playerIndex].PopulateHand(cardObjects);
    }

    public GameObject InstantiateCard(Card card, Vector3 cardPosition, Quaternion cardRotation, GameObject parent)
    {
        GameObject cardObject = Instantiate(_cardPrefab, cardPosition, cardRotation, parent.transform);
        CardObject cardFrontBackRenderer = cardObject.GetComponent<CardObject>();
        cardFrontBackRenderer.SetCard(card);
        return cardObject;
    }

    public void FlipHandsAnimation(IBoard board)
    {
        for (int i = 0; i < board.Players.Count; i++)
        {
            PlayersProperties[i].FlipHand();
        }
    }

    public void RotateHandsInTurnOrderAnimation(int numberOfRotations, IBoard board) 
    {
        int k = numberOfRotations % board.Players.Count;
        RotateHandsAnimation(k * ((int) board.TurnOrder), board);
        return;
    }

    void RotateHandsAnimation(int numberOfRotations, IBoard board)
    {
        List<ListWithConstantContainsCheck<CardObject>> beginHands = new();

        for (int i = 0; i < board.Players.Count; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            ListWithConstantContainsCheck<CardObject> hand = playerProperties.CardsInHand;
            foreach (CardObject cardObject in hand)
            {
                playerProperties.RemoveCardObjectOnMouseDownEvent(cardObject);
            }

            beginHands.Add(hand);
        }

        Deque<ListWithConstantContainsCheck<CardObject>> hands = new (beginHands);

        hands.Rotate(numberOfRotations);
        
        for (int i = 0; i < board.Players.Count; i++)
        {
            ListWithConstantContainsCheck<CardObject> hand = hands[i];
            PlayerProperties playerProperties = PlayersProperties[i];
            foreach (CardObject cardObject in hand)
            {
                playerProperties.SetCardObjectOnMouseDownEvent(cardObject);
            }
            PlayersProperties[i].PopulateHand(hand);
        }
    }

    public void BurnCards(int jokerCount)
    {
        if (jokerCount == 0)
        {
            _playTable.MoveEntirePlayPileToBurnPile();
            return;
        }
        
        _playTable.MoveTopCardsFromPlayPileToBurnPile(jokerCount);
    }

    void MoveCurrentPlayerArrow()
    {
        PlayerProperties playerProperties = PlayersProperties[Board.CurrentPlayerIndex];
        Quaternion towardsTable = Quaternion.LookRotation(_playTable.transform.position - playerProperties.transform.position);
        _currentPlayerArrow.transform.SetPositionAndRotation(playerProperties.gameObject.transform.position + new Vector3(0, -1.5f, 0), Quaternion.Euler(0, towardsTable.eulerAngles.y, 90));
    }

    void MoveCardsFromSelectionToPlayPile(int playerIndex)
    {
        List<CardObject> cardObjects = PlayersProperties[playerIndex].PopSelectedCardsFromSelection();
        _playTable.MoveCardsToTopOfPlayPile(cardObjects);
    }

    void DrawCards(int numberOfCards, int playerIndex)
    {
        List<CardObject> cardsDrawn = _playTable.DrawCards(numberOfCards);
        PlayersProperties[playerIndex].AddCardObjectsToHand(cardsDrawn);
    }

    void StartGiveAwayCards(int numberOfCards, int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        // Each giveaway is a separate CardGiveAwayHandler, which also automatically removes its listeners on completion, so no memory leaks.

        Board.Players[playerIndex].CardGiveAwayHandler.RegisterOnCardGiveAwayListener(new CardGiveAwayHandler.OnCardGiveAwayListener(GiveAwayCard)); 
        playerProperties.SetControllerState(new SelectingCardGiveAwaySelectionIndex(Board, playerProperties));
    }

    void StartGiveAwayPlayPile(int giverIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[giverIndex];

        playerProperties.SetControllerState(new SelectingPlayPileGiveAwayPlayerIndex(Board, playerProperties));
    }

    void GiveAwayCard(Card card, int giverIndex, int receiverIndex)
    {
        PlayersProperties[receiverIndex].ReceivePickedUpCard(PlayersProperties[giverIndex]);
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

    void CheckIfWinner(IBoard board)
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

    void VoteForWinners()
    {
        UpdatePlayerJokerCounts();
        UpdateValidTargetPlayersForWinVoting();

        _totalAvailableVotesForWinners = Enumerable.Sum(PlayerJokerCounts.Values);
        HashSet<int> playerIndicesToExclude = new();
        playerIndicesToExclude.UnionWith(Enumerable.Range(0, Board.Players.Count).ToList<int>());
        playerIndicesToExclude.ExceptWith(Board.PotentialWinnerIndices);
        
        foreach (int playerIndex in PlayerJokerCounts.Keys)
        {
            Board.CurrentPlayerIndex = playerIndex;
            PlayersProperties[playerIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
            PlayersProperties[playerIndex].SetControllerState(new VotingForWinner(Board, PlayersProperties[playerIndex]));
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

    void StartTurn(IBoard board)
    {
        MoveCurrentPlayerArrow();
        Board.Print();
        if (IsGameWonWithoutVoting(board) || IsGameWonWithVoting(board)) { return; }

        print("Starting game...");
        PlayersProperties[board.CurrentPlayerIndex].SetControllerState(new PickingAction(board, PlayersProperties[board.CurrentPlayerIndex]));
    }

    void NextTurn(IBoard board)
    {
        if (PlayersProperties[board.PlayerIndexWhoStartedTurn].Controller.State is SelectingCardGiveAwaySelectionIndex)
        {
            print("You need to giveaway: " + Board.Players[Board.PlayerIndexWhoStartedTurn].CardGiveAwayHandler.NumberOfCardsRemainingToGiveAway + " cards");
            // They can != only by PP: K, K, K BP: 9, You play K -> Q. It's incredibly RARE, but requires this check.
            Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;  
            return;
        }

        if (PlayersProperties[board.PlayerIndexWhoStartedTurn].Controller.State is SelectingPlayPileGiveAwayPlayerIndex)
        {
            // They can != only by PP: K, K, K BP: 9, You play K -> Q. It's incredibly RARE, but requires this check. 
            Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn; 
            return;
        }

        if (PlayersProperties[board.PlayerIndexWhoStartedTurn].Controller.State is VotingForWinner)
        {
            // They can != only by PP: K, K, K BP: 9, You play K -> Q. It's incredibly RARE, but requires this check. 
            Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;
            return;
        }

        if (!board.HasBurnedThisTurn)
        {
            StepToNextPlayer();
            return;
        }

        if (board.Players[board.PlayerIndexWhoStartedTurn].HasCards) 
        { 
            PlayTurnAgain();
            return;
        }

        StepToNextPlayer();
    }

    void StepToNextPlayer()
    {
        PlayersProperties[Board.PlayerIndexWhoStartedTurn].SetControllerState(new WaitForTurn(Board, PlayersProperties[Board.PlayerIndexWhoStartedTurn]));
        IfDebugModeDisableStartingPlayerMovement();
        Board.StepPlayerIndex(1);
        Board.StartTurn();
        IfDebugModeEnableCurrentPlayerMovement();
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

    void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
    {
        if (!VotesForWinners.ContainsKey(voteTargetIndex)) { VotesForWinners[voteTargetIndex] = 0; }
        VotesForWinners[voteTargetIndex] += PlayerJokerCounts[votingPlayerIndex];
        int totalVotes = Enumerable.Sum(VotesForWinners.Values);
        if (totalVotes == _totalAvailableVotesForWinners) { DecideWinners(); throw new GameWonException(GameRanks); }
        PlayersProperties[votingPlayerIndex].SetControllerState(new WaitForTurn(Board, PlayersProperties[votingPlayerIndex]));
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
            PlayersProperties[index].confirmSelectionButton.onClick.AddListener(delegate { TriggerCardSelectionConfirmed(index); });
            PlayersProperties[index].pickupPlayPileButton.onClick.AddListener(delegate { TriggerPickupActionSelected(index); });
        }
    }

    void TriggerCardSelectionConfirmed(int playerIndex)
    {
        if (PlayersProperties[playerIndex].Controller.State is PickingAction) { AttemptToPlayCardSelection(playerIndex); return; }
        if (PlayersProperties[playerIndex].Controller.State is SelectingCardGiveAwaySelectionIndex) { AttemptToGiveAwayCardSelection(playerIndex); return; }

        throw new NotImplementedException();
    }

    void TriggerPickupActionSelected(int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        PickUpAction.Apply(Board, playerProperties.Controller, playerProperties.CardSelector.Selection);
        List<CardObject> playPileCards = _playTable.PopAllFromPlayPile();
        PlayersProperties[playerIndex].AddCardObjectsToHand(playPileCards);
        Board.EndTurn();
    }

    void AttemptToPlayCardSelection(int playerIndex)
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

            CardsList cardSelection = playerProperties.CardSelector.Selection;
            MoveCardsFromSelectionToPlayPile(playerIndex);
            PlayCardsComboAction.Apply(Board, playerProperties.Controller, cardSelection);
            Board.EndTurn();
        }
    }

    void AttemptToGiveAwayCardSelection(int playerIndex)
    {
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
        HashSet<CardObject> selectedCards = playerProperties.CardSelector.CardObjects;

        if (selectedCards.Count != 1) { return; }

        CardObject selectedCard = selectedCards.First();
        if (!validCardValues.Contains(selectedCard.CurrentCard.Value)) { return; }
        
        playerProperties.SetControllerState(new SelectingCardGiveAwayPlayerIndex(Board, PlayersProperties[playerIndex]));
        playerProperties.PickedUpCard = selectedCard;
    }

    void AttemptGiveAwayPickedUpCard(int giverIndex, int targetIndex)
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
            PlayersProperties[giverIndex].SetControllerState(new WaitForTurn(Board, PlayersProperties[giverIndex]));
            Board.EndTurn();
            return;
        }

        PlayersProperties[giverIndex].SetControllerState(new SelectingCardGiveAwaySelectionIndex(Board, PlayersProperties[giverIndex]));
    }

    void AttemptGiveAwayPlayPile(int giverIndex, int targetIndex)
    {
        if (targetIndex == giverIndex) { return; }

        print("Giving away PLAY PILE (board) to player: " + targetIndex);
        Board.Players[targetIndex].Pickup(Board.PlayPile);

        PlayersProperties[targetIndex].AddCardObjectsToHand(_playTable.PopAllFromPlayPile());

        PlayersProperties[giverIndex].SetControllerState(new WaitForTurn(Board, PlayersProperties[giverIndex]));
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
}
