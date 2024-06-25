using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SFXPlayer : MonoBehaviour
{
    private AudioSource _firstSFXSource;
    [SerializeField] private AudioSource _secondSFXSource;
    public enum SFXType
    {
        Single,
        MiddleLoop
    }

    [SerializeField] 
    private SFXType _SFXType;
    [SerializeField] 
    private List<AudioClip> _SFXList = new List<AudioClip>();
    [SerializeField, Min(0f)]
    private float _scaleSfxVolume = 1f;

    public bool _isLooping = false;
    private bool _isFirstSourcePlaying = false;
    [SerializeField] private bool _playOnStart;

    void Awake()
    {
        _firstSFXSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (_playOnStart)
        {
            PlayEffect();
        }
    }

    /// <summary> 
    /// Plays the single SFX
    /// </summary> 
    public void PlayEffect()
    {
        if (_SFXType == SFXType.Single)
            _firstSFXSource.PlayOneShot(_SFXList[UnityEngine.Random.Range(0, _SFXList.Count)], _scaleSfxVolume);
        else if (_SFXType == SFXType.MiddleLoop)
        {
            _firstSFXSource.PlayOneShot(_SFXList[0], _scaleSfxVolume);
            _isLooping = true;
            StartCoroutine(PlayMiddleLoop());
        }
    }

    private IEnumerator PlayMiddleLoop()
    {
        yield return new WaitForSeconds(_SFXList[0].length - 0.1f);

        while (_isLooping)
        {
            (_isFirstSourcePlaying ? _secondSFXSource : _firstSFXSource).PlayOneShot(_SFXList[1], _scaleSfxVolume);
            yield return new WaitForSeconds(_SFXList[1].length - 0.1f);
        }

        PlayEndLoop();
    }

    private void PlayEndLoop()
    {
        _firstSFXSource.PlayOneShot(_SFXList[2], _scaleSfxVolume);
    }

    public void StopLoop()
    {
        _isLooping = false;
    }
}
