using UnityEngine;

[System.Serializable]
public class MusicSettings
{
    [SerializeField] int _volume = 100;

    public int Volume { get { return _volume; } set { _volume = Mathf.Clamp(value, 0, 100); } }
}
