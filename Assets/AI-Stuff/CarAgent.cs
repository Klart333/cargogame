using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement;

public class CarAgent : Agent
{
    private Rigidbody rBody;
    private Transform tForm;
    private Vector3 startPos;
  //  public Rigidbody[] wheelsRB = new Rigidbody[4];
    public Transform[] wheelsTF = new Transform[4];
    private Vector3[] wheelsV3 = new Vector3[4];
    public Transform[] checkPoints = new Transform[99];
    public CarMovement carScript;

    private void Start() {
        rBody = GetComponent<Rigidbody>();
        tForm = GetComponent<Transform>();
        startPos = tForm.position;

        for (int i = 0; i < wheelsTF.Length; i++)
        {
            wheelsV3[i] = wheelsTF[i].position;
        }

    }

    public override void OnEpisodeBegin(){
        tForm.position = startPos;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        tForm.rotation = Quaternion.identity;

        for (int i = 0; i < wheelsTF.Length; i++)
        {
            //wheelsRB[i].velocity = Vector3.zero;
            //wheelsRB[i].angularVelocity = Vector3.zero;
            wheelsTF[i].position = wheelsV3[i];
            wheelsTF[i].rotation = Quaternion.identity;
        }
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(rBody.angularVelocity);
        sensor.AddObservation(tForm.position);
        sensor.AddObservation(tForm.rotation);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers){
        carScript.SetInputs(actionBuffers.ContinuousActions[0],0,actionBuffers.ContinuousActions[1]);
    }

    public override void Heuristic(in ActionBuffers actionsOut){
    var continuousActionsOut = actionsOut.ContinuousActions;
    continuousActionsOut[0] = Input.GetAxis("Horizontal");
    continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision other) {
        if(other.transform.tag == "death"){
            //SetReward();
            EndEpisode();
        }
    }

}
