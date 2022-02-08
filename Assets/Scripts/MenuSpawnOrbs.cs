using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawnOrbs : MonoBehaviour
{
    [SerializeField]
    private GameObject orb;

    [SerializeField]
    private Rarity rarity;

    private Vector3 spawnOffset = new Vector3(0, 1, 0);

    private void Start()
    {
        var orbs = Save.GetOrbs();

        for (int i = 0; i < orbs.Count; i++)
        {
            if (orbs[i] == rarity)
            {
                SpawnOrb();
            }
        }
    }

    public void SpawnOrb()
    {
        Vector3 rand = new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), 0, UnityEngine.Random.Range(0.0f, 1.0f));
        Instantiate(orb, transform.position + spawnOffset + rand, Quaternion.identity);
    }
}
