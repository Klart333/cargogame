using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarCrash : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent crashSound;

    [SerializeField]
    private float amplitude = 25f;

    [SerializeField]
    private float frequency = 1;

    [SerializeField]
    private float length = 0.5f;

    private CarMovement car;
    private CameraShake cameraShake;

    private float lastVelocity = 0;

    private void Start()
    {
        car = GetComponent<CarMovement>();
        cameraShake = FindObjectOfType<CameraShake>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float lost = lastVelocity - car.Velocity.magnitude;

        float volume = Mathf.Clamp01(lost / 30f);

        /*Rigidbody colRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (colRigidbody != null)
        {
            if (colRigidbody.mass < 10)
            {
                volume = 0.1f;
            }
        }*/
 
        AudioManager.Instance.PlaySoundEffect(crashSound, volume);
        cameraShake.ScreenShake(amplitude * volume, frequency, length);
    }

    private void LateUpdate()
    {
        lastVelocity = car.Velocity.magnitude;
    }
}
