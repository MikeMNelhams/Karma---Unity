using System;

namespace KarmaLogic
{
    public class UnsupportedCharacterTypeException : Exception
    {
        public UnsupportedCharacterTypeException(string message) : base(message) { }

        public UnsupportedCharacterTypeException() : base("Unsupported player-character type!") { }
    }
}

