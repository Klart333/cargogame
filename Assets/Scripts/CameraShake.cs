using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ScreenShake(float amplitudeGain, float frequencyGain, float time)
    {
        StartCoroutine(Shake(amplitudeGain, frequencyGain, time));
        
    }

    private IEnumerator Shake(float amplitudeGain, float frequencyGain, float time)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;

        yield return new WaitForSeconds(time);

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }                           
}
