using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Karma.GameExceptions;
using Karma.BasicBoard;
using Karma.Board;
using Karma.Board.BoardEvents;
using Karma.Players;
using Karma.Cards;
using System;
using Karma.Controller;
using System.Linq;
using DataStructures;


public class KarmaGameManager : MonoBehaviour
{
    private static KarmaGameManager _instance;
    public static KarmaGameManager Instance { get { return _instance; } }

    public GameObject cardPrefab;
    public GameObject playerPrefab;

    [SerializeField] GameObject _currentPlayerArrow;
    [SerializeField] PlayTableProperties _playTable;

    [SerializeField] int _turnLimit = 100;
    [SerializeField] KarmaPlayerStartInfo[] _playersStartInfo;

    [Range(0f, 3f)][SerializeField] int _whichPlayerStarts = 0;
    [SerializeField] bool _isDebuggingMode = false;

    public List<PlayerProperties> PlayersProperties { get; protected set; }

    public IBoard Board { get; protected set; }
    public PickupPlayPile PickUpAction { get; set; } = new PickupPlayPile();
    public PlayCardsCombo PlayCardsComboAction { get; set; } = new PlayCardsCombo();

    Dictionary<int, int> GameRanks { get; set; }
    Dictionary<int, int> VotesForWinners { get; set; }
    Dictionary<int, int> PlayerJokerCounts { get; set; }

    public Transform CardTransform { get { return cardPrefab.transform; } }

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
        List<List<List<int>>> playerCardValues = new()
        {
            new() { new() { 2, 5, 12 }, new() { 7, 2, 14 }, new() { 10, 5, 7 } },
            new() { new() { 3, 10, 11 }, new() { 14, 6, 13 }, new() { 3, 14, 4 } },
            new() { new() { 4, 5, 15 }, new() { 6, 7, 2 }, new() { 2, 13, 9 } },
            new() { new() { 4, 5, 10 }, new() { 12, 11, 8 }, new() { 10, 13, 9 } }
        };

        List<int> drawCardValues = new() { 3, 4, 6, 7, 8 };
        List<int> playCardValues = new() { };
        List<int> burnCardValues = new() { };

        Board = BoardFactory.MatrixStart(playerCardValues, drawCardValues, playCardValues, burnCardValues, whoStarts: _whichPlayerStarts);
        //int numberOfPlayers = _playersStartInfo.Length;
        //Board = BoardFactory.RandomStart(numberOfPlayers, numberOfJokers: 1, whoStarts: _whichPlayerStarts);

        RegisterBoardEvents();
        CreatePlayers(_playersStartInfo);
        CreatePlayerCardsFromBoard();
        
        _playTable.CreateCardPilesFromBoard(Board);

