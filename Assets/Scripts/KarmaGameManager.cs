using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Karma.Game;
using Karma.BasicBoard;
using Karma.Board;
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
    public PlayTableProperties playTable;

    [SerializeField] int _turnLimit = 100;

    [SerializeField] List<Vector3> playerPositions;

    [SerializeField] List<bool> arePlayableCharacters;
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

    // Start is called before the first frame update
    void Start()
    {
        int numberOfPlayers = playerPositions.Count;
        Board = BoardFactory.RandomStart(numberOfPlayers, 1);
        CreatePlayers(playerPositions);
        CreatePlayerCardsFromBoard();
        playTable.CreateCardPilesFromBoard(Board);

        InitializeGameRanks();
        AssignButtonEvents();
    }

    void CreatePlayers(List<Vector3> playerStartPositions)
    {
        PlayersProperties = new ();
        int botNameIndex = 0;
        for (int i = 0; i < playerStartPositions.Count; i++)
        {
            Vector3 tableDirection = playTable.centre - playerStartPositions[i];
            tableDirection.y = 0;
            GameObject player = Instantiate(playerPrefab, playerStartPositions[i], Quaternion.LookRotation(tableDirection));
            player.name = "Player " + i;

            PlayerProperties playerProperties = player.GetComponent<PlayerProperties>();
            PlayersProperties.Add(playerProperties);
            bool isCurrentPlayer = i == Board.CurrentPlayerIndex;
            if (isCurrentPlayer) { playerProperties.EnableCamera(); }
            else { playerProperties.DisableCamera(); }

            if (arePlayableCharacters[i]) { playerProperties.Controller = new PlayerController(); }
            else 
            {
                IntegrationTestBot bot = new ("Bot" + botNameIndex, 0.0f);
                playerProperties.Controller = new BotController(bot);
                botNameIndex++;
            }

            if (isCurrentPlayer) { playerProperties.SetControllerState(new PickingAction(Board, playerProperties)); }
            else { playerProperties.SetControllerState(new WaitForTurn(Board, playerProperties)); }
        }
    }

    public void SetCardObjectProperties(Card card, GameObject cardObject)
    {
        cardObject.name = card.ToString();
        CardObject cardRenderer = cardObject.GetComponent<CardObject>();
        cardRenderer.SetCard(card);
    }

    void CreatePlayerCardsFromBoard()
    {
        
        for (int i = 0; i < Board.Players.Count; i++)
        {
            Player player = Board.Players[i];
            PlayerProperties playerProperties = PlayersProperties[i]; 
            if (playerProperties.cardHolder == null) { continue; }
            GameObject cardHolder = playerProperties.cardHolder;
            CreateCardsForHolder(i, cardHolder);
        }

        for (int i = 0; i < Board.Players.Count; i++)
        {
            Player player = Board.Players[i];
            if (i >= playTable.boardHolders.Count) { break; }
            if (playTable.boardHolders[i] == null) { continue; }
            GameObject boardHolder = playTable.boardHolders[i];
            KarmaBoardManager karmaBoardManager = boardHolder.GetComponent<KarmaBoardManager>();
            karmaBoardManager.CreateKarmaCards(player.KarmaUp, player.KarmaDown);
        }   
    }

    void CreateCardsForHolder(int playerIndex, GameObject cardHolder, float startAngle=-20.0f, float endAngle=20.0f, 
        float distanceFromHolder=0.75f)
    {
        List<CardObject> cardObjects = new ();
        foreach (Card card in Board.Players[playerIndex].Hand)
        {
            CardObject cardObject = Instantiate(cardPrefab, cardHolder.transform).GetComponent<CardObject>();
            cardObject.SetCard(card);
            cardObjects.Add(cardObject);
        }

        PlayersProperties[playerIndex].PopulateHand(cardObjects, startAngle, endAngle, distanceFromHolder);
    }

    void MoveCardsFromHandToPlayPile(List<CardObject> cardObjects, int playerIndex)
    {
        foreach (CardObject cardObject in cardObjects)
        {
            cardObject.OnCardClick -= PlayersProperties[playerIndex].CardSelector.Toggle;
            PlayersProperties[playerIndex].CardsInHand.Remove(cardObject);
        }
        playTable.MoveCardsToTopOfPlayPile(cardObjects);
    }

    void DrawCards(int numberOfCards, int playerIndex)
    {
        List<CardObject> cardsDrawn = playTable.DrawCards(numberOfCards);
        MoveCardObjectsIntoHand(playerIndex, cardsDrawn);
    }

    void MoveCardObjectsIntoHand(int playerIndex, List<CardObject> cardObjectsToAdd)
    {
        List<CardObject> currentHandCardObjects = PlayersProperties[playerIndex].CardsInHand;
        currentHandCardObjects.AddRange(cardObjectsToAdd);

        Dictionary<Card, List<int>> cardPositions = new();
        int n = Board.Players[playerIndex].Hand.Count;
        for (int i = 0; i < n; i++)
        {
            Card card = Board.Players[playerIndex].Hand[i];
            if (!cardPositions.ContainsKey(card)) { cardPositions[card] = new List<int>(); }
            cardPositions[card].Add(i);
        }
        print(Board.Players[playerIndex].Hand);
        CardObject[] handCorrectOrder = new CardObject[n];
        foreach (CardObject cardObject in currentHandCardObjects)
        {
            int position = cardPositions[cardObject.CurrentCard].Last();
            handCorrectOrder[position] = cardObject;
            Debug.Log("Moving card: " + cardObject.CurrentCard + cardObject + " to position: " + position);
            List<int> positions = cardPositions[cardObject.CurrentCard];
            positions.RemoveAt(positions.Count - 1);
            cardObject.transform.SetParent(PlayersProperties[playerIndex].cardHolder.transform);
        }

        List<CardObject> finalHandCardObjects = new();
        for (int i = 0; i < handCorrectOrder.Length; i++)
        {
            if (handCorrectOrder[i] != null)
            {
                finalHandCardObjects.Add(handCorrectOrder[i]);
            }
        }
        Debug.Log("Hand (Board): " + Board.CurrentPlayer.Hand);
        string handCardObjectsString = "Hand (CardObjects):";
        foreach (CardObject cardObject in finalHandCardObjects)
        {
            handCardObjectsString += " " + cardObject;
        }
        Debug.Log(handCardObjectsString);
        PlayersProperties[playerIndex].PopulateHand(finalHandCardObjects);
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

    public void EndTurn()
    {
        Board.EndTurn();
        CheckIfWinner();
        NextTurn();
    }

    void CheckIfWinner()
    {
        UpdateGameRanks();
        int numberOfPotentialWinners = Board.PotentialWinnerIndices.Count;
        if (numberOfPotentialWinners == 1 && Board.NumberOfJokersInPlay == 0)
        {
            throw new GameWonException(GameRanks);
        }
        if (numberOfPotentialWinners >= 2 && Board.NumberOfJokersInPlay == 0)
        {
            throw new GameWonException(GameRanks);
        }
        if (numberOfPotentialWinners >= 2)
        {
            VoteForWinners();
            throw new GameWonException(GameRanks);
        }

        if (Board.TurnsPlayed >= _turnLimit)
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
            int jokerCount = player.NumberOfJokers;
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

    void NextTurn()
    {
        if (Board.HasBurnedThisTurn && Board.Players[Board.PlayerIndexWhoStartedTurn].HasCards) 
        { 
            PlayTurnAgain();
            return;
        }

        StepToNextPlayer();
    }

    void StepToNextPlayer()
    {
        PlayersProperties[Board.CurrentPlayerIndex].SetControllerState(new WaitForTurn(Board, PlayersProperties[Board.CurrentPlayerIndex]));
        Board.StepPlayerIndex(1);
        StartTurn();
    }

    void PlayTurnAgain()
    {
        Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;
        StartTurn();
    }

    void StartTurn()
    {
        Board.StartTurn();
        PlayersProperties[Board.CurrentPlayerIndex].SetControllerState(new PickingAction(Board, PlayersProperties[Board.CurrentPlayerIndex]));
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
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        CardSelector cardSelector = playerProperties.CardSelector;
        print("Attempting to play cards: " + cardSelector.Selection);
        print("Attempting to play cardValues: " + cardSelector.SelectionCardValues);
        string currentLegalCombos = "Currently legal: ";
        foreach (FrozenMultiSet<CardValue> combo in Board.CurrentLegalCombos)
        {
            currentLegalCombos += combo + ", ";
        }
        print(currentLegalCombos + "]");

        // TODO Currently being given card via Joker and Via Queen WON'T add the physical cards to hand. Do this with events (Don't ask me how T.T)
        if (Board.CurrentLegalCombos.Contains(cardSelector.SelectionCardValues))
        {
            print("Card combo is LEGAL!! Proceeding to play it...");
            
            List<CardObject> cardObjects = cardSelector.CardObjects.ToList();
            PlayCardsComboAction.Apply(Board, playerProperties.Controller);
            MoveCardsFromHandToPlayPile(cardObjects, playerIndex);
            foreach (CardObject cardObject in cardObjects)
            {
                cardObject.DisableSelectShader();
                cardSelector.Remove(cardObject);
            }
            if (Board.CurrentPlayer.Hand.Count > 0) { DrawCards(Board.NumberOfCardsDrawnThisTurn, playerIndex); }
            EndTurn();
        }
    }

    void TriggerPickupActionSelected(int playerIndex)
    {
        PlayerProperties playerProperties = PlayersProperties[playerIndex];
        PickUpAction.Apply(Board, playerProperties.Controller);
        List<CardObject> playPileCards = playTable.PopAllFromPlayPile();
        MoveCardObjectsIntoHand(playerIndex, playPileCards);
        EndTurn();
    }

    public GameObject InstantiateCard(Card card, Vector3 cardPosition, Quaternion cardRotation, GameObject parent)
    {
        GameObject cardObject = Instantiate(KarmaGameManager.Instance.cardPrefab, cardPosition, cardRotation, parent.transform);
        CardObject cardFrontBackRenderer = cardObject.GetComponent<CardObject>();
        cardFrontBackRenderer.SetCard(card);
        return cardObject;
    }
}
