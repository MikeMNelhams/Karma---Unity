using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Bots;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using System.Linq;
using KarmaLogic.BasicBoard;
using DataStructures;
using System;

namespace KarmaLogic.Bots
{
    public class DecentlyFunBot : IBot
    {
        public string Name { get; protected set; }
        public float DelaySeconds { get; protected set; }
        protected List<BoardPlayerAction> _knownActions = new();

        public DecentlyFunBot(string name, float delay)
        {
            Name = name;
            DelaySeconds = delay;
            _knownActions.Add(new PickupPlayPile());
            _knownActions.Add(new PlayCardsCombo());
        }

        public int CardGiveAwayIndex(IBoard board)
        {
            CardsList playableCards = board.CurrentPlayer.PlayableCards;
            List<CardValue> cardValues = playableCards.CardValues;
            HashSet<int> legalIndices = new();
            for (int i = 0; i < cardValues.Count; i++)
            {
                CardValue cardValue = cardValues[i];
                if (cardValue != CardValue.JOKER)
                {
                    legalIndices.Add(i);
                }
            }
            return legalIndices.First();
        }

        public int CardPlayerGiveAwayIndex(IBoard board, HashSet<int> excludedPlayerIndices)
        {
            HashSet<int> legalIndices = OtherPlayerIndices(board);
            legalIndices.ExceptWith(excludedPlayerIndices);
            return legalIndices.First();
        }

        public FrozenMultiSet<CardValue> ComboToPlay(IBoard board)
        {
            LegalCombos legalCombos = board.CurrentLegalCombos;
            if (legalCombos.CardValues.Contains(CardValue.JOKER))
            {
                return LargestCombo(legalCombos, (x) => (x.Contains(CardValue.JOKER)));
            }

            if (legalCombos.CardValues.Contains(CardValue.TEN))
            {
                return LargestCombo(legalCombos, (x) => (x.Contains(CardValue.TEN)));
            }

            FrozenMultiSet<CardValue> largestCombo = LargestCombo(legalCombos);
            if (largestCombo.TotalCount >= 4) { return largestCombo; }

            if (legalCombos.CardValues.Contains(CardValue.QUEEN))
            {
                return LargestCombo(legalCombos, (x) => (x.Contains(CardValue.QUEEN)));
            }

            return largestCombo;
        }

        FrozenMultiSet<CardValue> LargestCombo(LegalCombos legalCombos, Func<FrozenMultiSet<CardValue>, bool> comboRequirement = null)
        {
            // Assumes that at least ONE combo matches the given requirement!
            List<FrozenMultiSet<CardValue>> combos = legalCombos.Combos.ToList();

            int maxIndex = -1;
            int maxValue = 0;

            if (comboRequirement != null)
            {
                for (int i = 0; i < combos.Count; i++)
                {
                    FrozenMultiSet<CardValue> combo = combos[i];
                    int count = combo.TotalCount;
                    if (count > maxValue && comboRequirement(combo))
                    {
                        maxValue = count;
                        maxIndex = i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < combos.Count; i++)
                {
                    int count = combos[i].TotalCount;
                    if (count > maxValue)
                    {
                        maxValue = count;
                        maxIndex = i;
                    }
                }
            }

            return combos[maxIndex];
        }

        public int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices)
        {
            HashSet<int> potentialWinnerIndices = board.PotentialWinnerIndices;
            potentialWinnerIndices.ExceptWith(excludedPlayerIndices);
            if (potentialWinnerIndices.Count > 0)
            {
                return potentialWinnerIndices.ToList<int>()[0];
            }
            HashSet<int> otherPlayerIndices = OtherPlayerIndices(board);
            otherPlayerIndices.ExceptWith(excludedPlayerIndices);
            return otherPlayerIndices.ToList<int>()[0];
        }

        public int MulliganHandIndex(IBoard board)
        {
            CardsList hand = board.CurrentPlayer.Hand;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Value != CardValue.JOKER)
                {
                    return i;
                }
            }

            throw new Exception("DecentlyFunBot should only be doing mulligan to rotate karmaUp jokers to their hand, but their entire hand is jokers!");
        }

        public int MulliganKarmaUpIndex(IBoard board)
        {
            CardsList karmaUp = board.CurrentPlayer.KarmaUp;
            for (int i = 0; i < karmaUp.Count; i++)
            {
                if (karmaUp[i].Value == CardValue.JOKER)
                {
                    return i;
                }
            }

            throw new Exception("DecentlyFunBot should only be doing mulligan to rotate karmaUp jokers to their hand, but their karmaUp has no jokers!");
        }

        public BoardTurnOrder PreferredStartDirection(IBoard board)
        {
            return BoardTurnOrder.RIGHT;
        }

        public BoardPlayerAction SelectAction(IBoard board)
        {
            foreach (BoardPlayerAction action in _knownActions)
            {
                if (board.CurrentLegalActions.Contains(_knownActions[1]))
                {
                    return _knownActions[1];
                }

                if (board.CurrentLegalActions.Contains(action))
                {
                    return action;
                }
            }

            throw new NoValidBoardPlayerActionsException();
        }

        public int VoteForWinnerIndex(IBoard board, HashSet<int> excludedPlayerIndices)
        {
            HashSet<int> potentialWinners = board.PotentialWinnerIndices;
            potentialWinners.ExceptWith(excludedPlayerIndices);
            List<int> potentialWinnerIndices = potentialWinners.ToList<int>();
            return potentialWinnerIndices[0];
        }

        public bool WantsToMulligan(IBoard board)
        {
            bool handIsNotOnlyJokers = board.CurrentPlayer.Hand.CardValues.Where(x => x == CardValue.JOKER).Count() != board.CurrentPlayer.Hand.Count;
            return board.CurrentPlayer.KarmaUp.CardValues.Contains(CardValue.JOKER) && handIsNotOnlyJokers;
        }

        HashSet<int> OtherPlayerIndices(IBoard board)
        {
            HashSet<int> otherPlayers = Enumerable.Range(0, board.Players.Count).ToHashSet();
            otherPlayers.Remove(board.CurrentPlayerIndex);
            return otherPlayers;
        }
    }
}