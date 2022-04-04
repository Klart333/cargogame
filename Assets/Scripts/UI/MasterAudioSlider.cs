using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterAudioSlider : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
        slider.value = savedVolume;
    }

    public void UpdateVolume()
    {
        float value = slider.value;
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}
