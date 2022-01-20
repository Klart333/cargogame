using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private LapHandler lapHandler;
    private bool gettable = true;

    private CheckpointRestart restart;

    private void Start()
    {
        lapHandler = GetComponentInParent<LapHandler>();
        restart = GetComponentInParent<CheckpointRestart>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gettable && other.GetComponent<CarMovement>() != null)
        {
            gettable = false;
            restart.checkpointSpot += 1;
            lapHandler.GetCheckPoint();
        }
    }
}
