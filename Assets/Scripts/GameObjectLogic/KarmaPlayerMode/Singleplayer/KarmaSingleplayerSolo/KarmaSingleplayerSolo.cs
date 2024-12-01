using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KarmaLogic.BasicBoard;
using KarmaLogic.Bots;
using StateMachine.CharacterStateMachines;
using UnityEngine;
using PlayTable;

namespace KarmaPlayerMode.Singleplayer
{
    public class KarmaSingleplayerSolo : KarmaSingleplayer
    {
        public KarmaSingleplayerSolo(BasicBoardParams basicBoardParams = null) : base(basicBoardParams)
        {
        }

        public KarmaSingleplayerSolo(int basicBoardPreset) : base(basicBoardPreset)
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
                    playerHandler.StateMachine = new SingleplayerSoloStateMachine(playerHandler);
                    playerHandler.SetCardLegalityHinter(AreLegalHintsEnabled(playerIndex));
                    playerHandler.EnableCamera();
                }
                else
                {
                    string botName = "Bot " + botNameIndex;
                    IntegrationTestBot bot = new(botName, botDelay);
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
            Board.EndTurn();
        }

        public override async Task TriggerFinishMulligan(int playerIndex)
        {
            await PlayerHandlers[playerIndex].ProcessStateCommand(Command.TurnEnded);
            NumberOfPlayersFinishedMulligan++;
            if (!IsMulliganFinished)
            {
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
                new TestStartQueenCombo(),                     // 0
                new TestStartJokerCombo(),                     // 1
                new TestStartVoting(),                         // 2
                new TestStartVoting2(),                        // 3
                new TestScenarioFullHand(),                    // 4
                new TestLeftHandRotate(),                      // 5
                new TestGameWonNoVoting(),                     // 6 
                new TestPotentialWinnerIsSkippedInUnwonGame(), // 7 
                new TestMultipleSeparateCardGiveaways(),       // 8
                new TestQueenComboLastCardToWin(),             // 9 
                new TestQueenComboLastCardWithJokerInPlay(),   // 10
                new TestValidJokerAsLastCardToWin(),           // 11
                new TestGettingJokered(),                      // 12
                new TestJokerAsAceLastCardToWin(),             // 13
                new TestAllPlayersNoActionsGameEnds(),         // 14
                new TestAceNoHandDoesNotCrash(),               // 15 
                new TestAceAndFive(),                          // 16 No more deterministic test cases past this point!
                new TestRandomStart(),                         // 17 
                new PlayRandomStartDefault()                   // 18
            };

            return presets;
        }
    }
}

