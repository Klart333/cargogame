using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarPathFind : MonoBehaviour
{
    private NavMeshAgent carAgent;
    public Transform[] checkPoints = new Transform[99];
    public GameObject[] checkPointsBlockers = new GameObject[99];
    private int checkpointSpot = 0;
    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
        carAgent.updateRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        

        carAgent.destination = checkPoints[checkpointSpot].position;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Before: " + checkpointSpot);

        if(other.GetComponent<LapHandler>() == true){
            checkpointSpot = 0;
        }

        checkpointSpot = other.GetComponent<Checkpoint>().Index+1;
        checkPointsBlockers[checkpointSpot-1].SetActive(true);
        Debug.Log("After: " + checkpointSpot);
    }
}
