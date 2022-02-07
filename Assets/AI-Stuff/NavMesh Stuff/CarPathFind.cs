using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarPathFind : MonoBehaviour
{
    private NavMeshAgent carAgent;
    public Transform[] checkPoints = new Transform[99];
    public GameObject[] checkPointsBlockers = new GameObject[99];
    private Vector3[] checkpointV = new Vector3[31];
    private int checkpointSpot = 0;
    public GameObject playerCar = null;
    private NavMeshAgent playerAgent;
    private CarPathFind playerPathfinder;
    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
        carAgent.updateRotation = true;

        playerAgent = playerCar.GetComponent<NavMeshAgent>();
        
        for (int i = 0; i < checkPoints.Length; i++)
        {
            checkpointV[i] = checkPoints[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        carAgent.destination = checkPoints[checkpointSpot].position;
        
        if(GameManager.Instance.TrackDone == true){
            playerAgent.enabled = true;
            playerPathfinder.enabled = true;

            ResetCheckpoints();

            playerCar.GetComponent<CarMovement>().UsePlayerInput = false;
        }

        if(playerAgent.enabled == true){
            
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Before: " + checkpointSpot);

        if(other.GetComponent<LapHandler>() == true){
            checkpointSpot = 0;
            ResetCheckpoints();
        }

        if(other.gameObject.tag == "ai-guide"){
            other.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); 
            checkpointSpot += 1;
        }

        //checkpointSpot = other.GetComponent<Checkpoint>().Index+1;
        //checkPointsBlockers[checkpointSpot-1].SetActive(true);
        Debug.Log("After: " + checkpointSpot);
    }

    private void ResetCheckpoints(){
        for (int i = 0; i < checkPoints.Length; i++)
        {
            checkPoints[i].position = checkpointV[i];
        }
    }
}
