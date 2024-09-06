using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using System;
using UnityEngine;

namespace KarmaLogic
{
    namespace Controller
    {
        public class IController
        {
            public ControllerState State { get; protected set; }
            public virtual void SetState(ControllerState newState)
            {
                State?.OnExit();
                newState.OnEnter();
                State = newState;
            } 
        }

        public abstract class ControllerState : IEquatable<ControllerState>
        {
            protected IBoard _board;
            public BaseCharacterProperties _playerProperties;

            protected ControllerState(IBoard board, BaseCharacterProperties playerProperties)
            {
                _board = board;
                _playerProperties = playerProperties;
            }

            public abstract void OnEnter();
            public abstract void OnExit();
            public abstract override int GetHashCode();

            public bool Equals(ControllerState other)
            {
                if (ReferenceEquals(this, other)) { return true; }
                if (GetType() == other.GetType()) { return true; }
                return false;
            }

            public override string ToString()
            {
                return "State ID: " + GetHashCode();
            }
        }
    }
}
