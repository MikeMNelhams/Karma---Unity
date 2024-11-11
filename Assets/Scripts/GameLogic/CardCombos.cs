using System.Collections;
using System.Collections.Generic;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using System;
using DataStructures;


namespace KarmaLogic
{
    namespace CardCombos
    {
        public class CardCombo_TWO : CardCombo
        {
            public CardCombo_TWO(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
            public override void Apply(IBoard board)
            {
                board.ResetPlayOrder();
                TriggerOnFinishApplyComboListeners();
            }
        }

        public class CardCombo_THREE : CardCombo
        {
            public CardCombo_THREE(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
            public override void Apply(IBoard board)
            {
                board.EffectMultiplier *= (int)Math.Pow(2, Cards.Count);
                TriggerOnFinishApplyComboListeners();
            }
        }

        public class CardCombo_FOUR : CardCombo
        {
            public CardCombo_FOUR(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
            public override void Apply(IBoard board)
            {
                TriggerOnFinishApplyComboListeners();
                return;
            }
        }

            public class CardCombo_FIVE : CardCombo
            {
                public CardCombo_FIVE(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    List<Hand> startHands = new();
                    foreach (Player player in board.Players)
                    {
                        startHands.Add(player.Hand);
                    }
                    Deque<Hand> hands = new(startHands);
                    int numberOfRepeats = Cards.Count * board.EffectMultiplier;

                    if (numberOfRepeats < board.Players.Count)
                    {
                        board.RotateHands(numberOfRepeats, hands);
                        TriggerOnFinishApplyComboListeners();
                        return;
                    }
                    board.RotateHands(board.Players.Count, hands);
                    board.RotateHands(numberOfRepeats % board.Players.Count, hands);
                    TriggerOnFinishApplyComboListeners();
                    return;
                }
            }

            public class CardCombo_SIX : CardCombo
            {
                public CardCombo_SIX(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    TriggerOnFinishApplyComboListeners();
                    return;
                }
            }

            public class CardCombo_SEVEN : CardCombo
            {
                public CardCombo_SEVEN(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    if (((Cards.Count * (uint)board.EffectMultiplier) & 0b1) == 0b0)
                    {
                        TriggerOnFinishApplyComboListeners();
                        return;
                    }
                    board.FlipPlayOrder();
                    TriggerOnFinishApplyComboListeners();
                }
            }

            public class CardCombo_EIGHT : CardCombo
            {
                public CardCombo_EIGHT(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    if (((Cards.Count * (uint)board.EffectMultiplier) & 0b1) == 0b0)
                    {
                        TriggerOnFinishApplyComboListeners();
                        return;
                    }
                    board.FlipTurnOrder();
                    TriggerOnFinishApplyComboListeners();
                }
            }

            public class CardCombo_NINE : CardCombo
            {
                public CardCombo_NINE(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    if (board.PlayPile.ContainsMinLengthRun(4)) { TriggerOnFinishApplyComboListeners(); return; }
                    int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                    board.StepPlayerIndex(numberOfRepeats);
                    TriggerOnFinishApplyComboListeners();
                }
            }

            public class CardCombo_TEN : CardCombo
            {
                public CardCombo_TEN(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    board.Burn(0);
                    TriggerOnFinishApplyComboListeners();
                }
            }

            public class CardCombo_JACK : CardCombo
            {
                public CardCombo_JACK(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    if (board.PlayPile.Count <= Cards.Count) { TriggerOnFinishApplyComboListeners(); return; }

                    Card cardBelowCombo = board.PlayPile[^(1 + Cards.Count)];
                    if (cardBelowCombo.Value == CardValue.JACK) { TriggerOnFinishApplyComboListeners(); return; }
                    int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                    if (cardBelowCombo.Value == CardValue.THREE) { numberOfRepeats = Cards.Count; }
                    if (cardBelowCombo.Value != CardValue.THREE && cardBelowCombo.Value != CardValue.JACK) { board.EffectMultiplier = 1; }

                    CardsList cardsToReplay = CardsList.Repeat(cardBelowCombo, numberOfRepeats);
                    board.EventSystem.RegisterOnFinishPlaySuccesfulComboListener(TriggerOnFinishApplyComboListeners);
                    board.PlayCards(cardsToReplay, false);
                }
            }

            public class CardCombo_QUEEN : CardCombo
            {
                public CardCombo_QUEEN(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
                public override void Apply(IBoard board)
                {
                    Player currentPlayer = board.CurrentPlayer;
                    if (board.StartingPlayerStartedPlayingFrom == PlayingFrom.KarmaUp && currentPlayer.KarmaUp.Count == 0) { TriggerOnFinishApplyComboListeners();  return; }
                    if (InvalidFilter(currentPlayer)) { TriggerOnFinishApplyComboListeners(); return; }
                    if (!ValidTargetPlayersExist(board)) { TriggerOnFinishApplyComboListeners();  return; }

                    int numberOfRepeats = Cards.Count * board.EffectMultiplier;

                    board.EventSystem.RegisterOnFinishCardGiveAwayListener(TriggerOnFinishApplyComboListeners);
                    board.StartGivingAwayCards(numberOfRepeats, InvalidFilter);
                }

                bool InvalidFilter(Player giver)
                {
                    if (!giver.HasCards) { return true; }
                    if (giver.PlayableCards.IsExclusively(CardValue.JOKER)) { return true; }
                    return false;
                }

                bool ValidTargetPlayersExist(IBoard board)
                {
                    for (int i = 0; i < board.Players.Count; i++)
                    {
                        if (i == board.CurrentPlayerIndex) { continue; }
                        if (board.Players[i].HasCards) { return true; }
                    }
                    return false;
                }
            }

        public class CardCombo_KING : CardCombo
        {
            public CardCombo_KING(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
            public override void Apply(IBoard board)
            {
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                // This way K -> K, K, K ends the combo dead in its tracks. 52x the King effect over and over is a nightmare...
                if (board.PlayPile.ContainsMinLengthRun(4)) { board.Burn(0); return; }
                numberOfRepeats = Math.Min(numberOfRepeats, board.BurnPile.Count);
                if (numberOfRepeats == 0) { return; }
                CardsList cardsToPlay = board.BurnPile.RemoveFromBottom(numberOfRepeats);
                foreach (Card card in cardsToPlay)
                {
                    board.CardValuesInPlayCounts[card.Value]++;
                    board.EventSystem.RegisterOnFinishCardGiveAwayListener(TriggerOnFinishApplyComboListeners);
                    board.PlayCards(new CardsList(card));
                }
            }
        }

        public class CardCombo_ACE : CardCombo
        {
            public CardCombo_ACE(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
            public override void Apply(IBoard board)
            {
                int numberOfRepeats = Cards.Count * board.EffectMultiplier;
                if (((uint)(numberOfRepeats) & 0b1) == 0b0)
                {
                    board.FlipHands();
                    board.FlipHands();
                    TriggerOnFinishApplyComboListeners();
                    return;
                }

                board.FlipHands();
                TriggerOnFinishApplyComboListeners();
            }
        }

        public class CardCombo_JOKER : CardCombo
        {
            public CardCombo_JOKER(CardsList cards, Dictionary<CardValue, int> counts) : base(cards, counts) { }
            public override void Apply(IBoard board)
            {
                board.Burn(Cards.Count);
                if (board.PlayPile.Count == 0) { return; }

                board.StartGivingAwayPlayPile();
            }
        }
    }
}
