using UnityEngine;

public class AudioManager : SingletonPersistent<AudioManager>
{
    [Header("Audio Source")]
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _SFXSource;

    [Header("Audio Clips - Music")]
    [SerializeField] AudioClip _backgroundMusic;

    [Header("Audio Clips - SFX")]
    [SerializeField] AudioClip _cardBurn;
    [SerializeField] AudioClip _cardAddedToPile;
    [SerializeField] AudioClip _shufflePile;
    [SerializeField] AudioClip _cardPickup;

    [Header("Audio Settings")]
    public AudioSettings audioSettings;

    void Start()
    {
        _musicSource.clip = _backgroundMusic;
        _musicSource.loop = true;
        
        _musicSource.volume = audioSettings.musicSettings.VolumeNormalised;
        _musicSource.Play();

        _SFXSource.loop = false;
    }

    void PlaySFX(AudioClip clip)
    {
        _SFXSource.clip = clip;
        _SFXSource.volume = audioSettings.sFXSettings.VolumeNormalised;
        _SFXSource.Play();
    }

    public void PlayBurnSFX()
    {
        PlaySFX(_cardBurn);
    }

    public void PlayShuffleSFX()
    {
        PlaySFX(_shufflePile);
    }

    public void PlayCardAddedToPileSFX()
    {
        PlaySFX(_cardAddedToPile);
    }

    public void PlayCardPickup()
    {
        PlaySFX(_cardPickup);
    }
}
