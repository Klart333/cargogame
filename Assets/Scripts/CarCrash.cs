using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarCrash : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent crashSound;

    [SerializeField]
    private float amplitude = 2f;

    [SerializeField]
    private float frequency = 3;

    [SerializeField]
    private float length = 0.5f;

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
        AudioManager.Instance.PlaySoundEffect(crashSound, volume);
        cameraShake.Noise(amplitude * volume, frequency, length);
    }
}
