using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointRestart : MonoBehaviour
{

    public Transform[] checkPoints = new Transform[9];
    public int checkpointSpot = -1;
    [SerializeField]
    private GameObject carObj;
    private Rigidbody carRB;

    private void Start() {
        carRB = carObj.GetComponent<Rigidbody>();
    }

    private void Update() {
        //if(Input.GetKeyDown(KeyCode.R)){
        //    carObj.transform.position = new Vector3(checkPoints[checkpointSpot].position.x,
        //                                            checkPoints[checkpointSpot].position.y,
        //                                            checkPoints[checkpointSpot].position.z);
        //
        //    carObj.transform.rotation = Quaternion.Euler(checkPoints[checkpointSpot].rotation.x, 
        //                                                 checkPoints[checkpointSpot].rotation.y,
        //                                                 checkPoints[checkpointSpot].rotation.z);
        //
        //    carRB.velocity = Vector3.zero;
        //    carRB.angularVelocity = Vector3.zero;
        //}
        //
        //if(Input.GetKeyDown(KeyCode.T)){
        //    checkpointSpot += 1;
        //}
        //
        //if(Input.GetKeyDown(KeyCode.Y)){
        //    checkpointSpot -= 1;
        //}
    }
}
