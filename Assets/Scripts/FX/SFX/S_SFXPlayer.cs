using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SFXPlayer : MonoBehaviour
{
    public enum SFXType
    {
        Single,
        MiddleLoop
    }

    [SerializeField] private SFXType _SFXType;

    [SerializeField] private List<AudioClip> _SFXList = new List<AudioClip>();
    private AudioSource _SFXSource;

    public bool _isLooping = false;


    void Awake()
    {
        _SFXSource = GetComponent<AudioSource>();
    }

    /// <summary> 
    /// Plays the single SFX
    /// </summary> 
    public void PlayEffect()
    {
        if (_SFXType == SFXType.Single)
            _SFXSource.PlayOneShot(_SFXList[UnityEngine.Random.Range(0, _SFXList.Count)]);
        else if (_SFXType == SFXType.MiddleLoop)
        {
            _SFXSource.PlayOneShot(_SFXList[0]);
            _isLooping = true;
            StartCoroutine(PlayMiddleLoop());
        }
    }

    private IEnumerator PlayMiddleLoop()
    {
        yield return new WaitForSeconds(_SFXList[0].length);

        while (_isLooping)
        {
            if (!_SFXSource.isPlaying)
            {
                _SFXSource.PlayOneShot(_SFXList[1]);
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        PlayEndLoop();
    }

    private void PlayEndLoop()
    {
        _SFXSource.PlayOneShot(_SFXList[2]);
    }

    public void StopLoop()
    {
        _isLooping = false;
    }
}
