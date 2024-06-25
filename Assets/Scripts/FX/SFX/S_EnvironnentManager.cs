using System.Collections;
using UnityEngine;

public class S_EnvironnentManager : MonoBehaviour
{
    public Transform Helicopter;
    public Transform PoliceCar;

    [SerializeField]
    private AudioClip _helicopterSound;
    [SerializeField]
    private AudioClip _policeCarSound;  

    private AudioSource helicopterAudioSource;
    private AudioSource policeCarAudioSource;

    public Light leftLight;
    public Light rightLight;

    private void Awake()
    {
        helicopterAudioSource = gameObject.AddComponent<AudioSource>();
        policeCarAudioSource = gameObject.AddComponent<AudioSource>();

        if (_helicopterSound != null)
        {
            helicopterAudioSource.clip = _helicopterSound;
        }
        else
        {
            Debug.LogError("helicopterSound n'est pas assigné !");
        }

        if (_policeCarSound != null)
        {
            policeCarAudioSource.clip = _policeCarSound;
        }
        else
        {
            Debug.LogError("PoliceCarSound n'est pas assigné !");
        }

    }

    void Start()
    {
        StartCoroutine(MovePoliceWithRandomDelay());
        StartCoroutine(MoveHelicopterWithRandomDelay());
    }

    private IEnumerator MoveHelicopterWithRandomDelay()
    {
        float minDelay = 10f;
        float maxDelay = 20f;
        float switchMinDelay = 10f;
        float switchMaxDelay = 25f;
        float speed = 50f;
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (helicopterAudioSource.clip != null)
            {
                PlaySoundHelicopter(0.05f);
            }

            Vector3 direction = Vector3.back;
            float timeElapsed = 0f;
            float switchTime = 17f;

            while (timeElapsed < switchTime)
            {
                timeElapsed += Time.deltaTime;
                Helicopter.transform.Translate(direction * speed * Time.deltaTime);
                yield return null;
            }

            float switchDelay = Random.Range(switchMinDelay, switchMaxDelay);
            yield return new WaitForSeconds(switchDelay);

            timeElapsed = 0f;

            if (helicopterAudioSource.clip != null)
            {
                PlaySoundHelicopter(0.05f);
            }

            while (timeElapsed <= switchTime)
            {
                timeElapsed += Time.deltaTime;
                Helicopter.transform.Translate(-direction * speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator MovePoliceWithRandomDelay()
    {
        float minDelay = 40f;
        float maxDelay = 60f;
        float switchMinDelay = 30f;
        float switchMaxDelay = 50f;
        float speed = 50f;

        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            StartCoroutine(FlashPoliceLights());

            if (policeCarAudioSource.clip != null)
            {
                PlaySoundPoliceCar(0.05f);
            }

            Vector3 direction = Vector3.back;
            float timeElapsed = 0f;
            float switchTime = 17f;

            while (timeElapsed < switchTime)
            {
                timeElapsed += Time.deltaTime;
                PoliceCar.transform.Translate(-direction * speed * Time.deltaTime);
                yield return null;
            }

            leftLight.enabled = false;
            rightLight.enabled = false;
            float switchDelay = Random.Range(switchMinDelay, switchMaxDelay);
            yield return new WaitForSeconds(switchDelay);

            timeElapsed = 0f;

            if (policeCarAudioSource.clip != null)
            {
                PlaySoundPoliceCar(0.05f);
            }

            StartCoroutine(FlashPoliceLights());

            while (timeElapsed <= switchTime)
            {
                timeElapsed += Time.deltaTime;
                PoliceCar.transform.Translate(direction * speed * Time.deltaTime);
                yield return null;
            }
        }
    }


    private IEnumerator FlashPoliceLights()
    {
        float flashInterval = 0.5f;

        while (true)
        {
            leftLight.enabled = true;
            rightLight.enabled = false;
            yield return new WaitForSeconds(flashInterval);

            leftLight.enabled = false;
            rightLight.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    public void PlaySoundHelicopter(float Volume = 0.5f)
    {
        helicopterAudioSource.PlayOneShot(_helicopterSound, Volume);
    }

    public void PlaySoundPoliceCar(float Volume = 0.5f)
    {
        policeCarAudioSource.PlayOneShot(_policeCarSound, Volume);
    }
}
