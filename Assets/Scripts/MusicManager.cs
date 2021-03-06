using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField]
    private SimpleAudioEvent[] musics;

    private AudioSource audioSource;

    private float outFadedVolume = -1;

    private int currentBuildIndex = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        musics[0].Play(audioSource);

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene fromScene, Scene toScene)
    {
        if (currentBuildIndex == toScene.buildIndex)
        {
            return;
        }

        if (toScene.buildIndex == 0)
        {
            outFadedVolume = audioSource.volume;
            audioSource.volume = 0;
            FadeIn(1, 0.2f);

            musics[0].Play(audioSource);
            audioSource.loop = false;
        }
        else
        {
            audioSource.Stop();
            audioSource.loop = true;

            if (toScene.buildIndex < musics.Length)
            {
                StartCoroutine(PlayAfterDelay(toScene.buildIndex));
            }
            else
            {
                StartCoroutine(PlayAfterDelay(musics.Length - 1));
            }
        }

        currentBuildIndex = toScene.buildIndex;
    }

    private IEnumerator PlayAfterDelay(int index)
    {
        yield return new WaitForSeconds(3);
        musics[index].Play(audioSource);
    }

    public void FadeOut(float finalPercentage = 0, float extraSpeed = 1)
    {
        StartCoroutine(FadingOut(finalPercentage, extraSpeed));
    }

    private IEnumerator FadingOut(float finalPercentage, float extraSpeed)
    {
        float t = 1;
        float speed = 1.0f;
        float ogVolume = audioSource.volume;
        outFadedVolume = ogVolume;

        while (t >= finalPercentage)
        {
            t -= Time.deltaTime * speed * extraSpeed;

            audioSource.volume = ogVolume * t;

            yield return null;
        }

        audioSource.volume = finalPercentage;
    }

    public void FadeIn(float finalPercentage = 1, float extraSpeed = 1)
    {
        StartCoroutine(FadingIn(finalPercentage, extraSpeed));
    }

    private IEnumerator FadingIn(float finalPercentage, float extraSpeed)
    {
        float t = audioSource.volume;
        float speed = 1.0f;

        if (outFadedVolume != -1)
        {
            float x = finalPercentage;
            finalPercentage = outFadedVolume * x;

            outFadedVolume = -1;
        }

        while (t <= finalPercentage)
        {
            t += Time.deltaTime * speed * extraSpeed;

            audioSource.volume = t;

            yield return null;
        }

        audioSource.volume = finalPercentage;
    }
}
