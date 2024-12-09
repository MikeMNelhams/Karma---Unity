using KarmaLogic.Board;
using System;
using System.Collections.Generic;

namespace KarmaLogic
{
    namespace Players
    {
        public class PlayPileGiveAwayHandler
        {
            readonly int _giverIndex;
            readonly Player _giver;

            public delegate void OnGiveAwayListener(int giverIndex, int targetIndex);
            event OnGiveAwayListener PlayPileGiveAway;

            public delegate bool InvalidFilter(Player giver);

            readonly List<OnGiveAwayListener> _onGiveAwayListeners;

            public delegate void OnFinishGiveAwayListener();
            readonly Queue<OnFinishGiveAwayListener> _onFinishGiveAwayListeners;

            readonly InvalidFilter _invalidFilter;

            public bool IsFinished { get; private set; }
            
            public PlayPileGiveAwayHandler(IBoard board, int giverIndex, InvalidFilter invalidFilter = null)
            {
                _giverIndex = giverIndex;
                _giver = board.Players[giverIndex];
                _invalidFilter = invalidFilter;

                _onGiveAwayListeners = new();
                _onFinishGiveAwayListeners = new();
            }

            public void GiveAway(int targetIndex)
            {
                if (_invalidFilter != null && _invalidFilter(_giver)) { EndGiveAway(); return; }
                IsFinished = true;
                PlayPileGiveAway?.Invoke(_giverIndex, targetIndex);
                if (IsFinished)
                {
                    RemoveAllOnGiveAwayListeners();
                    EndGiveAway();
                    return;
                }
            }

            public void RegisterOnCardGiveAwayListener(OnGiveAwayListener listener)
            {
                PlayPileGiveAway += listener;
                _onGiveAwayListeners.Add(listener);
            }

            void EndGiveAway()
            {
                RemoveAllOnGiveAwayListeners();
                TriggerOnFinishGiveAwayListeners();
            }

            void RemoveAllOnGiveAwayListeners()
            {
                foreach (OnGiveAwayListener listener in _onGiveAwayListeners)
                {
                    PlayPileGiveAway -= listener;
                }
                _onGiveAwayListeners.Clear();
            }

            void TriggerOnFinishGiveAwayListeners()
            {
                while (_onFinishGiveAwayListeners.Count > 0)
                {
                    OnFinishGiveAwayListener listener = _onFinishGiveAwayListeners.Dequeue()
                        ?? throw new NullReferenceException("Null reference exception for on finish giveaway listener!");
                    listener.Invoke();
                }
            }
        }
    }
}

