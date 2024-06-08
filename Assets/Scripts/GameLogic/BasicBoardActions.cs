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
            CardsList _cards = null;
            protected Func<CardsList> _cardsGetter;

            public PlayCardsCombo(Func<CardsList> cardsGetter)
            {
                _cardsGetter = cardsGetter;
            }

            public override void Apply(IBoard board, IController controller)
            {
                while (_cards is null || !board.CurrentLegalCombos.Contains(new FrozenMultiSet<CardValue>(_cards.CardValues)))
                {
                    GetCards();
                }
                CardsList cardsToPlay = board.CurrentPlayer.PlayableCards.Remove(_cards);
                board.PlayCards(cardsToPlay, controller);
            }

            public override BoardPlayerAction Copy()
            {
                return new PlayCardsCombo(_cardsGetter);
            }

            public CardsList Cards
            {
                get
                {
                    if (_cards is null)
                    {
                        GetCards();
                    }
                    return _cards;
                }
            }

            void GetCards() 
            {
                _cards = _cardsGetter();
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

