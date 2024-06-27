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

                public event BoardEventHandler OnTurnEndEvent;
                public event BoardBurnEventHandler OnBurnEvent;

                public void RegisterOnTurnEndEvent(BoardEventHandler newEventHandler)
                {
                    OnTurnEndEvent += newEventHandler;
                }

                public void TriggerOnTurnEndEvents(IBoard board)
                {
                    OnTurnEndEvent?.Invoke(board);
                }

                public void RegisterOnBurnEvent(BoardBurnEventHandler newEventHandler)
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