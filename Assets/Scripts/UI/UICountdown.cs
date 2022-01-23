using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UICountdown : MonoBehaviour
{
    private TextMeshProUGUI text;

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
            timer -= Time.deltaTime;

            text.text = Mathf.FloorToInt(timer).ToString();

            if (timer < 3)
            {
                text.color = Color.green;
            }

            if (timer < 2)
            {
                text.color = Color.yellow;
            }

            if (timer < 1)
            {
                text.color = Color.red;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
