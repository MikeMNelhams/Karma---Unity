using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using KarmaLogic.Board;
using KarmaLogic.BasicBoard;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using KarmaPlayerMode.GameTeardown;
using StateMachine.CharacterStateMachines;

namespace KarmaPlayerMode
{
    [System.Serializable]
    public abstract class KarmaPlayerMode
    {
        public int TurnLimit { get; protected set; }
        public IBoard Board { get; protected set; }
        public List<PlayerHandler> PlayerHandlers { get; protected set; }
        public List<PlayerKarmaBoardHolderHandler> PlayersBoardHolderHandlers { get; protected set; }

        protected abstract List<KarmaPlayModeBoardPreset<BasicBoard>> GetBasicBoardPresets();
        protected List<KarmaPlayModeBoardPreset<BasicBoard>> BasicBoardPresets { get; set; }
        public BasicBoardParams BoardParams { get; protected set; }
        public int NumberOfActivePlayers { get; protected set; }
        public int NumberOfPlayersToMulligan { get; protected set; }
        public int NumberOfPlayersFinishedMulligan { get; protected set; }

        public Dictionary<int, int> VotesForWinners { get; protected set; }
        public Dictionary<int, int> PlayerJokerCounts { get; protected set; }
        public Dictionary<int, int> GameRanks { get; protected set; }
        public HashSet<int> ValidPlayerIndicesForVoting { get; protected set; }

        public bool IsGameOver { get; set; }
        public bool IsGameWon { get; set; }
        public bool IsGameOverDueToNoLegalActions { get; set; }

        public KarmaPlayerMode(BasicBoardParams basicBoardParams = null)
        {
            BasicBoardPresets = GetBasicBoardPresets();
            BoardParams = basicBoardParams;
            BoardParams ??= new BasicBoardParams();
            Board = new BasicBoard(BoardParams);
            CreatePlayerObjects();
            TurnLimit = basicBoardParams.TurnLimit;

            SetupGame();
        }

        public KarmaPlayerMode(int basicBoardPresetIndex)
        {
            BasicBoardPresets = GetBasicBoardPresets();
            TurnLimit = BasicBoardPresetTurnLimit(basicBoardPresetIndex);

            BoardParams = BasicBoardPreset(basicBoardPresetIndex);
            Board = new BasicBoard(BoardParams);
            CreatePlayerObjects();

            SetupGame();
        }

        void SetupGame()
        {
            NumberOfPlayersFinishedMulligan = 0;
            BasicBoardPresets = new();
            DeclareGameRankingsInfo();

            SetupPlayerActionStates();
            SetupPlayerMovementControllers();

            CheckIfGameTurnTimerExceeded(Board);
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
            IsGameOverDueToNoLegalActions = false;
        }

        protected abstract void CreatePlayerObjects();

        public abstract void SetupPlayerActionStateForBasicStart();

        public abstract void SetupPlayerActionStatesForVotingForWinner();

        public abstract void SetupPlayerMovementControllers();

        public async void NextTurn(IBoard board)
        {
            // WARNING: Using PlayerHandler.StateMachine.CurrentState is unstable, since this is async VOID!
            PlayerHandler activePlayerHandler = PlayerHandlers[Board.PlayerIndexWhoStartedTurn];
            State activePlayerState = activePlayerHandler.StateMachine.CurrentState;
            UnityEngine.Debug.Log("While end of turn CURRENT PLAYER STATE: " + activePlayerHandler.StateMachine.CurrentState);

            if (activePlayerState is State.PotentialWinner || activePlayerState is State.GameOver)
            {
                StepToNextPlayer();
                return;
            }

            if (Board.CurrentPlayer.PlayPileGiveAwayHandler != null && !Board.CurrentPlayer.PlayPileGiveAwayHandler.IsFinished)
            {
                UnityEngine.Debug.Log("The good ending!");
                await PlayerHandlers[board.CurrentPlayerIndex].ProcessStateCommand(Command.PlayPileGiveAwayComboPlayed);
                return;
            }

            if (Board.CurrentPlayer.PlayPileGiveAwayHandler != null && Board.CurrentPlayer.PlayPileGiveAwayHandler.IsFinished)
            {
                PlayTurnAgain();
                return;
            }

            if (board.HasBurnedThisTurn && board.Players[board.CurrentPlayerIndex].HasCards)
            {
                UnityEngine.Debug.Log("WE BURNED. LET'S GO AGAIN!");
                PlayTurnAgain();
                return;
            }

            if (Board.CurrentPlayer.CardGiveAwayHandler != null && !Board.CurrentPlayer.CardGiveAwayHandler.IsFinished)
            {
                return;
            }

            if (activePlayerState is not State.WaitingForTurn)
            {
                await activePlayerHandler.ProcessStateCommand(Command.TurnEnded);
            }

            StepToNextPlayer();
            return;
        }

