using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICheckpoints : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Checkpoint[] checkpoints;

    private int numGot = 0;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        checkpoints = FindObjectsOfType<Checkpoint>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].OnGot += UICheckpoints_OnGot;
        }

        FindObjectOfType<LapHandler>().OnStartLap += UICheckpoints_OnStartLap;
    }

    private void UICheckpoints_OnStartLap()
    {
        text.text = string.Format("{0}/{1}", numGot, checkpoints.Length);
    }

    private void UICheckpoints_OnGot()
    {
        numGot++;
        text.text = string.Format("{0}/{1}", numGot, checkpoints.Length);
    }
}
