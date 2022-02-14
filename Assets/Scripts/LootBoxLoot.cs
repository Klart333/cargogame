using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxLoot : MonoBehaviour
{
    public event Action<LootBoxLoot> OnCollected = delegate { };

    [SerializeField]
    private bool shrinkParent = false;

    private bool shrinking = false;

    private void Start()
    {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        Vector3 ogScale = transform.localScale;
        if (shrinkParent)
        {
            ogScale = transform.parent.localScale;
        }

        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime;

            if (shrinkParent)
            {
                transform.parent.localScale = Vector3.Lerp(Vector3.zero, ogScale, Mathf.SmoothStep(0.0f, 1.0f, t));
            }
            else
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, ogScale, Mathf.SmoothStep(0.0f, 1.0f, t));
            }

            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if (shrinking)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(Shrink());
    }

    public IEnumerator Shrink()
    {
        shrinking = true;

        Vector3 ogScale = transform.localScale;
        if (shrinkParent)
        {
            ogScale = transform.parent.localScale;
        }

        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime;

            if (shrinkParent)
            {
                transform.parent.localScale = Vector3.Slerp(ogScale, Vector3.zero, t);
            }
            else
            {
                transform.localScale = Vector3.Slerp(ogScale, Vector3.zero, t);
            }

            yield return null;
        }

        Collect();
    }

    private void Collect()
    {
        OnCollected(this);

        Destroy(gameObject);
    }
}
