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
                public delegate void BoardEventHandler(IBoard board);
                public delegate void BoardBurnEventHandler(int jokerCount);
                public delegate void PlayerDrawEventHandler(int numberOfCards, int playerIndex);

                public event PlayerDrawEventHandler PlayerDrawEvent;
                public event BoardEventHandler OnTurnStartEvent;
                public event BoardEventHandler OnTurnEndEvent;
                public event BoardBurnEventHandler OnBurnEvent;

                public void RegisterPlayerDrawEventHandler(PlayerDrawEventHandler playerDrawEventHandler)
                {
                    PlayerDrawEvent += playerDrawEventHandler;
                }

                public void TriggerPlayerDrawEvents(int numberOfCards, int playerIndex)
                {
                    PlayerDrawEvent?.Invoke(numberOfCards, playerIndex);
                }

                public void RegisterOnTurnStartEventHandler(BoardEventHandler newEventHandler)
                {
                    OnTurnStartEvent += newEventHandler;
                }

                public void TriggerOnTurnStartEvents(IBoard board)
                {
                    OnTurnStartEvent?.Invoke(board);
                }

                public void RegisterOnTurnEndEventHandler(BoardEventHandler newEventHandler)
                {
                    OnTurnEndEvent += newEventHandler;
                }

                public void TriggerOnTurnEndEvents(IBoard board)
                {
                    OnTurnEndEvent?.Invoke(board);
                }

                public void RegisterOnBurnEventHanlder(BoardBurnEventHandler newEventHandler)
                {
                    OnBurnEvent += newEventHandler;
                }

                public void TriggerBurnEvents(int jokerCount)
                {
                    OnBurnEvent?.Invoke(jokerCount);
                }
            }
        }
    }
}