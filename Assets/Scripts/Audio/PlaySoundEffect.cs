using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent soundEffect;

    public void PlaySound()
    {
        AudioManager.Instance.PlaySoundEffect(soundEffect);
    }
}
