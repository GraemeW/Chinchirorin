using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    // Tunables
    [SerializeField] DiceType diceType = DiceType.D6;
    [SerializeField] float delayToCheckSpeed = 1.0f;
    [SerializeField] float speedThresholdToSettle = 0.001f;
    [SerializeField] float maxTimeToSettle = 5.0f;

    // State
    bool checkForStationary = false;
    int diceIndex = -1;
    float squareSpeed = Mathf.Infinity;
    float timeSinceThrown = 0f;

    // Cached References
    SoundEffects soundEffects = null;
    Rigidbody diceRigidbody = null;

    // Events
    public event Action<Dice> diceRollComplete;

    // Static Methods
    public static Vector3 GetOrientationMapping(DiceType diceType, int value)
    {
        // Warning:  Strictly tied to orientation of dice model as-constructed
        switch (diceType)
        {
            case DiceType.D6:
                switch (value)
                {
                    case 1: return Vector3.forward; 
                    case 2: return -Vector3.up;
                    case 3: return -Vector3.right;
                    case 4: return Vector3.right;
                    case 5: return Vector3.up;
                    case 6: return -Vector3.forward;
                }
                break;
        };
        return Vector3.zero;
    }

    public static int GetMaxCount(DiceType diceType)
    {
        switch(diceType)
        {
            case DiceType.D6:
                return 6;
        }
        return 0;
    }

    // Unity Methods
    private void Awake()
    {
        soundEffects = GetComponent<SoundEffects>();
        diceRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!checkForStationary) { return; }

        squareSpeed = diceRigidbody.velocity.sqrMagnitude;
        if (squareSpeed < speedThresholdToSettle)
        {
            checkForStationary = false;
            diceRollComplete?.Invoke(this);
        }

        timeSinceThrown += Time.deltaTime;
        if (timeSinceThrown > maxTimeToSettle)
        {
            diceRollComplete?.Invoke(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (soundEffects != null)
        {
            soundEffects.PlayRandomAudioClip();
        }
    }

    private IEnumerator QueueCheckForStationary()
    {
        checkForStationary = false;
        yield return new WaitForSeconds(delayToCheckSpeed);
        checkForStationary = true;
    }

    // Public Methods
    public void Setup(Vector3 position, Quaternion rotation, int diceIndex)
    {
        transform.position = position;
        transform.rotation = rotation;

        this.diceIndex = diceIndex;
        StartCoroutine(QueueCheckForStationary());
    }

    public int GetIndex() => diceIndex;
    public int GetUpValue()
    {
        if (checkForStationary) { return -1; } // Device still in motion while checking for value

        int maxCount = GetMaxCount(diceType);

        float maxDot = 0;
        int upValue = 1;
        for (int i = 1; i <= maxCount; i++)
        {
            Vector3 orientationMapping = GetOrientationMapping(diceType, i);
            float currentDot = Vector3.Dot(transform.rotation * orientationMapping, Vector3.up);
            if (currentDot > maxDot)
            {
                maxDot = currentDot;
                upValue = i;
            }
        }
        return upValue;
    }
}
