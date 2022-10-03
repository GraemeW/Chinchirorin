using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowHand : MonoBehaviour
{
    // Tunables
    [Header("Properties")]
    [SerializeField] int diceCount = 3;
    [SerializeField] ThrowPath throwPath = null;
    [SerializeField] float speed = 0.5f;
    [SerializeField] float arcHeight = 0.1f;
    [Header("Hookups")]
    [SerializeField] DiceContainer diceContainer = null;
    [SerializeField] SoundEffects grunter = null;
    [SerializeField] SoundEffects chucker = null;

    // State
    bool isThrowComplete = false;
    float stepScale = 0f;
    float progress = 0f;

    // Input
    ChinchirorinInput chinchirorinInput = null;

    // Unity Methods
    private void Awake()
    {
        chinchirorinInput = new ChinchirorinInput();
        chinchirorinInput.Standard.Throw.performed += context => InitializeThrow();
        isThrowComplete = false;
    }

    private void OnEnable()
    {
        chinchirorinInput.Standard.Enable();
    }

    private void OnDisable()
    {
        chinchirorinInput.Standard.Disable();
    }

    private void Start()
    {
        if (throwPath == null || !throwPath.HasPath()) { return; }

        stepScale = speed / throwPath.GetDistance();
    }

    private void Update()
    {
        if (throwPath == null || !throwPath.HasPath()) { return; }
        if (isThrowComplete) { return; }

        MoveHandAlongParabola();

        if (Mathf.Approximately(progress, 1.0f))
        {
            progress = 0f;
            throwPath.SetNextTarget();
            stepScale = speed / throwPath.GetDistance();
        }
    }

    // Private Methods
    private void MoveHandAlongParabola()
    {
        progress = Mathf.Min(progress + Time.deltaTime * stepScale, 1.0f);
        Vector3 lastTarget = throwPath.GetLastTarget();
        Vector3 currentTarget = throwPath.GetCurrentTarget();

        // Smoothstepping to avoid the sharp motion at targets
        float xPosition = Mathf.SmoothStep(lastTarget.x, currentTarget.x, progress);
        float yPosition = Mathf.SmoothStep(lastTarget.y, currentTarget.y, progress);
        float zPosition = Mathf.SmoothStep(lastTarget.z, currentTarget.z, progress);
        Vector3 nextPosition = new Vector3(xPosition, yPosition, zPosition);

        // Offset y in arc of parabola
        float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);
        nextPosition.y -= parabola * arcHeight;

        // No solid body / physics, just force update
        transform.position = nextPosition;
    }

    private void InitializeThrow()
    {
        if (diceContainer == null) { return; }
        if (isThrowComplete) { return; }

        isThrowComplete = true;
        grunter.PlayRandomAudioClip();
        grunter.audioComplete += FinishThrow;
    }

    private void FinishThrow()
    {
        if (diceContainer == null) { return; }

        chucker.PlayRandomAudioClip();
        diceContainer.SpawnDice(transform, diceCount);
        grunter.audioComplete -= FinishThrow;
    }

    // Public Methods
    public void ResetThrow()
    {
        isThrowComplete = false;
    }
}
