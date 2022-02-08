using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [SerializeField]
    private Transform lootPosition;

    [SerializeField]
    private GameObject lootObject;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject fractured;

    [Header("Screen Shake")]
    [SerializeField]
    private float amplitude = 20;

    [SerializeField]
    private float frequency = 1.5f;

    [SerializeField]
    private float length = 0.3f;

    private AnimatorEvent animatorEvent;
    private ParticleSystem psys;

    private bool idle = true;
    private bool hover = false;
    private bool open = false;
    private bool opened = false;

    private void Start()
    {
        psys = GetComponentInChildren < ParticleSystem>();
        animatorEvent = animator.gameObject.GetComponent<AnimatorEvent>();

        animatorEvent.OnEvent += LootReward;
    }

    private void Update()
    {
        if (opened)
        {
            return;
        }

        open = hover && Input.GetMouseButtonDown(0);

        animator.SetBool("Idle", idle);
        animator.SetBool("Hover", hover);
        animator.SetBool("Open", open);
    }

    private void OnMouseEnter()
    {
        idle = false;
        hover = true;
    }

    private void OnMouseExit()
    {
        idle = true;
        hover = false;
    }

    public void LootReward()
    {
        FindObjectOfType<CameraShake>().ScreenShake(amplitude, frequency, length);

        opened = true;

        Instantiate(GetLoot(), lootPosition.transform.position, Quaternion.identity);

        var gm = Instantiate(fractured, transform);
        gm.transform.position = animator.transform.position;
        Destroy(animator.gameObject);

        psys.Play();
    }

    private GameObject GetLoot()
    {
        return lootObject;
    }
}
