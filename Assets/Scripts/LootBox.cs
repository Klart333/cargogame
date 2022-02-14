using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [Header("Debug")]
    public Rarity DebugBox;

    public int DebugBoxAmount = 1;

    [Header("Main")]
    [SerializeField]
    private Transform lootPosition;

    [SerializeField]
    private Transform colorCarPosition;

    [SerializeField]
    private GameObject fractured;

    [SerializeField]
    private GameObject lootBox;

    [Header("Loot")]
    [SerializeField]
    private GameObject[] lootColors;

    [SerializeField]
    private GameObject[] lootCars;

    [SerializeField]
    private GameObject[] lootAccesories;

    [SerializeField]
    private GameObject nothingBurger;

    [Header("Probabilty")]
    [SerializeField]
    private AnimationCurve carsCurve;

    [SerializeField]
    private AnimationCurve colorsCurve;

    [SerializeField]
    private AnimationCurve accesoriesCurve;

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
    private new Collider collider;

    private Rarity currentRarity;

    private bool idle = true;
    private bool hover = false;
    private bool open = false;
    private bool lootHere = false;
    private bool smolCar = false;

    public bool BoxAvailable { get; set; } = true;

    private void Start()
    {
        collider = GetComponent<Collider>();
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

        GameObject loot_gm = Instantiate(GetLoot(currentRarity, out int colorCarIndex), lootPosition.transform.position, Quaternion.identity);
        if (colorCarIndex != -1)
        {
            Instantiate(lootCars[colorCarIndex], colorCarPosition);
        }

        var loot = loot_gm.GetComponentInChildren<LootBoxLoot>();
        loot.OnCollected += Loot_OnCollected;

        var gm = Instantiate(fractured, transform);
        gm.transform.position = animator.transform.position;
        Destroy(animator.gameObject);

        psys.Play();
    }

    private void Loot_OnCollected(LootBoxLoot loot)
    {
        Save.RemoveOrb(currentRarity);

        if (smolCar)
        {
            LootBoxLoot lootBoxLoot = colorCarPosition.GetComponentInChildren<LootBoxLoot>();
            lootBoxLoot.StartCoroutine(lootBoxLoot.Shrink());
        }

        BoxAvailable = true;
        collider.enabled = false;
        loot.OnCollected -= Loot_OnCollected;
    }

    private GameObject GetLoot(Rarity boxRarity, out int carIndex)
    {
        carIndex = -1;
        float extraChance = 1;
        switch (boxRarity)
        {
            case Rarity.White:
                extraChance = 0.8f;
                break;
            case Rarity.Green:
                extraChance = 1.25f;
                break;
            case Rarity.Blue:
                extraChance = 2f;
                break;
            case Rarity.Purple:
                extraChance = 3.25f;
                break;
            case Rarity.Yellow:
                extraChance = 5.5f;
                break;
            default:
                break;
        }

        float carNum = UnityEngine.Random.Range(0.0f, 1.0f);
        if (carNum > Mathf.Pow(1 - 0.1f, extraChance))
        {
            return CarReward(extraChance);
        }

        float colorNum = UnityEngine.Random.Range(0.0f, 1.0f);
        if (colorNum > Mathf.Pow(1 - 0.3f, extraChance))
        {
            return ColorReward(extraChance, out carIndex);
        }

        float accesoryNum = UnityEngine.Random.Range(0.0f, 1.0f);
        if (accesoryNum > Mathf.Pow(1 - 0.4f, extraChance))
        {
            
        }

        return nothingBurger;
    }

    private GameObject ColorReward(float extraChance, out int carIndex)
    {
        var cars = Save.GetUnlockedCars();
        carIndex = UnityEngine.Random.Range(0, lootCars.Length);
        int num = 0;
        while (!cars[carIndex] && num++ < 2)
        {
            carIndex = UnityEngine.Random.Range(0, lootCars.Length); // Some extra chance to get a color for a car you got
        }

        var colors = Save.GetUnlockedColors(carIndex);

        int unlocked = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i])
            {
                unlocked++;
            }
        }

        if (unlocked >= colors.Length)
        {
            return nothingBurger;
        }
        else if (unlocked >= colors.Length - 1)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                if (!colors[i])
                {
                    return lootColors[i];
                }
            }
        }

        int colorIndex = RandomFromCurve(lootColors.Length - 1, colorsCurve, extraChance, out int index);
        int EMERGENCY = 0;
        while (colors[colorIndex] && EMERGENCY++ < 50)
        {
            colorIndex = RandomFromCurve(lootColors.Length - 1, colorsCurve, extraChance, out index);
        }
        if (EMERGENCY >= 50)
        {
            Debug.Log("We couldn't find a color that wasn't unlocked?");
            return nothingBurger;
        }

        colors[index] = true;
        Save.SetUnlockedColors(carIndex, colors);

        return lootColors[colorIndex];
    }

    private GameObject CarReward(float extraChance)
    {
        var cars = Save.GetUnlockedCars();
        int unlocked = 0;
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i])
            {
                unlocked++;
            }
        }

        if (unlocked >= cars.Length)
        {
            return nothingBurger;
        }
        else if (unlocked >= cars.Length - 1)
        {
            for (int i = 0; i < cars.Length; i++)
            {
                if (!cars[i])
                {
                    return lootCars[i];
                }
            }
        }

        int carIndex = RandomFromCurve(lootCars.Length - 1, carsCurve, extraChance, out int index);
        int EMERGENCY = 0;
        while (cars[carIndex] && EMERGENCY++ < 50)
        {
            carIndex = RandomFromCurve(lootCars.Length - 1, carsCurve, extraChance, out index);
        }
        if (EMERGENCY >= 50)
        {
            Debug.LogError("We couldn't find a car that wasn't unlocked?");
            return nothingBurger;
        }

        Save.SetUnlockedCars(index);

        return lootCars[carIndex];
    }

    private int RandomFromCurve(int maxIndex, AnimationCurve curve, float extraChance, out int index)
    {
        for (int i = maxIndex; i >= 0; i--)
        {
            float num = UnityEngine.Random.Range(0.0f, 1.0f);
            if (num > Mathf.Pow(1.0f - curve.Evaluate(i), extraChance))
            {
                index = i;
                return i;
            }
        }

        index = 0;
        return 0;
    }

    public void LoadBox(Rarity rarity)
    {
        currentRarity = rarity;

        lootHere = true;
        BoxAvailable = false;
        collider.enabled = true;

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

[System.Serializable]
public struct LootColors
{
    public GameObject[] _LootColors;
}
