using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarPathFind : MonoBehaviour
{
    [SerializeField]
    private bool amIAnAI;
    private NavMeshAgent carAgent;
    public Transform[] checkPoints = new Transform[99];
    public GameObject[] checkPointsBlockers = new GameObject[99];
    private Vector3[] checkpointV = new Vector3[31];
    private int checkpointSpot = 0;
    public GameObject playerCar = null;
    public GameObject aiCar = null;
    private NavMeshAgent playerAgent;
    private bool lapdone;
    void Start()
    {
        carAgent =  aiCar.GetComponent<NavMeshAgent>();
        carAgent.updateRotation = true;

        for (int i = 0; i < checkPoints.Length; i++)
        {
            checkpointV[i] = checkPoints[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(amIAnAI == true){
            if(checkpointSpot >= checkPoints.Length){
                checkpointSpot = 0;
                ResetCheckpoints();
            }
            carAgent.destination = checkPoints[checkpointSpot].position;
        }

        if(GameManager.Instance.TrackDone == true){
            lapdone = true;

            playerCar.GetComponent<CarMovement>().UsePlayerInput = false;
            
            carAgent.enabled = false;
            aiCar.transform.position = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other) {
        
        if(amIAnAI == true){
            if(other.GetComponent<LapHandler>() != null){
                checkpointSpot = 0;
                ResetCheckpoints();
            }

            if(other.gameObject.tag == "ai-guide"){
                other.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); 
                checkpointSpot += 1;
            }
        }
    }

    private void ResetCheckpoints(){
        if(lapdone == true){
            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i].position = checkpointV[i];
            }

            lapdone = false;
        }
    }
}
