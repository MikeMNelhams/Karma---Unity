using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using KarmaLogic.BasicBoard;
using KarmaLogic.Cards;
using StateMachine.CharacterStateMachines;

namespace KarmaPlayerMode
{
    namespace Multiplayer
    {
        public class KarmaMultiplayer : KarmaPlayerMode
        {
            public KarmaMultiplayer(BasicBoardParams basicBoardParams = null) : base(basicBoardParams) { }
            public KarmaMultiplayer(int basicBoardPreset) : base(basicBoardPreset)
            {
            }

            public override void IfPlayableDisableStartingPlayerMovement()
            {
                throw new NotImplementedException();
            }

            public override void IfPlayableEnableCurrentPlayerMovement()
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
                    PlayerHandler playerProperties = PlayerHandlers[playerIndex];
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
                    PlayerHandler playerProperties = PlayerHandlers[playerIndex];
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

            public override void TriggerVoteForPlayer(int votingPlayerIndex, int voteTargetIndex)
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
                    PlayerHandlers[playerIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
                    tasks.Add(PlayerHandlers[playerIndex].ProcessStateCommand(Command.VotingStarted));
                }

                await Task.WhenAll(tasks);
            }

            protected override void CreatePlayerObjects()
            {
                throw new NotImplementedException();
            }

            protected override List<KarmaPlayModeBoardPreset<BasicBoard>> GetBasicBoardPresets()
            {
                throw new NotImplementedException();
            }
        }
    }
}
