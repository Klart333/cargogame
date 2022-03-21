using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LootMenuController : MonoBehaviour
{
    public event Action OnInLoot = delegate { };

    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [SerializeField]
    private Transform inLootTransform;

    [SerializeField]
    private SimpleAudioEvent clickSound;

    private Camera mainCam;
    private CinemachineBrain brain;
    private Canvas canvas;

    private List<GameObject> toggledUI = new List<GameObject>();

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
        if (PlayerPrefs.GetInt("DriveInstructions") == 0)
        {
            if (PlayerPrefs.GetInt("GetOrbs") == 0)
            {
                PlayerPrefs.SetInt("GetOrbs", 1);
                Save.SaveOrb(Rarity.White);
                Save.SaveOrb(Rarity.White);
                Save.SaveOrb(Rarity.White);

                Save.SaveOrb(Rarity.Green);
                Save.SaveOrb(Rarity.Green);

                Save.SaveOrb(Rarity.Blue);
            }


            gameObject.SetActive(false);
            return;
        }

        inMenuRotation = transform.rotation;
        inLootRotation = inLootTransform.rotation;
        inMenuScale = transform.localScale;
        inLootPosition = inLootTransform.position;
        inMenuPosition = transform.position;

        mainCam = Camera.main;
        brain = mainCam.GetComponent<CinemachineBrain>();
        canvas = FindObjectOfType<Canvas>();

        easeBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, transitionTime);
        defBlend = brain.m_DefaultBlend;
    }

    private void OnMouseDown()
    {
        AudioManager.Instance.PlaySoundEffect(clickSound);

        if (inTransit)
        {
            return;
        }

        if (!inLoot)
        {
            inLoot = true;
            ToggleUI(false);

            StartCoroutine(Transit(inLootPosition, inLootScale, inLootRotation));
            vcam.Priority = 100;
        }
        else
        {
            inLoot = false;
            ToggleUI(true);

            StartCoroutine(Transit(inMenuPosition, inMenuScale, inMenuRotation));
            vcam.Priority = 1;
        }
    }

    private void ToggleUI(bool toggle)
    {
        if (!toggle)
        {
            toggledUI.Clear();
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                GameObject gm = canvas.transform.GetChild(i).gameObject;
                toggledUI.Add(gm);
                gm.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < toggledUI.Count; i++)
            {
                toggledUI[i].SetActive(true);
            }
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

        if (targetPosition == inLootPosition)
        {
            OnInLoot();
        }
    }
}
