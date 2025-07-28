using UnityEngine;

public class SingletonPersistent<T> : MonoBehaviour where T : SingletonPersistent<T>
{
    private static T _instance;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            UnityEngine.Debug.LogWarning("There should only exist one singleton by definition.", this);
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
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