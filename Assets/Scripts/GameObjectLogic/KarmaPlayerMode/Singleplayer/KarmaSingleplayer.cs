using KarmaLogic.Board;
using KarmaLogic.Cards;
using StateMachine.CharacterStateMachines;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using KarmaLogic.BasicBoard;
using KarmaLogic.Bots;
using UnityEngine;
using PlayTable;

namespace KarmaPlayerMode.Singleplayer
{
    public abstract class KarmaSingleplayer : KarmaPlayerMode
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
                PlayerHandler playerProperties = PlayerHandlers[playerIndex];
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

            bool setFirstVotingPlayerForDebugMode = false;

            for (int playerIndex = 0; playerIndex < Board.Players.Count; playerIndex++)
            {
                PlayerHandler playerProperties = PlayerHandlers[playerIndex];
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

        public sealed override void SetupPlayerMovementControllers()
        {
            for (int i = 0; i < Board.Players.Count; i++)
            {
                PlayerHandler playerProperties = PlayerHandlers[i];
                if (!IsPlayableCharacter(i)) { continue; }
                if (IsGameWonWithVoting) { playerProperties.EnablePlayerMovement(); }
            }

            PlayerHandlers[Board.CurrentPlayerIndex].EnablePlayerMovement();
        }

        public override async Task VoteForWinners()
        {
            UpdatePlayerJokerCounts();
            UpdateValidTargetPlayersForWinVoting();

            HashSet<int> playerIndicesToExclude = new();
            playerIndicesToExclude.UnionWith(Enumerable.Range(0, Board.Players.Count).ToList<int>());
            playerIndicesToExclude.ExceptWith(Board.PotentialWinnerIndices);

            int firstValidIndex = PlayerJokerCounts.Keys.Min(); // Where(x => x > Board.CurrentPlayerIndex)
            PlayerHandlers[firstValidIndex].RegisterVoteForTargetEventListener(TriggerVoteForPlayer);
            await PlayerHandlers[firstValidIndex].ProcessStateCommand(Command.VotingStarted);
            return;
        }

        public override void IfPlayableDisableStartingPlayerMovement()
        {
            if (IsPlayableCharacter(Board.PlayerIndexWhoStartedTurn))
            {
                PlayerHandlers[Board.PlayerIndexWhoStartedTurn].DisablePlayerMovement();
            }
        }

        public override void IfPlayableEnableCurrentPlayerMovement()
        {
            if (IsPlayableCharacter(Board.PlayerIndexWhoStartedTurn))
            {
                PlayerHandlers[Board.PlayerIndexWhoStartedTurn].EnablePlayerMovement();
            }
        }
    }
}
