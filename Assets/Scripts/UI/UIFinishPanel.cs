using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIFinishPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private Image[] stars;

    [SerializeField]
    private Image orb;

    [SerializeField]
    private Image orbLight;

    [SerializeField]
    private Color[] colors;

    [Header("Best")]
    [SerializeField]
    private float newBestDelay = 1.9f;

    [SerializeField]
    private GameObject bestText;

    [SerializeField]
    private SimpleAudioEvent pbSound;

    private UILapTimer lapTimer;

    public bool Best { get; set; } = false;

    private void Start()
    {
        lapTimer = FindObjectOfType<UILapTimer>();

        timeText.text = lapTimer.DisplayTime(lapTimer.Timer, true);

        int index = GameManager.Instance.GetTrackIndex();
        for (int i = 0; i < stars.Length; i++)
        {
            if (lapTimer.Timer < Save.AllStarTimes[index].Times[i])
            {
                stars[i].color = Color.yellow;
            }
        }

        if (Best)
        {
            AudioManager.Instance.PlaySoundEffect(pbSound);
            StartCoroutine(NewBestText());
        }
    }

    private IEnumerator NewBestText()
    {
        yield return new WaitForSeconds(newBestDelay);

        bestText.SetActive(true);
    }

    public void ShowOrb(float shineTime, Rarity rarity)
    {
        StartCoroutine(ShowingOrb(shineTime));

        switch (rarity)
        {
            case Rarity.White:
                orb.color = colors[0];
                break;
            case Rarity.Green:
                orb.color = colors[1];
                break;
            case Rarity.Blue:
                orb.color = colors[2];
                break;
            case Rarity.Purple:
                orb.color = colors[3];
                break;
            case Rarity.Yellow:
                orb.color = colors[4];
                break;
            default:
                break;
        }

        orb.gameObject.SetActive(true);
        orbLight.gameObject.SetActive(true);

        Save.SaveOrb(rarity);
    }

    private IEnumerator ShowingOrb(float time)
    {
        float t = 0;

        Color alphaColor = orbLight.color;

        while (t <= 1)
        {
            t += Time.deltaTime * (1.0f / time);

            alphaColor.a = (1.0f - t);
            orbLight.color = alphaColor;

            yield return null;
        }

        alphaColor.a = 0;
        orbLight.color = alphaColor;
    }

    public void GoToMenu()
    {
        GameManager.Instance.LoadLevel(0);
    }

    public void PlayAgain()
    {
        GameManager.Instance.ReloadScene();
    }
}
