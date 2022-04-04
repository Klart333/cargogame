using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    private Queue<AudioSource> audioSources = new Queue<AudioSource>();

    private SimpleAudioEvent driftSound;
    private AudioSource driftSource;
    private Coroutine driftRoutine;
    private bool drifting = true;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume");
        if (volume == 0)
        {
            volume = 1;
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        AudioListener.volume = volume;
    }

    public void PlaySoundEffect(SimpleAudioEvent audio, float volume = 1)
    {
        if (audioSources.Count == 0)
        {
            audioSources.Enqueue(gameObject.AddComponent<AudioSource>());
        }
        var source = audioSources.Dequeue();
        int index = audio.Play(source, volume);

        StartCoroutine(ReturnToQueue(source, audio.Clips[index].length));
    }

    private IEnumerator ReturnToQueue(AudioSource source, float length)
    {
        yield return new WaitForSeconds(length);

        audioSources.Enqueue(source);
    }

    public void StartDrift(SimpleAudioEvent driftSound)
    {
        if (driftSource == null)
        {
            driftSource = gameObject.AddComponent<AudioSource>();
        }
        
        drifting = true;
        this.driftSound = driftSound;

        if (driftRoutine != null)
        {
            StopCoroutine(driftRoutine);
        }
        driftRoutine = StartCoroutine(ContinueDrifting());
    }

    private IEnumerator ContinueDrifting()
    {
        StartCoroutine(FadeIn());

        while (drifting)
        {
            driftSound.Play(driftSource, 1);
            yield return new WaitForSeconds(driftSound.Clips[0].length * 0.9f);
        }

        driftSound.Play(driftSource, 1);
        float t = 0;
        float speed = 4;
        while (t <= 1)
        {
            t += Time.deltaTime * speed;
            driftSource.volume = 1 * (1 - t);

            yield return null;
        }

        Destroy(driftSource);
        driftSource = null;
        driftRoutine = null;
    }

    private IEnumerator FadeIn()
    {
        float t = 0;
        float speed = 4;
        while (t <= 1)
        {
            t += Time.deltaTime * speed;
            driftSource.volume = 1 * t;

            yield return null;
        }
    }

    public void EndDrift()
    {
        drifting = false;
    }
}
