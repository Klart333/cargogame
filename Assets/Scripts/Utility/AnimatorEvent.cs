using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    public event Action OnEvent = delegate { };

    public void AnimatorCallEvent()
    {
        OnEvent();
    }
}
