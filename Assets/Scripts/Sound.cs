using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AnimationCurve pitchCurve;
    public AnimationCurve volumeCurve;
    public AudioSource audioSource;
    public float pitch;
    public float volume;
    public float soundrpm;
    public float maxrpm;
    public float crntrpm;

    private CarMovement car;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        car = FindObjectOfType<CarMovement>();
    }

    void Update()
    {
        crntrpm = car.EngineRPM;

        soundrpm = crntrpm / maxrpm * Time.timeScale;
        pitch = pitchCurve.Evaluate(soundrpm);
        volume = volumeCurve.Evaluate(soundrpm);
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }
}
