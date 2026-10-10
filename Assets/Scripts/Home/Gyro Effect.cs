using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroEffect : MonoBehaviour
{
    public float motionMultiplier = 0.06f; 
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;

        // Enable gyroscope....
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Debug.Log("Gyro enablle");
        }
        else
        {
            Debug.Log("Gyro not supported");
        }
    }

    void Update()
    {
        Vector3 offset = Vector3.zero;

        // Use gyroscope if available....
        if (SystemInfo.supportsGyroscope)
        {
            Gyroscope gyro = Input.gyro;
            offset = new Vector3(-gyro.rotationRateUnbiased.y, gyro.rotationRateUnbiased.x, 0);
        }
        else
        {
            // Fallback to accelerometer if gyroscope not available....
            Vector3 accel = Input.acceleration;
            offset = new Vector3(-accel.x, accel.y, 0);
        }

        // Apply smooth motion to background....
        transform.position = Vector3.Lerp(transform.position, initialPosition + offset * motionMultiplier, Time.deltaTime * 5f);
    }
}
