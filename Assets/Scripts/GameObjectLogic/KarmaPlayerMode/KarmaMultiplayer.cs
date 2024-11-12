using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using StateMachines.CharacterStateMachines;

namespace KarmaPlayerMode
{
    public class KarmaMultiplayer : KarmaPlayerMode
    {
        public KarmaMultiplayer(KarmaPlayerStartInfo[] startInfo, IBoard board, List<PlayerProperties> playerProperties) : base(startInfo, board, playerProperties) 
        { 
        }

        public override int NumberOfActivePlayers { get => throw new NotImplementedException(); }

        public override void EnableNextPlayableCamera(int playerCameraDisabledIndex, Func<State, bool> stateRequirement = null)
        {
            throw new NotImplementedException();
        }

        public override void IfDebugModeDisableStartingPlayerMovement()
        {
            throw new NotImplementedException();
        }

        public override void IfDebugModeEnableCurrentPlayerMovement()
        {
            throw new NotImplementedException();
        }

        public override void SetupPlayerActionStateForBasicStart()
        {
            throw new NotImplementedException();
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

            for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
            {
                PlayerProperties playerProperties = PlayersProperties[playerIndex];
                bool playerHasVotes = Board.Players[playerIndex].CountValue(CardValue.JOKER) > 0;

                if (playerHasVotes) { await playerProperties.ProcessStateCommand(Command.VotingStarted); }
            }
        }

        public override void SetupPlayerMovementControllers()
        {
            throw new NotImplementedException();
        }

        public override Task TriggerFinishMulligan(int playerIndex)
        {
            throw new NotImplementedException();
        }

        public override async Task VoteForWinners()
        {
            List<Task> tasks = new();

            // Voting is asynchronous
            foreach (int playerIndex in PlayerJokerCounts.Keys)
            {
                Board.CurrentPlayerIndex = playerIndex;
                PlayersProperties[playerIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
                tasks.Add(PlayersProperties[playerIndex].ProcessStateCommand(Command.VotingStarted));
            }

            await Task.WhenAll(tasks);
        }
    }
}
