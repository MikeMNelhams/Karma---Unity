using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using StateMachine.CharacterStateMachines;
using UnityEngine;
using KarmaLogic.BasicBoard;
using KarmaLogic.Bots;
using PlayTable;

namespace KarmaPlayerMode
{
    [System.Serializable]
    public abstract class KarmaPlayerMode
    {
        public int TurnLimit { get; protected set; }
        public IBoard Board { get; protected set; }
        public List<PlayerProperties> PlayersProperties { get; protected set; }
        public List<PlayerKarmaBoardHolderProperties> PlayersBoardHolderProperties { get; protected set; }

        protected abstract List<KarmaPlayModeBoardPreset<BasicBoard>> GetBasicBoardPresets();
        protected List<KarmaPlayModeBoardPreset<BasicBoard>> BasicBoardPresets { get; set; }
        public BasicBoardParams BoardParams { get; protected set; }
        public abstract int NumberOfActivePlayers { get; }

        public int NumberOfPlayersFinishedMulligan { get; protected set; }

        public Dictionary<int, int> VotesForWinners { get; protected set; }
        public Dictionary<int, int> PlayerJokerCounts { get; protected set; }
        public Dictionary<int, int> GameRanks { get; protected set; }
        public HashSet<int> ValidPlayerIndicesForVoting { get; protected set; }

        public bool IsGameOver { get; protected set; }
        public bool IsGameWon { get; protected set; }

        public KarmaPlayerMode(BasicBoardParams basicBoardParams = null)
        {
            BasicBoardPresets = GetBasicBoardPresets();
            BoardParams = basicBoardParams;
            BoardParams ??= new BasicBoardParams();
            Board = new BasicBoard(BoardParams);
            CreatePlayerObjects();
            TurnLimit = basicBoardParams.TurnLimit;

            NumberOfPlayersFinishedMulligan = 0;
            BasicBoardPresets = new ();
            DeclareGameRankingsInfo();

            SetupPlayerActionStates();
            SetupPlayerMovementControllers();

            CheckIfGameTurnTimerExceeded();
            InitializeGameRanks();
        }

        public KarmaPlayerMode(int basicBoardPresetIndex)
        {
            BasicBoardPresets = GetBasicBoardPresets();
            TurnLimit = BasicBoardPresetTurnLimit(basicBoardPresetIndex);

            BoardParams = BasicBoardPreset(basicBoardPresetIndex);
            Board = new BasicBoard(BoardParams);
            CreatePlayerObjects();
            
            NumberOfPlayersFinishedMulligan = 0;
            BasicBoardPresets = new();
            DeclareGameRankingsInfo();

            SetupPlayerActionStates();
            SetupPlayerMovementControllers();

            CheckIfGameTurnTimerExceeded();
            InitializeGameRanks();
        }

        protected void DeclareGameRankingsInfo()
        {
            VotesForWinners = new Dictionary<int, int>();
            PlayerJokerCounts = new Dictionary<int, int>();
            GameRanks = new Dictionary<int, int>();
            ValidPlayerIndicesForVoting = new HashSet<int>();
            IsGameOver = false;
            IsGameWon = false;
        }

        void CreatePlayerObjects()
        {
            PlayersProperties = new();
            PlayersBoardHolderProperties = new();
            int botNameIndex = 0;

            float botDelay = KarmaGameManager.Instance.GlobalBotDelayInSeconds;

            CirclularTable tableGeometry = KarmaGameManager.Instance.PlayTableProperties.TableGeometry;
            Vector3[] playerStartPositions = tableGeometry.PlayerPositions(Board.Players.Count);

            Vector3[] holderStartPositions = tableGeometry.PlayerKarmaPositions(Board.Players.Count);
            Quaternion[] holderStartRotations = tableGeometry.PlayerKarmaRotations(Board.Players.Count, holderStartPositions);

            for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
            {
                GameObject player = KarmaGameManager.Instance.InstantiatePlayer(playerStartPositions[playerIndex]);

                PlayerProperties playerProperties = player.GetComponent<PlayerProperties>();
                player.name = "Player " + playerIndex;
                playerProperties.Index = playerIndex;
                PlayersProperties.Add(playerProperties);

                if (IsPlayableCharacter(playerIndex))
                {
                    playerProperties.StateMachine = new PlayerStateMachine(playerProperties);
                }
                else
                {
                    string botName = "Bot " + botNameIndex;
                    IntegrationTestBot bot = new (botName, botDelay);
                    playerProperties.StateMachine = new BotStateMachine(bot, playerProperties, Board);
                    playerProperties.name = botName;
                    playerProperties.DisableCamera();
                    botNameIndex++;
                }

                PlayerKarmaBoardHolderProperties holderProperties = KarmaGameManager.Instance.
                    InstantiatePlayerKarmaBoardHolder(holderStartPositions[playerIndex],
                    holderStartRotations[playerIndex]);

                holderProperties.name = playerProperties.name + " karmaBoard";
                holderProperties.gameObject.transform.position += new Vector3(0, holderProperties.HolderCuboidRenderer.bounds.extents.y / 2, 0);
                PlayersBoardHolderProperties.Add(holderProperties);
            }


        }