        InitializeGameRanks();
        AssignButtonEvents();
        _currentPlayerArrow.SetActive(true);
        Board.StartTurn();
    }

    void RegisterBoardEvents()
    {
        Board.BoardEventSystem.RegisterOnTurnStartEventListener(new BoardEventSystem.BoardEventListener(StartTurn));

        Board.BoardEventSystem.RegisterPlayerDrawEventListener(new BoardEventSystem.PlayerDrawEventListener(DrawCards));
        Board.BoardEventSystem.RegisterHandsFlippedEventListener(new BoardEventSystem.BoardEventListener(FlipHandsAnimation));
        Board.BoardEventSystem.RegisterHandsRotatedListener(new BoardEventSystem.BoardHandsRotationEventListener(RotateHandsInTurnOrderAnimation));
        Board.BoardEventSystem.RegisterStartCardGiveAwayListener(new BoardEventSystem.BoardOnStartCardGiveAwayListener(StartGiveAwayCards));
        Board.BoardEventSystem.RegisterPlayPileGiveAwayListener(new BoardEventSystem.BoardOnStartPlayPileGiveAwayListener(StartGiveAwayPlayPile));

        Board.BoardEventSystem.RegisterOnBurnEventListener(new BoardEventSystem.BoardBurnEventListener(BurnCards));

        Board.BoardEventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(CheckIfWinner));
        Board.BoardEventSystem.RegisterOnTurnEndEventListener(new BoardEventSystem.BoardEventListener(NextTurn));
    }

    void CreatePlayers(KarmaPlayerStartInfo[] playersStartInfo)
    {
        PlayersProperties = new ();
        int botNameIndex = 0;
        for (int i = 0; i < playersStartInfo.Length; i++)
        {
            Vector3 tableDirection = _playTable.centre - playersStartInfo[i].startPosition;
            tableDirection.y = 0;
            GameObject player = Instantiate(playerPrefab, playersStartInfo[i].startPosition, Quaternion.LookRotation(tableDirection));
            player.name = "Player " + i;

            PlayerProperties playerProperties = player.GetComponent<PlayerProperties>();
            playerProperties.Index = i;
            playerProperties.RegisterPickedUpCardOnClickEventListener(AttemptGiveAwayPickedUpCard);
            playerProperties.RegisterTargetPickUpPlayPileEventListener(AttemptGiveAwayPlayPile);
            playerProperties.SetHandSorter(BoardPlayerHandSorter);
            PlayersProperties.Add(playerProperties);
            bool isCurrentPlayer = i == Board.CurrentPlayerIndex;
            if (isCurrentPlayer) { playerProperties.EnableCamera(); }
            else { playerProperties.DisableCamera(); }

            if (playersStartInfo[i].isPlayableCharacter) { playerProperties.Controller = new PlayerController(); }
            else 
            {
                IntegrationTestBot bot = new ("Bot" + botNameIndex, 0.0f);
                playerProperties.Controller = new BotController(bot);
                botNameIndex++;
            }

            if (!isCurrentPlayer) { playerProperties.SetControllerState(new WaitForTurn(Board, playerProperties)); }
        }

        SetupPlayerMovementControllers(playersStartInfo);
    }

    void SetupPlayerMovementControllers(KarmaPlayerStartInfo[] playersStartInfo)
    {
        for (int i = 0; i < playersStartInfo.Length; i++)
        {
            PlayerProperties playerProperties = PlayersProperties[i];
            if (!playersStartInfo[i].isPlayableCharacter) { continue; }

            if (!_isDebuggingMode) 
            {
                playerProperties.IsRotationEnabled = true;
                playerProperties.IsPointingEnabled = true;
                continue;
            }
        }

        if (_isDebuggingMode)
        {
            EnablePlayerMovement(Board.CurrentPlayerIndex);
        }
    }

    void CreatePlayerCardsFromBoard()
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
        GameObject cardObject = Instantiate(cardPrefab, cardPosition, cardRotation, parent.transform);
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
                cardObject.transform.SetParent(playerProperties.cardHolder.transform);
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

    void InitializeGameRanks()
    {
        GameRanks = new Dictionary<int, int>();
        for (int i = 0; i < Board.Players.Count; i++)
        {
            GameRanks[i] = Board.Players[i].Length;
        }
        VotesForWinners = new ();
    }

    void CheckIfWinner(IBoard board)
    {
        UpdateGameRanks();
        int numberOfPotentialWinners = board.PotentialWinnerIndices.Count;
        if (numberOfPotentialWinners == 1 && board.CardValuesInPlayCounts[CardValue.JOKER] == 0)
        {
            throw new GameWonException(GameRanks);
        }
        if (numberOfPotentialWinners >= 2 && board.CardValuesInPlayCounts[CardValue.JOKER] == 0)
        {
            throw new GameWonException(GameRanks);
        }
        if (numberOfPotentialWinners >= 2)
        {
            VoteForWinners();
            throw new GameWonException(GameRanks);
        }

        if (board.TurnsPlayed >= _turnLimit)
        {
            throw new GameTurnLimitExceededException(GameRanks, _turnLimit);
        }
    }

    void VoteForWinners()
    {
        Dictionary<int, int> jokerCounts = new();
        for (int i = 0; i < Board.Players.Count; i++)
        {
            Player player = Board.Players[i];
            int jokerCount = player.CountValue(CardValue.JOKER);
            if (jokerCount > 0)
            {
                jokerCounts[i] = jokerCount;
            }
        }
        PlayerJokerCounts = jokerCounts;
        HashSet<int> playerIndicesToExclude = new();
        playerIndicesToExclude.UnionWith(Enumerable.Range(0, Board.Players.Count).ToList<int>());
        playerIndicesToExclude.ExceptWith(Board.PotentialWinnerIndices);
        foreach (int playerIndex in jokerCounts.Keys)
        {
            int numberOfVotes = jokerCounts[playerIndex];
            Board.CurrentPlayerIndex = playerIndex;
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
        GameRanks = new();
        foreach ((int rank, HashSet<int> pair) in ranks)
        {
            foreach (int playerIndex in pair)
            {
                GameRanks[playerIndex] = rank;
            }
        }
    }

    void NextTurn(IBoard board)
    {
        if (PlayersProperties[board.PlayerIndexWhoStartedTurn].Controller.State is SelectingCardGiveAwaySelectionIndex)
        {
            print("CARD GIVEAWAY SELECTION MODE! TURN AIN'T OVER YET BUDDY");
            print("You need to giveaway: " + Board.Players[Board.PlayerIndexWhoStartedTurn].CardGiveAwayHandler.NumberOfCardsRemainingToGiveAway + " cards");
            Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn; // This only can occur PP: K, K, K BP: 9, You play K -> Q. It's incredibly RARE, but requires this check. 
            return;
        }

        if (PlayersProperties[board.PlayerIndexWhoStartedTurn].Controller.State is SelectingPlayPileGiveAwayPlayerIndex)
        {
            print("WOW WOW WOW, JOKER PLAYED by player: " + board.PlayerIndexWhoStartedTurn);
            Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn; // This only can occur PP: K, K, K BP: 9, You play K -> Q. It's incredibly RARE, but requires this check. 
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

    void StartTurn(IBoard board)
    {
        MoveCurrentPlayerArrow();
        Board.Print();
        PlayersProperties[board.CurrentPlayerIndex].SetControllerState(new PickingAction(board, PlayersProperties[board.CurrentPlayerIndex]));
    }

    void IfDebugModeDisableStartingPlayerMovement()
    {
        if (_isDebuggingMode && _playersStartInfo[Board.PlayerIndexWhoStartedTurn].isPlayableCharacter)
        {
            DisablePlayerMovement(Board.PlayerIndexWhoStartedTurn);
        }
    }

    void IfDebugModeEnableCurrentPlayerMovement()
    {
        if (_isDebuggingMode && _playersStartInfo[Board.PlayerIndexWhoStartedTurn].isPlayableCharacter)
        {
            EnablePlayerMovement(Board.CurrentPlayerIndex);
        }
    }

    void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
    {
        if (!VotesForWinners.ContainsKey(voteTargetIndex)) { VotesForWinners[voteTargetIndex] = 0; }
        VotesForWinners[voteTargetIndex] += PlayerJokerCounts[votingPlayerIndex];
        int totalAvailableVotes = Enumerable.Sum(PlayerJokerCounts.Values);
        int totalVotes = Enumerable.Sum(VotesForWinners.Values);
        if (totalVotes == totalAvailableVotes) { DecideWinners(); }
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

        // TODO Currently being given card via Joker and Via Queen WON'T add the physical cards to hand. Do this with events
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
        string selectedCardsMessage = "Attempting card giveaway. Selected Cards: ";

        foreach (CardObject card in selectedCards)
        {
            selectedCardsMessage += card + " ";
        }
        print(selectedCardsMessage);
        if (selectedCards.Count != 1) { return; }

        CardObject selectedCard = selectedCards.First();
        if (!validCardValues.Contains(selectedCard.CurrentCard.Value)) { return; }
        print("CARD " + selectedCard + " IS VALID FOR GIVEAWAY!!");
        
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
        Board.DrawUntilFull(giverIndex); // TODO move this into BasicBoard somehow?

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

    void EnablePlayerMovement(int playerIndex)
    {
        PlayersProperties[playerIndex].IsRotationEnabled = true;
        PlayersProperties[playerIndex].IsPointingEnabled = true;
    }

    void DisablePlayerMovement(int playerIndex)
    {
        PlayersProperties[playerIndex].IsRotationEnabled = false;
    }
}
