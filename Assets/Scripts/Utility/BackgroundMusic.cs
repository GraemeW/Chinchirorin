using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    // Tunables
    [SerializeField] float rampVolumeStart = 0f;
    [SerializeField] float rampVolumeStop = 0.5f;
    [SerializeField] float timeToRamp = 5.0f;

    // Cached References
    AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.volume = rampVolumeStart;
        StartCoroutine(StartFade());
    }

    private IEnumerator StartFade()
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < timeToRamp)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, rampVolumeStop, currentTime / timeToRamp);
            yield return null;
        }
        yield break;
    }
}
