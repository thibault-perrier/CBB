using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RadioPlayer : MonoBehaviour
{
    private AudioSource _musicSource;      // The AudioSource component
    [SerializeField] private AudioClip[] _musicTracks;
    private int currentTrackIndex = 0;
    private bool isEmpty = false;


    void Awake()
    {
        _musicSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        isEmpty = (_musicTracks.Length == 0);
        ShufflePlaylist();
        PlayCurrenttrack();
    }

    void Update()
    {
        if (!isEmpty && !_musicSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void PlayCurrenttrack()
    {
        _musicSource.PlayOneShot(_musicTracks[currentTrackIndex]);
    }

    void PlayNextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex >= _musicTracks.Length)
        {
            ShufflePlaylist();
            currentTrackIndex = 0;
        }
        PlayCurrenttrack();
    }

    void ShufflePlaylist()
    {
        for (int i = 0; i < _musicTracks.Length; i++)
        {
            MusicTracksSwap(i, UnityEngine.Random.Range(0, _musicTracks.Length));
        }
    }

    void MusicTracksSwap(int firstIndex, int secondIndex)
    {
        (_musicTracks[firstIndex], _musicTracks[secondIndex]) = (_musicTracks[secondIndex], _musicTracks[firstIndex]);
    }
}
