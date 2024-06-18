using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SFXPlayer : MonoBehaviour
{
    private AudioSource _SFXSource;
    [SerializeField] private List<AudioClip> _SFXList = new List<AudioClip>();

    void Awake()
    {
        _SFXSource = GetComponent<AudioSource>();
    }

    /// <summary> 
    /// Plays the single SFX
    /// </summary> 
    public void PlayEffect()
    {
        _SFXSource.PlayOneShot(_SFXList[UnityEngine.Random.Range(0, _SFXList.Count)]);
    }
}