        public abstract void SetupPlayerActionStateForBasicStart();

        public abstract void SetupPlayerActionStatesForVotingForWinner();

        public abstract void SetupPlayerMovementControllers();

        public abstract void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement = null);

        public abstract void IfPlayableDisableStartingPlayerMovement();

        public abstract void IfPlayableEnableCurrentPlayerMovement();

        public abstract Task VoteForWinners();

        public abstract Task TriggerFinishMulligan(int playerIndex);

        public BasicBoardParams BasicBoardPreset(int presetIndex)
        {
            if (IsValidBoardPresetIndex(presetIndex)) { throw new BoardPresetException("Invalid preset index!"); }
            return BasicBoardPresets[presetIndex].BoardParams;
        }

        public int BasicBoardPresetTurnLimit(int presetIndex)
        {
            if (IsValidBoardPresetIndex(presetIndex)) { throw new BoardPresetException("Invalid preset index!"); }
            return BasicBoardPresets[presetIndex].TurnLimit;
        }

        bool IsValidBoardPresetIndex(int presetIndex)
        {
            return presetIndex < 0 || presetIndex >= BasicBoardPresets.Count;
        }

        public void SetupPlayerActionStates()
        {
            if (IsGameWonWithVoting) { SetupPlayerActionStatesForVotingForWinner(); }
            else { SetupPlayerActionStateForBasicStart(); }
        }

        public bool IsGameWonWithVoting
        {
            get
            {
                return Board.PotentialWinnerIndices.Count >= 2 && Board.CardValuesInPlayCounts[CardValue.JOKER] > 0;
            }
        }

        public bool IsGameWonWithoutVoting
        {
            get
            {
                return Board.PotentialWinnerIndices.Count >= 1 && Board.CardValuesInPlayCounts[CardValue.JOKER] == 0;
            }
        }

        public void IfWinnerVoteOrEndGame()
        {
            UpdateGameRanks();

            if (IsGameWonWithVoting)
            {
                VoteForWinners();
            }

            if (IsGameWonWithoutVoting)
            {
                IsGameOver = true;
                IsGameWon = true;
                UnityEngine.Debug.LogWarning("Game has finished. Game ranks: " + string.Join(Environment.NewLine, GameRanks));
            }
        }

        public async void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
        {
            int totalAvailableVotesForWinners = Enumerable.Sum(PlayerJokerCounts.Values);
            if (!VotesForWinners.ContainsKey(voteTargetIndex)) { VotesForWinners[voteTargetIndex] = 0; }
            VotesForWinners[voteTargetIndex] += PlayerJokerCounts[votingPlayerIndex];
            int totalVotes = Enumerable.Sum(VotesForWinners.Values);
            UnityEngine.Debug.Log("There are " + totalVotes + " votes out of " + totalAvailableVotesForWinners);
            if (totalVotes == totalAvailableVotesForWinners)
            {
                DecideWinners();
                IsGameOver = true;
                IsGameWon = true;
                UnityEngine.Debug.LogWarning("Game has finished. Game ranks: " + string.Join(Environment.NewLine, GameRanks));
                return;
            }
            await PlayersProperties[votingPlayerIndex].ProcessStateCommand(Command.GameEnded);
            EnableNextPlayableCamera(voteTargetIndex, IsWaitingTurn);
            Board.EndTurn();
        }

        public void CheckIfGameTurnTimerExceeded()
        {
            if (Board.TurnsPlayed >= TurnLimit)
            {
                IsGameOver = true;
                IsGameWon = false;
                UnityEngine.Debug.LogWarning("Game has finished by turn limit exceeded. Game ranks: " + string.Join(Environment.NewLine, GameRanks));
            }
        }

        public bool IsPlayableCharacter(int playerIndex)
        {
            return BoardParams.PlayersParams[playerIndex].IsPlayableCharacter;
        }

        protected bool IsMulliganFinished
        {
            get
            {
                UnityEngine.Debug.Log("Number of mulligan finished players: " + NumberOfPlayersFinishedMulligan);
                UnityEngine.Debug.Log("Number of total players: " + Board.Players.Count);
                return NumberOfPlayersFinishedMulligan == Board.Players.Count;
            }
        }

        protected void InitializeGameRanks()
        {
            GameRanks = new Dictionary<int, int>();
            for (int i = 0; i < Board.Players.Count; i++)
            {
                GameRanks[i] = Board.Players[i].Length;
            }
            VotesForWinners = new();
        }

        protected void UpdatePlayerJokerCounts()
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

        protected void UpdateValidTargetPlayersForWinVoting()
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

        protected void UpdateGameRanks()
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

        protected void DecideWinners()
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

        protected bool IsWaitingTurn(State state)
        {
            return state == State.WaitingForTurn;
        }
    }
}
