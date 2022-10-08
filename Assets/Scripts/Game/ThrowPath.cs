using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPath : MonoBehaviour
{
    // Tunables
    [SerializeField] Transform[] path = null;

    // State
    int lastTarget = 0;
    int currentTarget = 1;
    float distance = Mathf.Infinity;

    // Unity Methods
    private void Awake()
    {
        if (!HasPath()) { return; }

        distance = Vector3.Distance(GetLastTarget(), GetCurrentTarget());
    }

    // Public Methods
    public bool HasPath() => (path != null && path.Length > 1);
    public float GetDistance() => distance;

    public Vector3 GetCurrentTarget()
    {
        if (!HasPath()) { return Vector3.zero; }

        return path[currentTarget].position;
    }

    public Vector3 GetLastTarget()
    {
        if (!HasPath()) { return Vector3.zero; }

        return path[lastTarget].position;
    }

    public void SetNextTarget()
    {
        lastTarget = currentTarget;
        currentTarget++;
        if (currentTarget >= path.Length) { currentTarget = 0; }

        distance = Vector3.Distance(GetLastTarget(), GetCurrentTarget());
    }
}
