using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KarmaLogic.BasicBoard;
using KarmaLogic.Bots;
using StateMachine.CharacterStateMachines;
using PlayTable;

namespace KarmaPlayerMode.Singleplayer
{
    public class KarmaSingleplayerMany : KarmaSingleplayer
    {
        public KarmaSingleplayerMany(BasicBoardParams basicBoardParams = null) : base(basicBoardParams)
        {
        }

        public KarmaSingleplayerMany(int basicBoardPreset) : base(basicBoardPreset)
        {
        }

        protected override void CreatePlayerObjects()
        {
            PlayerHandlers = new();
            PlayersBoardHolderHandlers = new();
            int botNameIndex = 0;

            float botDelay = KarmaGameManager.Instance.GlobalBotDelayInSeconds;

            CircularTable tableGeometry = KarmaGameManager.Instance.PlayTableProperties.TableGeometry;
            Vector3[] playerStartPositions = tableGeometry.PlayerPositions(Board.Players.Count);

            Vector3[] holderStartPositions = tableGeometry.PlayerKarmaPositions(Board.Players.Count);
            Quaternion[] holderStartRotations = tableGeometry.PlayerKarmaRotations(Board.Players.Count, holderStartPositions);

            for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
            {
                GameObject player = KarmaGameManager.Instance.InstantiatePlayer(playerStartPositions[playerIndex]);

                PlayerHandler playerHandler = player.GetComponent<PlayerHandler>();
                player.name = "Player " + playerIndex;
                playerHandler.Index = playerIndex;
                PlayerHandlers.Add(playerHandler);

                if (IsPlayableCharacter(playerIndex))
                {
                    playerHandler.StateMachine = new SingleplayerManyStateMachine(playerHandler);
                    playerHandler.SetCardLegalityHinter(AreLegalHintsEnabled(playerIndex));
                }
                else
                {
                    string botName = "Bot " + botNameIndex;

                    BasicBoardBotParams botParams = BoardParams.CharactersParams[playerIndex] as BasicBoardBotParams;
                    BotBase bot = botParams.Bot(botName, botDelay);

                    playerHandler.StateMachine = new BotStateMachine(bot, playerHandler, Board);
                    playerHandler.name = botName;
                    playerHandler.SetCardLegalityHinter(false); // Bots should NOT have legal hints enabled
                    playerHandler.HoverTipHandler.enabled = false;
                    playerHandler.Canvas.enabled = false;
                    playerHandler.DisconnectCameraWithoutDeparenting();
                    botNameIndex++;
                }

                PlayerKarmaBoardHolderHandler boardHolderHandler = KarmaGameManager.Instance.
                    InstantiatePlayerKarmaBoardHolder(holderStartPositions[playerIndex],
                    holderStartRotations[playerIndex]);

                boardHolderHandler.name = playerHandler.name + " karmaBoard";
                boardHolderHandler.gameObject.transform.position += new Vector3(0, boardHolderHandler.HolderCuboidRenderer.bounds.extents.y / 2, 0);
                PlayersBoardHolderHandlers.Add(boardHolderHandler);
            }

            UpdateActivePlayersCount();
            UpdateNumberOfPlayersToMulligan();
        }

        public override async void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
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
            await PlayerHandlers[votingPlayerIndex].ProcessStateCommand(Command.GameEnded);
            EnableNextPlayableCamera(voteTargetIndex, IsWaitingTurn);
            Board.EndTurn();
        }

        public override async Task TriggerFinishMulligan(int playerIndex)
        {
            await PlayerHandlers[playerIndex].ProcessStateCommand(Command.TurnEnded);
            NumberOfPlayersFinishedMulligan++;
            if (!IsMulliganFinished)
            {
                EnableNextPlayableCamera(playerIndex, IsWaitingTurn);
                Board.StepPlayerIndex(1);
                await PlayerHandlers[Board.CurrentPlayerIndex].ProcessStateCommand(Command.MulliganStarted);
                Board.StartTurn();
                return;
            }

            UnityEngine.Debug.Log("Finished mulligan, it's turn time let's goooo");
            UnityEngine.Debug.Log("Starting player: " + Board.WhichPlayerStartedGame);
            if (Board.CurrentPlayerIndex != Board.WhichPlayerStartedGame)
            {
                Board.CurrentPlayerIndex = Board.WhichPlayerStartedGame;
            }

            Board.StartTurn();
        }

        protected override List<KarmaPlayModeBoardPreset<BasicBoard>> GetBasicBoardPresets()
        {
            List<KarmaPlayModeBoardPreset<BasicBoard>> presets = new()
            {
                new PlayRandomStartFourPlayable()           // 0
            };

            return presets;
        }

        async void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement = null)
        {
            for (int playerIndex = playerCameraDisabledIndex + 1; playerIndex < Board.Players.Count; playerIndex++)
            {
                if (!IsPlayableCharacter(playerIndex)) { continue; }
                if (stateRequirement != null && !stateRequirement(PlayerHandlers[playerIndex].StateMachine.CurrentState)) { continue; }
                await PlayerHandlers[playerIndex].EnableCamera();
                await PlayerHandlers[playerCameraDisabledIndex].DisconnectCameraWithoutDeparenting();
                return;
            }

            for (int playerIndex = 0; playerIndex < playerCameraDisabledIndex; playerIndex++)
            {
                if (!IsPlayableCharacter(playerIndex)) { continue; }
                if (stateRequirement != null && !stateRequirement(PlayerHandlers[playerIndex].StateMachine.CurrentState)) { continue; }
                await PlayerHandlers[playerIndex].EnableCamera();
                await PlayerHandlers[playerCameraDisabledIndex].DisconnectCameraWithoutDeparenting();
                return;
            }
        }
    }
}

