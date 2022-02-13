using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SelectionCar : MonoBehaviour
{
    public int CarIndex = 0;

    [SerializeField]
    private MeshRenderer[] renderers;

    [SerializeField]
    private IntArray[] materialIndexToChange;

    public Material AppliedMat { get; set; } = null;

    public void ApplyMaterial(Material mat, bool singleColor = true)
    {
        AppliedMat = mat;

        if (singleColor)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (materialIndexToChange != null && materialIndexToChange[i].Ints != null)
                {
                    var mats = renderers[i].sharedMaterials.ToList();

                    for (int h = 0; h < materialIndexToChange[i].Ints.Length; h++)
                    {
                        mats[materialIndexToChange[i].Ints[h]] = mat;
                    }
                    
                    renderers[i].materials = mats.ToArray();
                }
                else
                {
                    var mats = renderers[i].sharedMaterials.ToList();
                    int length = mats.Count;
                    mats.Clear();
                    for (int g = 0; g < length; g++)
                    {
                        mats.Add(mat);
                    }

                    renderers[i].materials = mats.ToArray();

                }
            }
        }
    }
}

[System.Serializable]
public struct IntArray
{
    public int[] Ints;
}
