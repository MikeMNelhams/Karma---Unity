using System;
using UnityEngine;

[Serializable]
public class KarmaPlayerStartInfo
{
    [SerializeField] public Vector3 startPosition;

    public KarmaPlayerStartInfo(Vector3 startPosition)
    {
        this.startPosition = startPosition;
    }
}
