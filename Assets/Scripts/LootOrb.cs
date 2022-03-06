using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootOrb : MonoBehaviour
{
    public Rarity OrbRarity;

    [SerializeField]
    private GameObject shatteredOrb;

    [SerializeField]
    private Vector3 attachedOffset = new Vector3(0, 2, 0.2f);

    [SerializeField]
    private Light collectLight;

    [SerializeField]
    private float collectLightMax = 1000;

    private LapHandler lapHandler;
    private HingeJoint hinge;
    private new Rigidbody rigidbody;

    private bool attached = false;
    private float collectTime = 1.5f;
    private float collectDelay = 4f;

    private void Start()
    {
        lapHandler = FindObjectOfType<LapHandler>();
        hinge = GetComponent<HingeJoint>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void CollectOrb()
    {
        StartCoroutine(Collecting());
    }

    private IEnumerator Collecting()
    {
        yield return new WaitForSeconds(collectDelay);

        FindObjectOfType<UIFinishPanel>().ShowOrb(collectTime, OrbRarity);

        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * (1.0f / collectTime);

            collectLight.intensity = collectLightMax * t;
            yield return null;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject != collectLight.gameObject)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * (1.0f / collectTime);

            collectLight.intensity = collectLightMax * (1.0f - t);

            yield return null;
        }

        collectLight.intensity = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (attached)
        {
            CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                return;
            }

            Destroy(GetComponent<HingeJoint>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());

            var shattered = Instantiate(shatteredOrb, transform.GetChild(2));

            Destroy(transform.GetChild(0).gameObject);
            Destroy(transform.GetChild(1).gameObject);

            lapHandler.OnEndLap -= CollectOrb;
        }
        else
        {
            PickUp(collision);
        }

    }

    private void PickUp(Collision collision)
    {
        CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
        if (carMovement != null)
        {
            attached = true;

            rigidbody.isKinematic = false;
            transform.position = carMovement.transform.position + carMovement.transform.up * attachedOffset.y + carMovement.transform.forward * attachedOffset.z;
            hinge.connectedBody = carMovement.GetComponent<Rigidbody>();

            lapHandler.OnEndLap += CollectOrb;

            FindObjectOfType<OrbSpawner>().PickUpOrb();
        }
    }
}

public enum Rarity
{
    White,
    Green,
    Blue,
    Purple,
    Yellow
}