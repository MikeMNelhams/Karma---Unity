using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KarmaLogic.Cards;
using System;

namespace KarmaLogic
{
    namespace Board
    {
        namespace BoardEvents
        {
            public class BoardEventSystem
            {
                public delegate void EventListenerAutoTearDown();
                public delegate void BoardEventListener(IBoard board);
                public delegate void BoardBurnEventListener(int jokerCount);
                public delegate void PlayerDrawEventListener(int numberOfCards, int playerIndex);
                public delegate void BoardHandsRotationEventListener(int numberOfRotations, IBoard board);
                public delegate void BoardOnStartCardGiveAwayListener(int numberOfCards, int playerIndex);
                public delegate void BoardOnStartPlayPileGiveAwayListener(int playerIndex);

                public event BoardEventListener HandsFlippedEvent;
                public event BoardHandsRotationEventListener HandsRotatedEvent;
                public event PlayerDrawEventListener PlayerDrawEvent;
                public event BoardEventListener OnTurnStartEvent;
                public event BoardEventListener OnTurnEndEvent;
                public event BoardBurnEventListener OnBurnEvent;
                public event BoardOnStartCardGiveAwayListener StartedCardGiveAway;
                public event BoardOnStartPlayPileGiveAwayListener StartedPlayPileGiveAway;
                
                // Events that cleanup after themselves:
                public Queue<EventListenerAutoTearDown> _onFinishPlaySuccesfulComboListeners = new ();
                public Queue<EventListenerAutoTearDown> _onFinishGiveAwayListeners = new ();

                public void RegisterOnFinishPlaySuccesfulComboListener(EventListenerAutoTearDown listener)
                {
                    _onFinishPlaySuccesfulComboListeners ??= new ();
                    _onFinishPlaySuccesfulComboListeners.Enqueue(listener);
                }

                public void TriggerOnFinishPlaySuccesfulComboListenersWithTearDown()
                {
                    while (_onFinishPlaySuccesfulComboListeners.Count > 0)
                    {
                        EventListenerAutoTearDown listener = _onFinishPlaySuccesfulComboListeners.Dequeue() 
                            ?? throw new NullReferenceException("Null Board Finish Play Succesful Combo Listener");
                        listener.Invoke();
                    }
                }

                public void RegisterOnFinishGiveAwayListener(EventListenerAutoTearDown listener)
                {
                    _onFinishGiveAwayListeners ??= new ();
                    _onFinishGiveAwayListeners.Enqueue(listener);
                }

                public void TriggerOnFinishGiveAwayListenersWithTearDown()
                {
                    while (_onFinishGiveAwayListeners.Count > 0)
                    {
                        EventListenerAutoTearDown listener = _onFinishGiveAwayListeners.Dequeue() 
                            ?? throw new NullReferenceException("Null Board Finish Give Away listener"); 
                        listener.Invoke();
                    }
                }

                public void RegisterHandsRotatedListener(BoardHandsRotationEventListener listener)
                {
                    HandsRotatedEvent += listener;
                }

                public void TriggerHandsRotatedEventListener(int numberOfRotations, IBoard board)
                {
                    HandsRotatedEvent?.Invoke(numberOfRotations, board);
                }

                public void RegisterHandsFlippedEventListener(BoardEventListener listener)
                {
                    HandsFlippedEvent += listener;
                }

                public void TriggerHandsFlippedEvent(IBoard board)
                {
                    HandsFlippedEvent?.Invoke(board);
                }

                public void RegisterPlayerDrawEventListener(PlayerDrawEventListener listener)
                {
                    PlayerDrawEvent += listener;
                }

                public void TriggerPlayerDrawEvents(int numberOfCards, int playerIndex)
                {
                    PlayerDrawEvent?.Invoke(numberOfCards, playerIndex);
                }

                public void RegisterStartCardGiveAwayListener(BoardOnStartCardGiveAwayListener listener)
                {
                    StartedCardGiveAway += listener;
                }

                public void TriggerStartedCardGiveAway(int numberOfCards, int giverIndex)
                {
                    StartedCardGiveAway?.Invoke(numberOfCards, giverIndex);
                }

                public void RegisterPlayPileGiveAwayListener(BoardOnStartPlayPileGiveAwayListener listener)
                {
                    StartedPlayPileGiveAway += listener;
                }

                public void TriggerStartedPlayPileGiveAway(int giverIndex)
                {
                    StartedPlayPileGiveAway?.Invoke(giverIndex);
                }

                public void RegisterOnTurnStartEventListener(BoardEventListener listener)
                {
                    OnTurnStartEvent += listener;
                }

                public void TriggerOnTurnStartEvents(IBoard board)
                {
                    OnTurnStartEvent?.Invoke(board);
                }

                public void RegisterOnTurnEndEventListener(BoardEventListener listener)
                {
                    OnTurnEndEvent += listener;
                }

                public void TriggerOnTurnEndEvents(IBoard board)
                {
                    OnTurnEndEvent?.Invoke(board);
                }

                public void RegisterOnBurnEventListener(BoardBurnEventListener listener)
                {
                    OnBurnEvent += listener;
                }

                public void TriggerBurnEvents(int jokerCount)
                {
                    OnBurnEvent?.Invoke(jokerCount);
                }
            }
        }
    }
}