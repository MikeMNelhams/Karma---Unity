using KarmaLogic.Board;
using KarmaLogic.Cards;

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

            public override void Apply(IBoard board, CardsList selectedCards)
            {
                board.CurrentPlayer.Pickup(board.PlayPile);
                board.EffectMultiplier = 1;
                TriggerFinishListeners();
            }
        }

        public class PlayCardsCombo : BoardPlayerAction
        {
            public PlayCardsCombo() { }

            public override void Apply(IBoard board, CardsList selectedCards)
            {
                CardsList cardsToPlay = board.CurrentPlayer.PlayableCards.Remove(selectedCards);
                board.EventSystem.RegisterOnFinishPlaySuccesfulComboListener(TriggerFinishListeners);
                board.PlayCards(cardsToPlay);
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

