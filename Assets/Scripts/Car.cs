using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public int CarIndex = 0;

    private void Awake()
    {
        var wheels = GetComponentsInChildren<WheelSkid>();
        var skid = FindObjectOfType<Skidmarks>();
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].skidmarksController = skid;
        }
    }
}
