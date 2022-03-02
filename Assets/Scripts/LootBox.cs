using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [Header("Debug")]
    public Rarity DebugBox;

    public int DebugBoxAmount = 1;

    [SerializeField]
    private bool displayProbability = true;

    [Header("Main")]
    [SerializeField]
    private Transform lootPosition;

    [SerializeField]
    private Transform colorCarPosition;

    [SerializeField]
    private GameObject fractured;

    [SerializeField]
    private GameObject lootBox;

    [SerializeField]
    private Material[] rarityMaterials;

    [Header("Loot")]
    [SerializeField]
    private GameObject[] lootColors;

    [SerializeField]
    private GameObject[] lootCars;

    [SerializeField]
    private GameObject[] lootAccesories;

    [SerializeField]
    private GameObject[] stickers;

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

        if (displayProbability)
        {
            DisplayProbability();
        }
    }

    private void DisplayProbability()
    {
        // Cars
        for (int i = 0; i < lootCars.Length; i++)
        {
            float prob = Probability(i, lootCars.Length - 1, carsCurve);
            print("Probability of Car " + i + ": " + prob + "\nCurve Probability: " + carsCurve.Evaluate((float)i / (float)(lootCars.Length)));
        }

        // Colors
        for (int i = 0; i < lootColors.Length; i++)
        {
            float prob = Probability(i, lootColors.Length - 1, colorsCurve);
            print("Probability of Color " + (i + 1) + ": " + prob + "\nCurve Probability: " + carsCurve.Evaluate((float)i / (float)(lootColors.Length)));
        }
    }

    private float Probability(int i, int maxIndex, AnimationCurve curve)
    {
        float prob = curve.Evaluate((float)i / (float)(maxIndex + 1));

        if (i < maxIndex)
        {
            prob *= 1.0f - Probability(i + 1, maxIndex, curve);
        }

        return prob;
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

        GameObject lootObject = GetLoot(currentRarity, out int colorCarIndex);
        GameObject loot_gm = Instantiate(lootObject, lootPosition.transform.position, lootObject.transform.rotation);
        if (colorCarIndex != -1)
        {
            smolCar = true;
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
            if (lootBoxLoot != null)
            {
                lootBoxLoot.StartCoroutine(lootBoxLoot.Shrink());
            }

            smolCar = false;
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
                extraChance = 1;
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
        if (carNum > Mathf.Pow(1 - 0.2f, extraChance))
        {
            return CarReward(extraChance);
        }

        float colorNum = UnityEngine.Random.Range(0.0f, 1.0f);
        if (colorNum > Mathf.Pow(1 - 0.35f, extraChance))
        {
            return ColorReward(extraChance, out carIndex);
        }

        float accesoryNum = UnityEngine.Random.Range(0.0f, 1.0f);
        if (accesoryNum > Mathf.Pow(1 - 0.6f, extraChance))
        {
            return AccesoryReward(extraChance, out carIndex);
        }

        return GetSticker();
    }

    private GameObject ColorReward(float extraChance, out int carIndex)
    {
        var cars = Save.GetUnlockedCars();
        carIndex = UnityEngine.Random.Range(0, lootCars.Length);
        int num = 0;
        while (!cars[carIndex] && num++ < 5)
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
            return GetSticker();
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
            return GetSticker();
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
            return GetSticker();
        }
        else if (unlocked >= cars.Length - 1)
        {
            for (int i = 0; i < cars.Length; i++)
            {
                if (!cars[i])
                {
                    Save.SetUnlockedCars(i);
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
            return GetSticker();
        }

        Save.SetUnlockedCars(index);

        return lootCars[carIndex];
    }

    private GameObject AccesoryReward(float extraChance, out int carIndex)
    {
        var cars = Save.GetUnlockedCars();
        carIndex = UnityEngine.Random.Range(0, lootCars.Length);
        int num = 0;
        while (!cars[carIndex] && num++ < 5)
        {
            carIndex = UnityEngine.Random.Range(0, lootCars.Length); // Some extra chance to get a color for a car you got
        }

        var accesories = Save.GetUnlockedAccesories(carIndex);

        int unlocked = 0;
        for (int i = 0; i < accesories.Length; i++)
        {
            if (accesories[i])
            {
                unlocked++;
            }
        }

        if (unlocked >= accesories.Length)
        {
            return GetSticker();
        }
        else if (unlocked >= accesories.Length - 1)
        {
            for (int i = 0; i < accesories.Length; i++)
            {
                if (!accesories[i])
                {
                    return lootAccesories[i];
                }
            }
        }

        int accesoryIndex = RandomFromCurve(lootAccesories.Length - 1, accesoriesCurve, extraChance, out int index);
        int EMERGENCY = 0;
        while (accesories[accesoryIndex] && EMERGENCY++ < 50)
        {
            accesoryIndex = RandomFromCurve(lootColors.Length - 1, colorsCurve, extraChance, out index);
        }
        if (EMERGENCY >= 50)
        {
            Debug.Log("We couldn't find a accesory that wasn't unlocked?");
            return GetSticker();
        }

        accesories[index] = true;
        Save.SetUnlockedAccesories(carIndex, accesories);

        return lootAccesories[accesoryIndex];
    }

    private GameObject GetSticker()
    {
        int stickerIndex = UnityEngine.Random.Range(0, stickers.Length);

        Save.AddSticker(stickerIndex);
        FindObjectOfType<StickerSpawner>().SpawnStickers();

        return stickers[stickerIndex];
    }

    private int RandomFromCurve(int maxIndex, AnimationCurve curve, float extraChance, out int index)
    {
        for (int i = maxIndex; i >= 0; i--)
        {
            float num = UnityEngine.Random.value;
            if (num > Mathf.Pow(1.0f - curve.Evaluate((float)i / maxIndex + 1), extraChance))
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
        ColorTheBoy(box, rarity);

        animator = box.GetComponent<Animator>();
        animatorEvent = animator.gameObject.GetComponent<AnimatorEvent>();

        animatorEvent.OnEvent += LootReward;

        StartCoroutine(GrowUpPlease(box.transform));
    }

    private void ColorTheBoy(GameObject box, Rarity rarity)
    {
        var mats = box.GetComponentInChildren<MeshRenderer>();
        switch (rarity)
        {
            case Rarity.White:
                mats.material = rarityMaterials[0];
                break;
            case Rarity.Green:
                mats.material = rarityMaterials[1];
                break;
            case Rarity.Blue:
                mats.material = rarityMaterials[2];
                break;
            case Rarity.Purple:
                mats.material = rarityMaterials[3];
                break;
            case Rarity.Yellow:
                mats.material = rarityMaterials[4];
                break;
            default:
                break;
        }
        
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
