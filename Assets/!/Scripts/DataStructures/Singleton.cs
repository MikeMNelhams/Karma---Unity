using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    protected virtual void Awake()
    {
        UnityEngine.Debug.Log("AWOKEN", this);
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
        }
    }

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }
}