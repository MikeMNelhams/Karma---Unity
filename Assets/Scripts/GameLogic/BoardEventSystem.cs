using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karma
{
    namespace Board
    {
        namespace BoardEvents
        {
            public class BoardEventSystem
            {
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