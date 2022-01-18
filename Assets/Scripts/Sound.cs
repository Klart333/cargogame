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

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        car = FindObjectOfType<CarMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        print(car.wheelRPM);
        crntrpm = car.wheelRPM;

        soundrpm = crntrpm / maxrpm;
        pitch = pitchCurve.Evaluate(soundrpm);
        volume = volumeCurve.Evaluate(soundrpm);
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }
}
