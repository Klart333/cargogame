using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UILapTimer : MonoBehaviour
{
    private LapHandler lapHandler;
    private TextMeshProUGUI text;

    private bool displaying = false;
    private float timer = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        lapHandler = FindObjectOfType<LapHandler>();
        lapHandler.OnStartLap += LapHandler_OnStartLap;
        lapHandler.OnEndLap += LapHandler_OnEndLap;
    }

    private void Update()
    {
        if (displaying)
        {
            timer += Time.deltaTime;
            float fakeTimer = timer;
            int minuteTens = Mathf.FloorToInt(fakeTimer / 600.0f);
            fakeTimer -= minuteTens * 600;
            int minutes = Mathf.FloorToInt(fakeTimer / 60.0f);
            fakeTimer -= minutes * 60;
            int tens = Mathf.FloorToInt(fakeTimer / 10.0f);
            fakeTimer -= tens * 10;
            int seconds = Mathf.FloorToInt(fakeTimer);
            text.text = string.Format("{0}{1}:{2}{3}", minuteTens, minutes, tens , seconds);
        }
    }

    private void LapHandler_OnStartLap()
    {
        displaying = true;
        timer = 0;
    }

    private void LapHandler_OnEndLap()
    {
        displaying = false;
        GameManager.Instance.SaveTime(timer);
    }
}