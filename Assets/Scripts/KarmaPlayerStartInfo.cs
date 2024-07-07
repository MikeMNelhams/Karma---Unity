using System;
using UnityEngine;

[Serializable]
public class KarmaPlayerStartInfo
{
    public Vector3 startPosition;
    public bool isPlayableCharacter;

    public KarmaPlayerStartInfo(Vector3 startPosition, bool isPlayableCharacter)
    {
        this.startPosition = startPosition;
        this.isPlayableCharacter = isPlayableCharacter;
    }
}
