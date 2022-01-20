using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector]
    public Quaternion CarRotation;

    [HideInInspector]
    public Vector3 CarVelocity;

    [HideInInspector]
    public Vector3 CarAngularVelocity;

    [SerializeField]
    private int index;

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
            CarRotation = other.gameObject.transform.rotation;
            var rb = other.gameObject.GetComponent<Rigidbody>();
            CarVelocity = rb.velocity;
            CarAngularVelocity = rb.angularVelocity;

            gettable = false;
            lapHandler.GetCheckPoint(index);
        }
    }
}
