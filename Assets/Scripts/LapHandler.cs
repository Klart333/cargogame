using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapHandler : MonoBehaviour
{
    public event Action OnStartLap = delegate { };
    public event Action OnEndLap = delegate { };

    private Checkpoint[] checkpoints;

    private int checkpointsGotten = 0;

    private void Start()
    {
        checkpoints = GetComponentsInChildren<Checkpoint>();

        OnStartLap();
    }

    public void GetCheckPoint()
    {
        checkpointsGotten += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkpointsGotten >= checkpoints.Length && other.GetComponent<CarMovement>() != null)
        {
            CompleteLap();
        }
    }

    private void CompleteLap()
    {
        OnEndLap();
    }
}
