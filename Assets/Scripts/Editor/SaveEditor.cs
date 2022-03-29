using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveFunctionality))]
public class SaveEditor : Editor
{
    private SaveFunctionality _target;

    public override void OnInspectorGUI()
    {
        if (_target == null)
        {
            _target = (SaveFunctionality)target;
        }

        if (GUILayout.Button("Initialize Loot"))
        {
            Save.SetUnlockedColors(0, new bool[8] { false, false, false, false, false, false, false, false });
            Save.SetUnlockedAccesories(0, new bool[3] { false, false, false });

            for (int i = 1; i < Save.AmountOfCars; i++)
            {
                Save.SetUnlockedCars(i, false);
                Save.SetUnlockedColors(i, new bool[8] { false, false, false, false, false, false, false, false });
                Save.SetUnlockedAccesories(i, new bool[3] { false, false, false});
            }
        }

        if (GUILayout.Button("Show Unlocked Cars And Colors"))
        {
            var cars = Save.GetUnlockedCars();
            for (int i = 0; i < cars.Length; i++)
            {
                var colors = Save.GetUnlockedColors(i);
                var accesories = Save.GetUnlockedAccesories(i);
                Debug.Log(string.Format("Car {0} is Unlocked?: {1} \nColors: {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9} \nAccesories: {10}, {11}, {12}",
                    i, cars[i], colors[0], colors[1], colors[2], colors[3], colors[4], colors[5], colors[6], colors[7], accesories[0], accesories[1], accesories[2]));
            }
        }

        base.OnInspectorGUI();
    }
}
