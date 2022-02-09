using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOrb : MonoBehaviour
{
    [SerializeField]
    private Rarity rarity;

    private LootBox lootBox;
    private new Rigidbody rigidbody;

    private void OnMouseDown()
    {
        lootBox = FindObjectOfType<LootBox>();
        if (!lootBox.BoxAvailable)
        {
            return;
        }

        rigidbody = GetComponentInChildren<Rigidbody>();
        rigidbody.isKinematic = true;

        lootBox.BoxAvailable = false;

        StartCoroutine(ClickOrb());
    }

    private IEnumerator ClickOrb()
    {
        Vector3 ogPos = transform.position;
        Vector3 targetPos = lootBox.transform.position;
        Vector3 ogScale = transform.localScale;

        float t = 0;
        float speed = 1;

        while (t <= 1)
        {
            t += Time.deltaTime * speed;

            transform.position = Vector3.Lerp(ogPos, targetPos, Mathf.SmoothStep(0.0f, 1.0f, t));

            yield return null;
        }
        transform.position = targetPos;

        t = 0;

        lootBox.LoadBox(rarity);
        while (t <= 1)
        {
            t += Time.deltaTime * speed;

            transform.localScale = Vector3.Lerp(ogScale, Vector3.zero, Mathf.SmoothStep(0.0f, 1.0f, t));

            yield return null;
        }
    }
}
