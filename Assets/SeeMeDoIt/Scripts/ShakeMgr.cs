using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float accelerometerUpdateInterval = 1.0f / 60.0f;
    const float lowPassKernelWidthInSeconds = 1.0f;
    float shakeDetectionThreshold = 1f; // 2.0f;
    public bool ynShake;
    public bool ynShakeLast;
    float lowPassFilterFactor;
    Vector3 lowPassValue;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateYnShake();
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
        ynShakeLast = ynShake;
    }

    void UpdateYnShake()
    {
        ynShake = false;
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            Debug.Log("Shake event detected at time " + Time.time);
            ynShake = true;
        }
    }

}
