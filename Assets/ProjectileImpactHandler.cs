using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileImpactHandler : MonoBehaviour
{
    public float LifeTime;
    private float currentTime;

    void Update()
    {
        // Accumulate time
        currentTime += Time.deltaTime;

        // Check if the accumulated time exceeds the lifetime
        if (currentTime >= LifeTime)
        {
            Destroy(gameObject);
        }
    }
}
