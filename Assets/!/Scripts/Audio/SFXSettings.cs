using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXSettings
{
    [SerializeField] int _volume = 100;

    public int Volume { get { return _volume; } set { _volume = Mathf.Clamp(value, 0, 100); } }
    public float VolumeNormalised { get { return _volume / 100.0f; } }
}
