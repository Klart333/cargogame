using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LootMenuController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [SerializeField]
    private Transform inLootTransform;

    private Camera mainCam;
    private CinemachineBrain brain;

    private CinemachineBlendDefinition easeBlend;
    private CinemachineBlendDefinition defBlend;
    private Vector3 inLootPosition;
    private Vector3 inMenuPosition;
    private Vector3 inLootScale = new Vector3(100, 100, 100);
    private Vector3 inMenuScale = new Vector3(270, 270, 270);
    private Quaternion inLootRotation;
    private Quaternion inMenuRotation;

    private bool inLoot = false;
    private float transitionTime = 1.5f;
    private bool inTransit = false;

    private void Start()
    {
        inMenuRotation = transform.rotation;
        inLootRotation = inLootTransform.rotation;
        inMenuScale = transform.localScale;
        inLootPosition = inLootTransform.position;
        inMenuPosition = transform.position;

        mainCam = Camera.main;
        brain = mainCam.GetComponent<CinemachineBrain>();

        easeBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, transitionTime);
        defBlend = brain.m_DefaultBlend;
    }

    private void OnMouseDown()
    {
        if (inTransit)
        {
            return;
        }

        if (!inLoot)
        {
            inLoot = true;
            StartCoroutine(Transit(inLootPosition, inLootScale, inLootRotation));
            vcam.Priority = 100;
        }
        else
        {
            inLoot = false;

            StartCoroutine(Transit(inMenuPosition, inMenuScale, inMenuRotation));
            vcam.Priority = 1;
        }
    }

    private IEnumerator Transit(Vector3 targetPosition, Vector3 targetScale, Quaternion targetRotation)
    {
        inTransit = true;
        brain.m_DefaultBlend = easeBlend;

        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.rotation;

        float t = 0;
        float speed = 1.0f / transitionTime;

        while (t <= 1)
        {
            t += Time.deltaTime * speed;

            transform.position = Vector3.Slerp(startPosition, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
            transform.localScale = Vector3.Slerp(startScale, targetScale, Mathf.SmoothStep(0.0f, 1.0f, t));
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, Mathf.SmoothStep(0.0f, 1.0f, t));

            yield return null;
        }

        transform.position = targetPosition;
        inTransit = false;

        brain.m_DefaultBlend = defBlend;
    }
}
