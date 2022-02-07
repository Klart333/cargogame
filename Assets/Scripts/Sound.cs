using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public float soundrpm;
    public float maxrpm;
    public float crntrpm;
    private CarMovement car;

    public Sounding[] soundings = new Sounding[0];



    [System.Serializable]
    public struct Sounding
    {
        public AnimationCurve pitchCurve;
        public AnimationCurve volumeCurve;
        public AudioSource audioSource;
        public AudioClip clip;
    }

    void Start()
    {
        for (int i = 0; i < soundings.Length; i++)
        {
            soundings[i].audioSource = gameObject.AddComponent<AudioSource>();
            soundings[i].audioSource.clip = soundings[i].clip;
            soundings[i].audioSource.loop = true;
            soundings[i].audioSource.Play();
        }
        car = FindObjectOfType<CarMovement>();

    }

    void Update()
    {
        for (int i = 0; i < soundings.Length; i++)
        {
            float pitch;
            float volume;
            pitch = soundings[i].pitchCurve.Evaluate(soundrpm);
            volume = soundings[i].volumeCurve.Evaluate(soundrpm);           
           soundings[i].audioSource.volume = volume;
           soundings[i].audioSource.pitch = pitch;
        }
        crntrpm = car.wheelRPM;
        soundrpm = crntrpm / maxrpm * Time.timeScale;
    }
}
