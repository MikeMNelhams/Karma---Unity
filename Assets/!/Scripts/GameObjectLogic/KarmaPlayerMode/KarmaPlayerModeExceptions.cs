using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KarmaPlayerMode
{
    public class KarmaPlayerModeException : Exception
    {
        public KarmaPlayerModeException(string message) : base(message) { }
    }

    public class BoardPresetException : Exception
    {
        public BoardPresetException(string message) : base(message) { }
    }
}

