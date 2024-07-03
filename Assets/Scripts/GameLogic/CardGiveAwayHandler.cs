using System.Collections;
using System.Collections.Generic;
using Karma.Board;
using Karma.Cards;

namespace Karma
{
    namespace Players
    {
        public class CardGiveAwayHandler
        {
            protected int _numberOfCardsToGiveAway;
            protected int _giverIndex;
            readonly Player _giver;
            
            protected PlayingFrom _playingFromAtStart;

            public delegate void OnCardGiveAwayListener(Card card, int giverIndex, int receiverIndex);
            event OnCardGiveAwayListener CardGiveAway;
            readonly List<OnCardGiveAwayListener> _registeredListeners;

            public bool IsFinished { get => _numberOfCardsToGiveAway == 0; }
            public int NumberOfCardsRemainingToGiveAway { get => _numberOfCardsToGiveAway; }

            public CardGiveAwayHandler(int NumberOfCardsToGiveAway, IBoard board, int giverIndex)
            {
                _numberOfCardsToGiveAway = NumberOfCardsToGiveAway;
                _giverIndex = giverIndex;
                _giver = board.Players[giverIndex];
                _playingFromAtStart = _giver.PlayingFrom;

                _registeredListeners = new();
            }

            public void GiveAway(Card card, int receiverIndex)
            {
                if (_numberOfCardsToGiveAway <= 0) { throw new AlreadyCompletedGiveAwayException(); }

                if (_giver.PlayingFrom != _playingFromAtStart) { EndGiveAway(); return; }
                if (_giver.PlayableCards.IsExclusively(CardValue.JOKER)) { EndGiveAway(); return; }

                _numberOfCardsToGiveAway -= 1;
                CardGiveAway?.Invoke(card, _giverIndex, receiverIndex);
                if (!_giver.HasCards) { EndGiveAway(); return; }
                if (_numberOfCardsToGiveAway == 0) { RemoveAllListeners(); } 
            }

            public void RegisterOnCardGiveAwayListener(OnCardGiveAwayListener listener)
            {
                CardGiveAway += listener;
                _registeredListeners.Add(listener);
            }

            void EndGiveAway()
            {
                _numberOfCardsToGiveAway = 0;
                RemoveAllListeners();
            }

            void RemoveAllListeners()
            {
                foreach (OnCardGiveAwayListener listener in _registeredListeners)
                {
                    CardGiveAway -= listener;
                }
                _registeredListeners.Clear();
            }
        }

        public class AlreadyCompletedGiveAwayException : System.Exception
        {
            public AlreadyCompletedGiveAwayException(string message) : base(message) { }
            public AlreadyCompletedGiveAwayException()
            {
                string message = "Could not give away, the player has already given away the required cards!\n Create a new PlayerCardGiveAwayHandler(X) to begin giving away cards";
                throw new AlreadyCompletedGiveAwayException(message);
            }
        }
    }
}
