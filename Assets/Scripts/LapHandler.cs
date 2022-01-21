using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapHandler : MonoBehaviour
{
    public event Action OnStartLap = delegate { };
    public event Action OnEndLap = delegate { };

    private Checkpoint[] checkpoints;
    private CarMovement car;

    private int checkpointsGotten = 0;
    private int lastCheckpoint = -1;

    private float offsetX = -30;
    private float offsetY = 10;

    private void Start()
    {
        car = FindObjectOfType<CarMovement>();
        checkpoints = GetComponentsInChildren<Checkpoint>();

        OnStartLap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCar();
        }
    }

    private void ResetCar()
    {
        if (lastCheckpoint == -1)
        {
            GameManager.Instance.ReloadScene();
        }
        else
        {
            Vector3 xOffset = checkpoints[lastCheckpoint].transform.right * offsetX;
            Vector3 yOffset = checkpoints[lastCheckpoint].transform.up * offsetY;
            car.transform.position = checkpoints[lastCheckpoint].transform.position + xOffset + yOffset;
            car.transform.rotation = checkpoints[lastCheckpoint].CarRotation;

            var carRigidbody = car.GetComponent<Rigidbody>();
            carRigidbody.velocity = checkpoints[lastCheckpoint].CarVelocity;
            carRigidbody.angularVelocity = checkpoints[lastCheckpoint].CarAngularVelocity;
        }
    }

    public void GetCheckPoint(int index)
    {
        lastCheckpoint = index;
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
