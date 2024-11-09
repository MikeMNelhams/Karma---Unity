using System;
using System.Collections;
using System.Collections.Generic;
using KarmaLogic.Board;
using KarmaLogic.Cards;

namespace KarmaLogic
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

            public delegate bool InvalidFilter(Player giver);

            readonly List<OnCardGiveAwayListener> _onGiveAwayListeners;

            public delegate void OnFinishGiveAwayListener();
            Queue<OnFinishGiveAwayListener> _onFinishGiveAwayListeners;

            readonly InvalidFilter _invalidFilter;

            public bool IsFinished { get => _numberOfCardsToGiveAway == 0; }
            public int NumberOfCardsRemainingToGiveAway { get => _numberOfCardsToGiveAway; }

            public CardGiveAwayHandler(int NumberOfCardsToGiveAway, IBoard board, int giverIndex, InvalidFilter invalidFilter = null)
            {
                _numberOfCardsToGiveAway = NumberOfCardsToGiveAway;
                _giverIndex = giverIndex;
                _giver = board.Players[giverIndex];
                _playingFromAtStart = _giver.PlayingFrom;
                _invalidFilter = invalidFilter;

                _onGiveAwayListeners = new();
                _onFinishGiveAwayListeners = new();
            }

            public void GiveAway(Card card, int receiverIndex)
            {
                if (_numberOfCardsToGiveAway <= 0) { throw new AlreadyCompletedGiveAwayException(); }

                if (_giver.PlayingFrom != _playingFromAtStart) { EndGiveAway(); return; }
                if (_invalidFilter is not null && _invalidFilter(_giver)) { EndGiveAway(); return; }

                _numberOfCardsToGiveAway -= 1;
                CardGiveAway?.Invoke(card, _giverIndex, receiverIndex);
                if (!_giver.HasCards || _numberOfCardsToGiveAway == 0) 
                { 
                    RemoveAllOnGiveAwayListeners(); 
                    EndGiveAway(); 
                    return; 
                }
            }

            public void RegisterOnCardGiveAwayListener(OnCardGiveAwayListener listener)
            {
                CardGiveAway += listener;
                _onGiveAwayListeners.Add(listener);
            }

            void EndGiveAway()
            {
                _numberOfCardsToGiveAway = 0;
                RemoveAllOnGiveAwayListeners();
                TriggerOnFinishCardGiveAwayListeners();
            }

            void RemoveAllOnGiveAwayListeners()
            {
                foreach (OnCardGiveAwayListener listener in _onGiveAwayListeners)
                {
                    CardGiveAway -= listener;
                }
                _onGiveAwayListeners.Clear();
            }

            public void RegisterOnFinishCardGiveAwayListener(OnFinishGiveAwayListener listener)
            {
                _onFinishGiveAwayListeners.Enqueue(listener);
            }

            void TriggerOnFinishCardGiveAwayListeners()
            {
                while (_onFinishGiveAwayListeners.Count > 0)
                {
                    OnFinishGiveAwayListener listener = _onFinishGiveAwayListeners.Dequeue() 
                        ?? throw new NullReferenceException("Null reference exception for on finish giveaway listener!");
                    listener.Invoke();
                }
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
