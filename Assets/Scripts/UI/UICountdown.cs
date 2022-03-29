using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UICountdown : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent[] countdownSounds;

    private TextMeshProUGUI text;

    private int lastNumber;

    public void StartCountdown(float time)
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Countdown(time));
    }

    private IEnumerator Countdown(float time)
    {
        float timer = time;
        while (timer > 0)
        {
            yield return null;

            int num = Mathf.CeilToInt(timer);
            text.text = num.ToString();

            if (num != lastNumber)
            {
                AudioManager.Instance.PlaySoundEffect(countdownSounds[num - 1]);
            }

            switch (num)
            {
                case 1:
                    text.color = Color.red;
                    break;
                case 2:
                    text.color = Color.yellow;
                    break;
                case 3:
                    text.color = Color.green;
                    break;
                default:
                    break;
            }

            lastNumber = num;

            timer -= Time.deltaTime;
        }

        Destroy(gameObject);
    }
}
