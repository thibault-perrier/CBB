using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SFXPlayer : MonoBehaviour
{
    private AudioSource _SFXSource;
    [SerializeField] private List<AudioClip> _SFXList = new List<AudioClip>();

    public enum SFXType
    {
        Single,
        MiddleLoop
    }

    [SerializeField] private SFXType _SFXType;

    private bool _isPlaying = false;


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

        }
    }

    public void PlaySaw()
    {

    }

    public void PlayNextEffect()
    {

    }

    void Update()
    {
        if (!_SFXSource.isPlaying)
            Debug.Log("IS PLAYING!!");
    }
}
