using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LootBox))]
public class LootBoxEditor : Editor
{
    private LootBox _target;

    public override void OnInspectorGUI()
    {
        _target = (LootBox)target;

        if (GUILayout.Button("Get Box"))
        {
            for (int i = 0; i < _target.DebugBoxAmount; i++)
            {
                Save.SaveOrb(_target.DebugBox);
            }
        } 

        base.OnInspectorGUI();  
    }

}
