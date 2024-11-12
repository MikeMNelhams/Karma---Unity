using KarmaLogic.Board;
using KarmaLogic.Cards;
using StateMachines.CharacterStateMachines;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace KarmaPlayerMode
{
    public class KarmaSingleplayer : KarmaPlayerMode
    {
        public KarmaSingleplayer(KarmaPlayerStartInfo[] playersStartInfo, IBoard board, List<PlayerProperties> playersProperties, int turnLimit = 100) : base(playersStartInfo, board, playersProperties, turnLimit)
        {
            
        }

        public override int NumberOfActivePlayers { get => 1; }

        public override async void SetupPlayerActionStateForBasicStart()
        {
            // TODO the Command.HasNoCards and Command.TurnEnded can all be Task.WhenAll() awaited for minor startup performance improvement (might save a frame or two)

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
            for (int i = 0; i < PlayersStartInfo.Length; i++)
            {
                PlayerProperties playerProperties = PlayersProperties[i];
                if (!PlayersStartInfo[i].isPlayableCharacter) { continue; }

                if (IsGameWonWithVoting) { playerProperties.EnablePlayerMovement(); }
            }

            PlayersProperties[Board.CurrentPlayerIndex].EnablePlayerMovement();
        }

        public override void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement = null)
        {
            if (NumberOfActivePlayers <= 1) { return; }

            for (int playerIndex = playerCameraDisabledIndex + 1; playerIndex < Board.Players.Count; playerIndex++)
            {
                if (!PlayersStartInfo[playerIndex].isPlayableCharacter) { continue; }
                if (stateRequirement != null && !stateRequirement(PlayersProperties[playerIndex].StateMachine.CurrentState)) { continue; }
                PlayersProperties[playerIndex].EnableCamera();
                return;
            }

            for (int playerIndex = 0; playerIndex < playerCameraDisabledIndex; playerIndex++)
            {
                if (!PlayersStartInfo[playerIndex].isPlayableCharacter) { continue; }
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

        public override void IfDebugModeDisableStartingPlayerMovement()
        {
            if (PlayersStartInfo[Board.PlayerIndexWhoStartedTurn].isPlayableCharacter)
            {
                PlayersProperties[Board.PlayerIndexWhoStartedTurn].DisablePlayerMovement();
            }
        }

        public override void IfDebugModeEnableCurrentPlayerMovement()
        {
            if (PlayersStartInfo[Board.PlayerIndexWhoStartedTurn].isPlayableCharacter)
            {
                PlayersProperties[Board.PlayerIndexWhoStartedTurn].EnablePlayerMovement();
            }
        }
    }
}
