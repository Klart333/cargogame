using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarCrash : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent crashSound;

    [SerializeField]
    private float amplitude = 15f;

    [SerializeField]
    private float frequency = 1;

    [SerializeField]
    private float length = 0.25f;

    private CarMovement car;
    private CameraShake cameraShake;

    private void Start()
    {
        car = GetComponent<CarMovement>();
        cameraShake = FindObjectOfType<CameraShake>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float volume = 1.0f - Mathf.Clamp01(car.Velocity.magnitude / 45f);

        Rigidbody colRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (colRigidbody != null)
        {
            if (colRigidbody.mass < 10)
            {
                volume = 0.1f;
            }
        }
 
        AudioManager.Instance.PlaySoundEffect(crashSound, volume);
        cameraShake.ScreenShake(amplitude * volume, frequency, length);
    }
}
