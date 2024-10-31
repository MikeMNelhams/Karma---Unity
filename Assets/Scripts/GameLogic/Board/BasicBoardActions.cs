using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KarmaLogic
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

            public override void Apply(IBoard board, Controller.Controller controller, CardsList selectedCards)
            {
                board.CurrentPlayer.Pickup(board.PlayPile);
                board.EffectMultiplier = 1;
            }
        }

        public class PlayCardsCombo : BoardPlayerAction
        {
            public PlayCardsCombo() { }

            public override void Apply(IBoard board, Controller.Controller controller, CardsList selectedCards)
            {
                CardsList cardsToPlay = board.CurrentPlayer.PlayableCards.Remove(selectedCards);
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

