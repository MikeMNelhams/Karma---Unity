using Karma.BasicBoard;
using Karma.Board;
using Karma.Cards;
using Karma.Controller;
using Karma.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Karma
{
    namespace Game
    {
        public class GameWonException : Exception 
        { 
            public GameWonException(string message) : base(message) { }
            public GameWonException(Dictionary<int, int> gameRanks)
            {
                string message = "Overall Rankings: " + gameRanks;
                throw new GameWonException(message);
            }
        }

        public class GameTurnLimitExceededException : Exception
        {
            public GameTurnLimitExceededException(string message) : base(message) { }
            public GameTurnLimitExceededException(Dictionary<int, int> gameRanks, int turnLimit)
            {
                string message = "Max turn limit of " + turnLimit + " has been hit!\nGame rankings: " + gameRanks;
                throw new GameTurnLimitExceededException(message);
            }
        }

        public class Game
        {
            public IBoard Board { get; set; }
            public IController Controller { get; set; }
            public Dictionary<int, int> GameRanks { get; set; }
            protected int _turnLimit;

            public Game(IBoard startBoard, IController controller, int turnLimit = 100)
            {
                Board = startBoard;
                Controller = controller;
                _turnLimit = turnLimit;

                GameRanks = new Dictionary<int, int>();
                for (int i = 0; i < Board.Players.Count; i++)
                {
                    GameRanks[i] = Board.Players[i].Length;
                }
                Board.RegisterOnTurnEndEvent(StepOneTurn);
                Board.RegisterOnTurnEndEvent(PlayAgainIfBurnedThisTurn);
                Board.RegisterOnTurnEndEvent(CheckIfWinner);
            }

            public void Play()
            {
                int n = _turnLimit * Board.Players.Count + 1;
                for (int i = 0; i < n; i++)
                {
                    PlayTurn();
                }
            }

            public void PlayTurn()
            {
                Board.StartTurn();

                HashSet<BoardPlayerAction> actions = Board.CurrentLegalActions;

                if (actions.Count == 0) { Board.EndTurn(); return; }

                Dictionary<string, BoardPlayerAction> actionsMap = new();
                foreach (BoardPlayerAction action in actions) { actionsMap[action.Name] = action; }

                BoardPlayerAction selectedAction = Controller.SelectAction(Board);
                selectedAction.Apply(Board, Controller);
                Board.EndTurn();
            }

            void StepOneTurn(IBoard board) { board.StepPlayerIndex(1); }

            void PlayAgainIfBurnedThisTurn(IBoard board)
            {
                if (!board.Players[board.PlayerIndexWhoStartedTurn].HasCards) { return; }
                if (!board.HasBurnedThisTurn) { return; }
                board.CurrentPlayerIndex = board.PlayerIndexWhoStartedTurn;
                PlayTurn();
            }

            void CheckIfWinner(IBoard board)
            {
                UpdateGameRanks(board);
                int numberOfPotentialWinners = board.PotentialWinnerIndices.Count;
                if (numberOfPotentialWinners == 1 && board.NumberOfJokersInPlay == 0)
                {
                    throw new GameWonException(GameRanks);
                }
                if (numberOfPotentialWinners >= 2 && board.NumberOfJokersInPlay == 0)
                {
                    throw new GameWonException(GameRanks);
                }
                if (numberOfPotentialWinners >= 2)
                {
                    VoteForWinners(board);
                    throw new GameWonException(GameRanks);
                }

                if (board.TurnsPlayed >= _turnLimit)
                {
                    throw new GameTurnLimitExceededException(GameRanks, _turnLimit);
                }
            }

            void UpdateGameRanks(IBoard board)
            {
                Dictionary<int, HashSet<int>> cardCounts = new();
                for (int i = 0; i < board.Players.Count; i++)
                {
                    if (!cardCounts.ContainsKey(board.Players[i].Length)) { cardCounts[board.Players[i].Length] = new HashSet<int>(); }
                    cardCounts[board.Players[i].Length].Add(i);
                }

                List<Tuple<int, HashSet<int>>> ranks = new();
                foreach (int key in cardCounts.Keys)
                {
                    HashSet<int> playerIndices = cardCounts[key];
                    ranks.Add(Tuple.Create(key, playerIndices));
                }
                ranks.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                GameRanks = new();
                foreach ((int rank, HashSet<int> pair) in ranks)
                {
                    foreach (int playerIndex in pair)
                    {
                        GameRanks[playerIndex] = rank;
                    }
                }
            }

            void VoteForWinners(IBoard board)
            {
                Dictionary<int, int> votes = new();
                Dictionary<int, int> jokerCounts = new();
                for (int i = 0; i < board.Players.Count; i++)
                {
                    Player player = board.Players[i];
                    int jokerCount = player.NumberOfJokers;
                    if (jokerCount > 0)
                    {
                        jokerCounts[i] = jokerCount;
                    }
                }
                HashSet<int> playerIndicesToExclude = new ();
                playerIndicesToExclude.UnionWith(Enumerable.Range(0, board.Players.Count).ToList<int>());
                playerIndicesToExclude.ExceptWith(board.PotentialWinnerIndices);
                foreach (int playerIndex in jokerCounts.Keys) 
                {
                    int numberOfVotes = jokerCounts[playerIndex];
                    board.CurrentPlayerIndex = playerIndex;
                    int voteTargetIndex = Controller.VoteForWinner(playerIndicesToExclude);
                    if (!votes.ContainsKey(voteTargetIndex)) { votes[voteTargetIndex] = 0; }
                    votes[voteTargetIndex] += numberOfVotes;
                }

                if (votes.Count > 0)
                {
                    int mostVotes = Enumerable.Max(votes.Values);
                    List<int> mostVotedPlayerIndices = new();
                    foreach (int playerIndex in votes.Keys)
                    {
                        if (votes[playerIndex] == mostVotes)
                        {
                            mostVotedPlayerIndices.Add(playerIndex);
                        }
                    }
                    HashSet<int> loserIndices = new();
                    loserIndices.UnionWith(Enumerable.Range(0, board.Players.Count));
                    loserIndices.ExceptWith(mostVotedPlayerIndices);
                    foreach (int playerIndex in loserIndices)
                    {
                        GameRanks[playerIndex]++;
                    }
                }
            }
        }
    }
}

