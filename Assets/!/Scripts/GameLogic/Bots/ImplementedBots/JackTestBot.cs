using System.Collections.Generic;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using System.Linq;
using KarmaLogic.BasicBoard;
using DataStructures;

namespace KarmaLogic.Bots
{
    public class JackTestBot : BotBase
    {
        protected List<BoardPlayerAction> _knownActions = new();

        public JackTestBot(string name, float delay) : base(name, delay)
        {
            _knownActions.Add(new PickupPlayPile());
            _knownActions.Add(new PlayCardsCombo());
        }

        public override int CardGiveAwayIndex(IBoard board)
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

        public override int CardPlayerGiveAwayIndex(IBoard board, HashSet<int> excludedPlayerIndices)
        {
            HashSet<int> legalIndices = OtherPlayerIndices(board);
            legalIndices.ExceptWith(excludedPlayerIndices);
            return legalIndices.First();
        }

        public override FrozenMultiSet<CardValue> ComboToPlay(IBoard board)
        {
            LegalCombos legalCombos = board.CurrentLegalCombos;

            FrozenMultiSet<CardValue> jackCombo = new() { CardValue.JACK };

            if (legalCombos.Contains(jackCombo))
            {
                return jackCombo;
            }

            return legalCombos.First();
        }

        public override int JokerTargetIndex(IBoard board, HashSet<int> excludedPlayerIndices)
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

        public override int MulliganHandIndex(IBoard board)
        {
            throw new System.NotImplementedException();
        }

        public override int MulliganKarmaUpIndex(IBoard board)
        {
            throw new System.NotImplementedException();
        }

        public override BoardTurnOrder PreferredStartDirection(IBoard board)
        {
            return BoardTurnOrder.RIGHT;
        }

        public override BoardPlayerAction SelectAction(IBoard board)
        {
            // If can play JACK, play it
            if (board.CurrentLegalActions.Contains(_knownActions[1]) && board.CurrentLegalCombos.Contains(new FrozenMultiSet<CardValue>() { CardValue.JACK }))
            {
                return _knownActions[1];
            }

            foreach (BoardPlayerAction action in _knownActions)
            {
                if (board.CurrentLegalActions.Contains(action))
                {
                    return action;
                }
            }

            throw new NoValidBoardPlayerActionsException();
        }

        public override int VoteForWinnerIndex(IBoard board, HashSet<int> excludedPlayerIndices)
        {
            HashSet<int> potentialWinners = board.PotentialWinnerIndices;
            potentialWinners.ExceptWith(excludedPlayerIndices);
            List<int> potentialWinnerIndices = potentialWinners.ToList<int>();
            return potentialWinnerIndices[0];
        }

        public override bool WantsToMulligan(IBoard board)
        {
            return false;
        }

        HashSet<int> OtherPlayerIndices(IBoard board)
        {
            HashSet<int> otherPlayers = Enumerable.Range(0, board.Players.Count).ToHashSet();
            otherPlayers.Remove(board.CurrentPlayerIndex);
            return otherPlayers;
        }
    }
}