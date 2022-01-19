using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private LapHandler lapHandler;
    private bool gettable = true;

    private void Start()
    {
        lapHandler = GetComponentInParent<LapHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gettable && other.GetComponent<CarMovement>() != null)
        {
            gettable = false;
            lapHandler.GetCheckPoint();
        }
    }
}
