using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CrowdCheering : MonoBehaviour
{
    [SerializeField] private List<AudioSource> _crowdAudioSources;
    [SerializeField] private List<AudioClip> _crowdSounds;

    private void Start()
    {
        StartCheering();
    }

    IEnumerator PlayOnAudioSource(int audioSourceIndex)
    {
        AudioClip _crowdSound = _crowdSounds[Random.Range(0, _crowdSounds.Count)];
        _crowdAudioSources[audioSourceIndex].PlayOneShot(_crowdSound);

        yield return new WaitForSeconds(_crowdSound.length - 0.5f);
        StartCoroutine(PlayOnAudioSource((audioSourceIndex + 1) % _crowdAudioSources.Count));

        yield return null;
    }

    public void StartCheering()
    {
        StartCoroutine(PlayOnAudioSource(0));
    }

    public void StopCheering()
    {
        foreach (AudioSource _audioSource in _crowdAudioSources)
        {
            _audioSource.Stop();
        }
    }
}
