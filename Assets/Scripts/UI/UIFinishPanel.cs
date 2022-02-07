using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    private UILapTimer lapTimer;

    private void Start()
    {
        lapTimer = FindObjectOfType<UILapTimer>();

        timeText.text = lapTimer.DisplayTime(lapTimer.Timer);

        int index = GameManager.Instance.GetTrackIndex();
        for (int i = 0; i < stars.Length; i++)
        {
            if (lapTimer.Timer < Save.AllStarTimes[index].Times[i])
            {
                stars[i].color = Color.yellow;
            }
        }
    }

    public void ShowOrb(float shineTime, Rarity rarity)
    {
        StartCoroutine(ShowingOrb(shineTime, rarity));
    }

    private IEnumerator ShowingOrb(float time, Rarity rarity)
    {
        float t = 0;

        Color alphaColor = orbLight.color;

        while (t <= 1)
        {
            t += Time.deltaTime * (1.0f / time);

            alphaColor.a = 255 * (1.0f - t);
            orbLight.color = alphaColor;

            yield return null;
        }

        alphaColor.a = 0;
        orbLight.color = alphaColor;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);   
    }
}
