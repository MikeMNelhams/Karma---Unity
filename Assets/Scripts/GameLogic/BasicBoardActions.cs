using DataStructures;
using Karma.Board;
using Karma.Cards;
using Karma.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Karma
{
    namespace BasicBoard
    {
        public class PickupPlayPile : BoardPlayerAction
        {
            public PickupPlayPile() { }
            public override string Name { get => "pickup"; }
            public override bool IsValid(IBoard board)
            {
                if (!board.CurrentPlayer.HasCards) { return false; }
                return board.PlayPile.Count > 0;
            }
            public override BoardPlayerAction Copy()
            {
                return new PickupPlayPile();
            }

            public override void Apply(IBoard board, IController controller)
            {
                board.CurrentPlayer.Pickup(board.PlayPile);
                board.EffectMultiplier = 1;
            }
        }

        public class PlayCardsCombo : BoardPlayerAction
        {
            public PlayCardsCombo() { }

            public override void Apply(IBoard board, IController controller)
            {
                FrozenMultiSet<CardValue> cards = null;
                Func<IBoard, FrozenMultiSet<CardValue>> cardSelector = controller.SelectCardsToPlay;
                throw new Exception("good until here!");
                while (cards is null || !board.CurrentLegalCombos.Contains(cards))
                {
                    cards = cardSelector(board);
                    
                }
                CardsList cardsToPlay = board.CurrentPlayer.PlayableCards.Remove(cards);
                board.PlayCards(cardsToPlay, controller);
            }

            public override BoardPlayerAction Copy()
            {
                return new PlayCardsCombo();
            }

            public override string Name { get => "play_cards"; }
            public override bool IsValid(IBoard board)
            {
                if (!board.CurrentPlayer.HasCards) { return false; }
                return board.CurrentLegalCombos.Count > 0;
            }
        }
    }
}

