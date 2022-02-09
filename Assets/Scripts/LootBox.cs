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
    private GameObject fractured;

    [SerializeField]
    private GameObject lootBox;

    [Header("Screen Shake")]
    [SerializeField]
    private float amplitude = 20;

    [SerializeField]
    private float frequency = 1.5f;

    [SerializeField]
    private float length = 0.3f;

    private AnimatorEvent animatorEvent;
    private ParticleSystem psys;
    private Animator animator;

    private bool idle = true;
    private bool hover = false;
    private bool open = false;
    private bool lootHere = false;

    public bool BoxAvailable { get; set; } = true;

    private void Start()
    {
        psys = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (!lootHere)
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

        lootHere = false;

        GameObject loot_gm = Instantiate(GetLoot(), lootPosition.transform.position, Quaternion.identity);
        var loot = loot_gm.GetComponentInChildren<LootBoxLoot>();
        loot.OnCollected += Loot_OnCollected;

        var gm = Instantiate(fractured, transform);
        gm.transform.position = animator.transform.position;
        Destroy(animator.gameObject);

        psys.Play();
    }

    private void Loot_OnCollected(LootBoxLoot loot)
    {
        BoxAvailable = true;
        loot.OnCollected -= Loot_OnCollected;
    }

    private GameObject GetLoot()
    {
        return lootObject;
    }

    public void LoadBox(Rarity rarity)
    {
        lootHere = true;
        BoxAvailable = false;

        var box = Instantiate(lootBox, transform);

        animator = box.GetComponent<Animator>();
        animatorEvent = animator.gameObject.GetComponent<AnimatorEvent>();

        animatorEvent.OnEvent += LootReward;

        StartCoroutine(GrowUpPlease(box.transform));
    }

    private IEnumerator GrowUpPlease(Transform trn)
    {
        Vector3 targetScale = trn.localScale;

        float t = 0;
        float speed = 1;

        while (t <= 1)
        {
            t += Time.deltaTime * speed;

            trn.localScale = Vector3.Slerp(Vector3.zero, targetScale, Mathf.SmoothStep(0.0f, 1.0f, t));

            yield return null;
        }

        trn.localScale = targetScale;
    }
}
