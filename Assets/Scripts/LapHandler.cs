using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

public class LapHandler : MonoBehaviour
{
    public event Action OnStartLap = delegate { };
    public event Action OnEndLap = delegate { };

    [SerializeField]
    private UIFinishPanel finishPanel;

    [SerializeField]
    private ParticleSystem finishParticle;

    [SerializeField]
    private Transform particlePosition;

    private Checkpoint[] checkpoints;
    private CarMovement car;
    private CinemachineVirtualCamera vcam;

    private int checkpointsGotten = 0;
    private int lastCheckpoint = -1;
    private bool finished = false;

    private void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
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
            StartCoroutine(DisableCameraSmoothingBriefly());
            ResetCar();
        }
    }

    private IEnumerator DisableCameraSmoothingBriefly()
    {
        var poser = vcam.GetCinemachineComponent<CinemachineTransposer>();
        poser.m_XDamping = 0;
        poser.m_YDamping = 0;
        poser.m_ZDamping = 0;

        yield return new WaitForSeconds(0.1f);

        poser.m_XDamping = 1;
        poser.m_YDamping = 1;
        poser.m_ZDamping = 1;
    }

    public void StartLap()
    {
        OnStartLap();
    }

    private void ResetCar()
    {
        if (lastCheckpoint == -1)
        {
            return;
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
        if (checkpointsGotten >= checkpoints.Length && other.GetComponent<CarMovement>() != null && other.tag == "Player")
        {
            CompleteLap();
        }
    }

    private void CompleteLap()
    {
        if (finished)
        {
            return;
        }

        finished = true;
        Instantiate(finishPanel, FindObjectOfType<Canvas>().transform);
        Instantiate(finishParticle, particlePosition);

        int trackIndex = GameManager.Instance.GetTrackIndex();
        Save.CompleteTrack(trackIndex);
        GameManager.Instance.TrackDone = true;
        OnEndLap();
    }
}
