using UnityEngine;

public class ColorManager : MonoBehaviour
{
    static ColorManager _instance;

    public static ColorManager Instance { get => _instance; }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    [SerializeField] Color _colorPicker;
}
