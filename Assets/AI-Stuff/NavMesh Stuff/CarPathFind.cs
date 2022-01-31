using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarPathFind : MonoBehaviour
{
    private NavMeshAgent carAgent;
    public Transform[] checkPoints = new Transform[99];
    private int checkpointSpot = 0;
    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        carAgent.destination = checkPoints[checkpointSpot].position;
        //carAgent.SetDestination(checkPoints[checkpointSpot].position);
        if(carAgent.destination == checkPoints[checkpointSpot].position){
            checkpointSpot += 1;
        }
    }
}
