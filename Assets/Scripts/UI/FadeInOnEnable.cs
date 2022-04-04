using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOnEnable : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed = 3f;

    private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * fadeSpeed;

            canvasGroup.alpha = t;

            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
