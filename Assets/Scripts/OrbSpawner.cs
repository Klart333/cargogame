using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public const float WhiteOrbChance  = 0.40f;
    public const float GreenOrbChance  = 0.25f;
    public const float BlueOrbChance   = 0.15f;
    public const float PurpleOrbChance = 0.1f;
    public const float YellowOrbChance = 0.05f;

    private const float HoursBetweenSpawns = 16.0f;

    [SerializeField]
    private OrbSpawn[] orbSpawns;

    [SerializeField]
    private LootOrb[] orbs;

    private int trackIndex = 0;

    private void Start()
    {
        trackIndex = GameManager.Instance.GetTrackIndex();
        float timeSinceSpawn = TimeManager.GetTimeSinceLastSpawn(trackIndex);
        print("Hours since last spawn: " + timeSinceSpawn);

        if (timeSinceSpawn >= HoursBetweenSpawns)
        {
            print("Spawning Orb");
            SpawnOrbs();
        }
    }

    private void SpawnOrbs() 
    {
        for (int i = 0; i < orbSpawns.Length; i++)
        {
            LootOrb orbToSpawn = GetOrb(orbSpawns[i].probabilityWeight);
            if (orbToSpawn != null)
            {
                Instantiate(orbToSpawn, orbSpawns[i].orbSpawn.position, Quaternion.identity);
            } 
        }
    }

    private LootOrb GetOrb(float prob)
    {
        float orb1 = UnityEngine.Random.Range(0.0f, 1.0f);
        if (orb1 >= Mathf.Pow(1.0f - YellowOrbChance, prob))
        {
            return orbs[4];
        }

        float orb2 = UnityEngine.Random.Range(0.0f, 1.0f);
        if (orb2 >= Mathf.Pow(1.0f - PurpleOrbChance, prob))
        {
            return orbs[3];
        }

        float orb3 = UnityEngine.Random.Range(0.0f, 1.0f);
        if (orb3 >= Mathf.Pow(1.0f - BlueOrbChance, prob))
        {
            return orbs[2];
        }

        float orb4 = UnityEngine.Random.Range(0.0f, 1.0f);
        if (orb4 >= Mathf.Pow(1.0f - GreenOrbChance, prob))
        {
            return orbs[1];
        }

        float orb5 = UnityEngine.Random.Range(0.0f, 1.0f);
        if (orb5 >= Mathf.Pow(1.0f - WhiteOrbChance, prob))
        {
            return orbs[0];
        }

        return null;
    }

    public void PickUpOrb()
    {
        TimeManager.StoreOrbTime(trackIndex);
    }
}

[System.Serializable]
public struct OrbSpawn
{
    public Transform orbSpawn;
    public float probabilityWeight;
}