        void StepToNextPlayer()
        {
            if (IsGameOver) { return; }

            if (IsPlayableCharacter(Board.CurrentPlayerIndex)) { IfPlayableDisableStartingPlayerMovement(); }
            Board.StepPlayerIndex(1);
            UnityEngine.Debug.Log("Starting Turn. Current active player: " + Board.CurrentPlayerIndex);
            Board.StartTurn();
        }

        void PlayTurnAgain()
        {
            if (IsGameOver) { return; }

            Board.CurrentPlayerIndex = Board.PlayerIndexWhoStartedTurn;
            Board.StartTurn();
        }

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

        public async void IfWinnerVoteOrEndGame(IBoard board)
        {
            UpdateGameRanks();

            if (IsGameWonWithVoting)
            {
                await VoteForWinners();
            }

            if (IsGameWonWithoutVoting)
            {

                FinishGame(new GameWonTeardown());
            }

            await CheckPotentialWinner(board);
        }

        async Task CheckPotentialWinner(IBoard board)
        {
            if (board.CurrentPlayer.HasCards) { return; }

            await PlayerHandlers[board.CurrentPlayerIndex].ProcessStateCommand(Command.HasNoCards);
        }

        public abstract void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex);

        public void CheckIfGameTurnTimerExceeded(IBoard board)
        {
            if (board.TurnsPlayed >= TurnLimit)
            {
                FinishGame(new GameTurnLimitExceededTeardown());
            }
        }

        public bool IsPlayableCharacter(int playerIndex)
        {
            return BoardParams.CharactersParams[playerIndex].IsPlayableCharacter;
        }

        protected bool AreLegalHintsEnabled(int playerIndex)
        {
            return BoardParams.CharactersParams[playerIndex].AreLegalHintsEnabled;
        }

        protected bool IsMulliganFinished
        {
            get
            {
                UnityEngine.Debug.Log("Number of mulligan finished players: " + NumberOfPlayersFinishedMulligan);
                UnityEngine.Debug.Log("Number of total players: " + NumberOfPlayersToMulligan);
                return NumberOfPlayersFinishedMulligan == NumberOfPlayersToMulligan;
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

        public void FlipHands(IBoard board)
        {
            for (int i = 0; i < board.Players.Count; i++)
            {
                PlayerHandlers[i].FlipHand();
            }
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
            if (VotesForWinners.Count == 0) { return; }

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

        protected bool IsWaitingTurn(State state)
        {
            return state == State.WaitingForTurn;
        }

        public void EndGameEarlyDueToNoLegalActions()
        {
            UpdateGameRanks();
            FinishGame(new GameEndedDueToNoLegalActionsTeardown());
        }
        
        protected void FinishGame(IKarmaPlayerModeTeardown gameTeardown, bool destroyGame = false)
        {
            gameTeardown.Apply(this);

            if (!destroyGame)
            {
                DisablePlayerActions();
                return;
            }

            DestroyBoardHolders();
            DestroyPlayers();
        }

        public void DestroyGame()
        {
            UpdateGameRanks();

            FinishGame(new GameExitedEarlyTeardown(), true);
        }

        public void DisablePlayerActions()
        {
            foreach (PlayerHandler handler in PlayerHandlers)
            {
                handler.HoverTipHandler.enabled = false;
                handler.Canvas.enabled = false;
            }
        }

        void DestroyBoardHolders()
        {
            foreach (PlayerKarmaBoardHolderHandler playerBoardHolderHandler in PlayersBoardHolderHandlers)
            {
                playerBoardHolderHandler.Destroy();
            }
        }

        void DestroyPlayers()
        {
            foreach (PlayerHandler playerHandler in PlayerHandlers)
            {
                playerHandler.Destroy();
            }
        }

        protected void UpdateActivePlayersCount()
        {
            NumberOfActivePlayers = 0;

            foreach (BasicBoardCharacterParams playerParams in BoardParams.CharactersParams)
            {
                if (playerParams.IsPlayableCharacter) { NumberOfActivePlayers++; }
            }
        }

        protected void UpdateNumberOfPlayersToMulligan()
        {
            NumberOfPlayersToMulligan = Board.Players.Count;

            foreach (Player player in Board.Players)
            {
                if (!player.HasCards) { NumberOfPlayersToMulligan--; }
            }
        }
    }
}
