using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [SerializeField]
    private OrbSpawn[] orbSpawns;
}

[System.Serializable]
public struct OrbSpawn
{
    public Transform orbSpawn;
    public float probabilityWeight;
}
