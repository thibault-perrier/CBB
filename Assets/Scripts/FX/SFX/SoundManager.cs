using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Assignables")]
    [SerializeField] private Slider _masterSoundVolume;
    [SerializeField] private Slider _musicSoundVolume;
    [SerializeField] private Slider _SFXSoundVolume;
    
    [Space(10)]
    [Header("Settings Volumes")]
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;

    public UnityEvent MusicChangedEvent;
    public UnityEvent SFXChangedEvent;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadSoundVolume();
    }

    private void LoadSoundVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1);

        _masterSoundVolume.value = MasterVolume;
        _musicSoundVolume.value = MusicVolume;
        _SFXSoundVolume.value = SFXVolume;
    }

    public void OnMasterChanged()
    {
        MasterVolume = _masterSoundVolume.value;
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        MusicChangedEvent.Invoke();
        SFXChangedEvent.Invoke();   
    }

    public void OnMusicChanged()
    {
        MusicVolume = _musicSoundVolume.value;
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        MusicChangedEvent.Invoke();
    }

    public void OnSFXChanged()
    {
        SFXVolume = _SFXSoundVolume.value;
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        SFXChangedEvent.Invoke();
    }
}
