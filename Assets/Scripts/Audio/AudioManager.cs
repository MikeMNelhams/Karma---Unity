using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    [Header("Audio Source")]
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _SFXSource;

    [Header("Audio Clips - Music")]
    public AudioClip _backgroundMusic;

    [Header("Audio Clips - SFX")]
    public AudioClip _cardBurn;
    public AudioClip _cardAddedToPile;
    public AudioClip _shufflePile;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        _musicSource.clip = _backgroundMusic;
        _musicSource.loop = true;
        _musicSource.Play();

        _SFXSource.loop = false;
    }

    public void PlayBurnSFX()
    {
        _SFXSource.clip = _cardBurn;
        _SFXSource.Play();
    }

    public void PlayShuffleSFX()
    {
        _SFXSource.clip = _shufflePile;
        _SFXSource.Play();
    }

    public void PlayCardAddedToPileSFX()
    {
        _SFXSource.clip = _cardAddedToPile;
        _SFXSource.Play();
    }
}
