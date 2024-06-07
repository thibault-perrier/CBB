using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioSource _boomSound;
    // Start is called before the first frame update
    void Awake()
    {
        _boomSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("PLAYING SOUND");
            _boomSound.PlayOneShot(_boomSound.clip);
        }
    }
}
