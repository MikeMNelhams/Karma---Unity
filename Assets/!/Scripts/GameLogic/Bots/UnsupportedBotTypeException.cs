using System;

namespace KarmaLogic.Bots
{
    public class UnsupportedBotTypeException : Exception
    {
        public UnsupportedBotTypeException(string message) : base(message) { }

        public UnsupportedBotTypeException() : base("Unsupported bot type!") { }
    }
}