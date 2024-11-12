using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.GameExceptions;
using KarmaLogic.Players;
using StateMachines.CharacterStateMachines;
using UnityEngine;

namespace KarmaPlayerMode
{
    [System.Serializable]
    public abstract class KarmaPlayerMode
    {

        public KarmaPlayerStartInfo[] PlayersStartInfo { get; protected set; }
        public int TurnLimit { get; protected set; }
        public IBoard Board { get; protected set; }
        public List<PlayerProperties> PlayersProperties { get; protected set; }
        public abstract int NumberOfActivePlayers { get; }
        
        public Dictionary<int, int> VotesForWinners { get; protected set; }
        public Dictionary<int, int> PlayerJokerCounts { get; protected set; }
        public Dictionary<int, int> GameRanks { get; protected set; }
        public HashSet<int> ValidPlayerIndicesForVoting { get; protected set; }
        public int NumberOfPlayersFinishedMulligan { get; protected set; }

        public KarmaPlayerMode(KarmaPlayerStartInfo[] playersStartInfo, IBoard board, List<PlayerProperties> playersProperties, int turnLimit = 100)
        {
            PlayersStartInfo = playersStartInfo;
            Board = board;
            PlayersProperties = playersProperties;
            TurnLimit = turnLimit;
            NumberOfPlayersFinishedMulligan = 0;
            DeclareGameInfo();

            CheckIfGameTurnTimerExceeded();
            InitializeGameRanks();
        }

        protected void DeclareGameInfo()
        {
            VotesForWinners = new Dictionary<int, int>();
            PlayerJokerCounts = new Dictionary<int, int>();
            GameRanks = new Dictionary<int, int>();
            ValidPlayerIndicesForVoting = new HashSet<int>();
        }

        public abstract void SetupPlayerActionStateForBasicStart();

        public abstract void SetupPlayerActionStatesForVotingForWinner();

        public abstract void SetupPlayerMovementControllers();

        public abstract void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement = null);

        public abstract void IfDebugModeDisableStartingPlayerMovement();

        public abstract void IfDebugModeEnableCurrentPlayerMovement();

        public abstract Task VoteForWinners();

        public abstract Task TriggerFinishMulligan(int playerIndex);

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
                throw new GameWonException(GameRanks);
            }
        }

        public async void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
        {
            int totalAvailableVotesForWinners = Enumerable.Sum(PlayerJokerCounts.Values);
            if (!VotesForWinners.ContainsKey(voteTargetIndex)) { VotesForWinners[voteTargetIndex] = 0; }
            VotesForWinners[voteTargetIndex] += PlayerJokerCounts[votingPlayerIndex];
            int totalVotes = Enumerable.Sum(VotesForWinners.Values);
            UnityEngine.Debug.Log("There are " + totalVotes + " out of " + totalAvailableVotesForWinners);
            if (totalVotes == totalAvailableVotesForWinners)
            {
                DecideWinners();
                throw new GameWonException(GameRanks);
            }
            await PlayersProperties[votingPlayerIndex].ProcessStateCommand(Command.GameEnded);
            EnableNextPlayableCamera(voteTargetIndex, IsWaitingTurn);
            Board.EndTurn();
        }

        public void CheckIfGameTurnTimerExceeded()
        {
            if (Board.TurnsPlayed >= TurnLimit)
            {
                throw new GameTurnLimitExceededException(GameRanks, TurnLimit);
            }
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
