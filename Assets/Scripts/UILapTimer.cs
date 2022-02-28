using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UILapTimer : MonoBehaviour
{
    [SerializeField]
    private bool UI = true;

    private LapHandler lapHandler;
    private TextMeshProUGUI text;
    private TextMeshPro text3D;

    private bool displaying = false;

    public float Timer { get; private set; } = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text3D = GetComponent<TextMeshPro>();

        lapHandler = FindObjectOfType<LapHandler>();
        lapHandler.OnStartLap += LapHandler_OnStartLap;
        lapHandler.OnEndLap += LapHandler_OnEndLap;
    }

    private void Update()
    {
        if (displaying)
        {
            Timer += Time.deltaTime;
            var timeString = DisplayTime(Timer, UI);

            if (UI)
            {
                text.text = timeString;
            }
            else
            {
                text3D.text = timeString;
            }
        }
    }

    public string DisplayTime(float time, bool displayTenths = false)
    {
        float fakeTimer = time;
        int minuteTens = Mathf.FloorToInt(fakeTimer / 600.0f);
        fakeTimer -= minuteTens * 600;
        int minutes = Mathf.FloorToInt(fakeTimer / 60.0f);
        fakeTimer -= minutes * 60;
        int tens = Mathf.FloorToInt(fakeTimer / 10.0f);
        fakeTimer -= tens * 10;
        int seconds = Mathf.FloorToInt(fakeTimer);
        fakeTimer -= seconds;
        int tenths = Mathf.FloorToInt(fakeTimer * 10);
        fakeTimer -= tenths / 10.0f;
        int hundreths = Mathf.FloorToInt(fakeTimer * 100);

        if (displayTenths)
        {
            return string.Format("{0}{1}:{2}{3}:{4}{5}", minuteTens, minutes, tens, seconds, tenths, hundreths);
        }
        else
        {
            return string.Format("{0}{1}:{2}{3}", minuteTens, minutes, tens, seconds);
        }
    }

    private void LapHandler_OnStartLap()
    {
        displaying = true;
        Timer = 0;
    }

    private void LapHandler_OnEndLap()
    {
        displaying = false;
        if (UI)
        {
            GameManager.Instance.SaveTime(Timer, out bool best);
            FindObjectOfType<UIFinishPanel>().Best = best;
        }
    }
}
