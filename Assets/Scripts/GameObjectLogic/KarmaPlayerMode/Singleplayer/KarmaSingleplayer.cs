using KarmaLogic.Board;
using KarmaLogic.Cards;
using StateMachine.CharacterStateMachines;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using KarmaLogic.BasicBoard;

namespace KarmaPlayerMode
{
    namespace Singleplayer
    {
        public class KarmaSingleplayer : KarmaPlayerMode
        {

            public KarmaSingleplayer(BasicBoardParams basicBoardParams = null) : base(basicBoardParams)
            {
            }

            public KarmaSingleplayer(int basicBoardPreset) : base(basicBoardPreset)
            {
            }

            public override async void SetupPlayerActionStateForBasicStart()
            {
                for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
                {
                    PlayerProperties playerProperties = PlayersProperties[playerIndex];
                    if (!Board.Players[playerIndex].HasCards)
                    {
                        await playerProperties.ProcessStateCommand(Command.HasNoCards);
                        continue;
                    }
                    await playerProperties.ProcessStateCommand(Command.MulliganStarted);
                }
            }

            public override async void SetupPlayerActionStatesForVotingForWinner()
            {
                for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
                {
                    PlayerProperties playerProperties = PlayersProperties[playerIndex];
                    if (!Board.Players[playerIndex].HasCards)
                    {
                        await playerProperties.ProcessStateCommand(Command.HasNoCards);
                        continue;
                    }

                    bool playerHasVotes = Board.Players[playerIndex].CountValue(CardValue.JOKER) > 0;
                    if (Board.Players[playerIndex].HasCards && !playerHasVotes)
                    {
                        await playerProperties.ProcessStateCommand(Command.TurnEnded);
                    }
                }

                bool setFirstVotingPlayerForDebugMode = false;

                for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
                {
                    PlayerProperties playerProperties = PlayersProperties[playerIndex];
                    bool playerHasVotes = Board.Players[playerIndex].CountValue(CardValue.JOKER) > 0;

                    if (!playerHasVotes) { continue; }

                    if (!setFirstVotingPlayerForDebugMode)
                    {
                        setFirstVotingPlayerForDebugMode = true;
                        await playerProperties.ProcessStateCommand(Command.VotingStarted);
                    }
                    else
                    {
                        await playerProperties.ProcessStateCommand(Command.TurnEnded);
                    }
                }
            }

            public override void SetupPlayerMovementControllers()
            {
                for (int i = 0; i < Board.Players.Count; i++)
                {
                    PlayerProperties playerProperties = PlayersProperties[i];
                    if (!IsPlayableCharacter(i)) { continue; }
                    if (IsGameWonWithVoting) { playerProperties.EnablePlayerMovement(); }
                }

                PlayersProperties[Board.CurrentPlayerIndex].EnablePlayerMovement();
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
                await PlayersProperties[votingPlayerIndex].ProcessStateCommand(Command.GameEnded);
                EnableNextPlayableCamera(voteTargetIndex, IsWaitingTurn);
                Board.EndTurn();
            }

            public void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement = null)
            {
                if (NumberOfActivePlayers <= 1) { return; }

                UnityEngine.Debug.LogError("Should be IMPOSSIBLE to get here from Singleplayer!");

                for (int playerIndex = playerCameraDisabledIndex + 1; playerIndex < Board.Players.Count; playerIndex++)
                {
                    if (!IsPlayableCharacter(playerIndex)) { continue; }
                    if (stateRequirement != null && !stateRequirement(PlayersProperties[playerIndex].StateMachine.CurrentState)) { continue; }
                    PlayersProperties[playerIndex].EnableCamera();
                    return;
                }

                for (int playerIndex = 0; playerIndex < playerCameraDisabledIndex; playerIndex++)
                {
                    if (!IsPlayableCharacter(playerIndex)) { continue; }
                    if (stateRequirement != null && !stateRequirement(PlayersProperties[playerIndex].StateMachine.CurrentState)) { continue; }
                    PlayersProperties[playerIndex].EnableCamera();
                    return;
                }
            }

            public override async Task VoteForWinners()
            {
                UpdatePlayerJokerCounts();
                UpdateValidTargetPlayersForWinVoting();

                HashSet<int> playerIndicesToExclude = new();
                playerIndicesToExclude.UnionWith(Enumerable.Range(0, Board.Players.Count).ToList<int>());
                playerIndicesToExclude.ExceptWith(Board.PotentialWinnerIndices);

                int firstValidIndex = PlayerJokerCounts.Keys.Min(); // Where(x => x > Board.CurrentPlayerIndex)
                PlayersProperties[firstValidIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
                await PlayersProperties[firstValidIndex].ProcessStateCommand(Command.VotingStarted);
                return;
            }

            public override async Task TriggerFinishMulligan(int playerIndex)
            {
                await PlayersProperties[playerIndex].ProcessStateCommand(Command.TurnEnded);
                NumberOfPlayersFinishedMulligan++;
                if (!IsMulliganFinished)
                {
                    EnableNextPlayableCamera(playerIndex, IsWaitingTurn);
                    Board.StepPlayerIndex(1);
                    await PlayersProperties[Board.CurrentPlayerIndex].ProcessStateCommand(Command.MulliganStarted);
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

            public override void IfPlayableDisableStartingPlayerMovement()
            {
                if (IsPlayableCharacter(Board.PlayerIndexWhoStartedTurn))
                {
                    PlayersProperties[Board.PlayerIndexWhoStartedTurn].DisablePlayerMovement();
                }
            }

            public override void IfPlayableEnableCurrentPlayerMovement()
            {
                if (IsPlayableCharacter(Board.PlayerIndexWhoStartedTurn))
                {
                    PlayersProperties[Board.PlayerIndexWhoStartedTurn].EnablePlayerMovement();
                }
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
                    new PlayRandomStartFourPlayable(),             // 18
                    new PlayRandomStartDefault()                   // 19
                };

                return presets;
            }
        }
    }
}
