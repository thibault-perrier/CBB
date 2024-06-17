using UnityEngine;
public class AudioLoader : MonoBehaviour
{
    public enum AudioType
    {
        Music,
        SFX
    };

    [SerializeField] private AudioType _audioType;
    private AudioSource _source;
    private SoundManager _soundManager;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _soundManager = SoundManager.instance;

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        float musicVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1);

        switch (_audioType)
        {
            case AudioType.Music:
                _source.volume = masterVolume * musicVolume;
                if (_soundManager != null) 
                    _soundManager.MusicChangedEvent.AddListener(RefreshMusicVolume);     
                break;
            case AudioType.SFX:
                _source.volume = masterVolume * sfxVolume;
                if (_soundManager != null) 
                    _soundManager.SFXChangedEvent.AddListener(RefreshSFXVolume);
                break;
            default: 
                break;  
        }
    }
    public void RefreshMusicVolume()
    {
        Debug.Log("MUSIC VOLUME CHANGING");
        _source.volume = _soundManager.MasterVolume * _soundManager.MusicVolume;
    }

    public void RefreshSFXVolume()
    {
        Debug.Log("SFX VOLUME CHANGING");
        _source.volume = _soundManager.MasterVolume * _soundManager.SFXVolume;
    }
}
