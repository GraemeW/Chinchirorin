using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffects : MonoBehaviour
{
    // Tunables
    [SerializeField] AudioClip[] audioClips = null;

    // State
    bool checkForAudioClipCompletion = false;
    float timeSinceClipStarted = 0f;
    float clipLength = Mathf.Infinity;

    // Cached References
    AudioSource audioSource = null;

    // Events
    public event Action audioComplete;

    // Unity Methods
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!checkForAudioClipCompletion) { return; }

        timeSinceClipStarted += Time.deltaTime;
        if (timeSinceClipStarted > clipLength)
        {
            audioComplete?.Invoke();
            checkForAudioClipCompletion = false;
        }
    }

    // Public Methods
    public void PlayRandomAudioClip()
    {
        if (audioClips == null || audioClips.Length == 0) { return; }
        if (audioSource.isPlaying) { return; }

        int audioClipIndex = UnityEngine.Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[audioClipIndex];
        audioSource.Play();

        // Traceability for clip completion for queuing / event-driven clips
        timeSinceClipStarted = 0f;
        clipLength = audioSource.clip.length;
        checkForAudioClipCompletion = true;
    }
}
