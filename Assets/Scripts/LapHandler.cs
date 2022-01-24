using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapHandler : MonoBehaviour
{
    public event Action OnStartLap = delegate { };
    public event Action OnEndLap = delegate { };

    [SerializeField]
    private GameObject finishPanel;

    private Checkpoint[] checkpoints;
    private CarMovement car;

    private int checkpointsGotten = 0;
    private int lastCheckpoint = -1;

    private void Start()
    {
        car = FindObjectOfType<CarMovement>();
        GetCheckpoints();
    }

    private void GetCheckpoints()
    {
        var points = FindObjectsOfType<Checkpoint>();
        checkpoints = new Checkpoint[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            for (int g = 0; g < points.Length; g++)
            {
                if (points[g].Index == i)
                {
                    checkpoints[i] = points[g];
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCar();
        }
    }

    public void StartLap()
    {
        OnStartLap();
    }

    private void ResetCar()
    {
        if (lastCheckpoint == -1)
        {
            GameManager.Instance.ReloadScene();
        }
        else
        {
            car.transform.position = checkpoints[lastCheckpoint].CarPosition;
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
        Instantiate(finishPanel, FindObjectOfType<Canvas>().transform);

        GameManager.Instance.TrackDone = true;
        OnEndLap();
    }
}
