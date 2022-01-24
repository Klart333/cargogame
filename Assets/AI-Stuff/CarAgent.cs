using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement;

public class CarAgent : Agent
{
    public Rigidbody rBody;
    private Transform tForm;
    private Vector3 startPos;
    public Transform[] wheelsTF = new Transform[4];
    private Vector3[] wheelsV3 = new Vector3[4];
    public Transform[] checkPoints = new Transform[99];
    private Vector3[] checkPointsV = new Vector3[99];
    public CarMovement carScript;
    private int checkpointSpot = 0;
    public float timer;

    private void Start() {
        rBody = GetComponent<Rigidbody>();
        tForm = GetComponent<Transform>();
        startPos = tForm.position;

        for (int i = 0; i < wheelsTF.Length; i++)
        {
            wheelsV3[i] = wheelsTF[i].position;
        }

        for (int i = 0; i < checkPoints.Length; i++){
            checkPointsV[i] = checkPoints[i].position;
        }
    }

    public override void OnEpisodeBegin(){
        tForm.position = startPos;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        tForm.rotation = Quaternion.identity;

        for (int i = 0; i < wheelsTF.Length; i++){
            wheelsTF[i].position = wheelsV3[i];
            wheelsTF[i].rotation = Quaternion.identity;
        }

        for (int i = 0; i < checkPoints.Length; i++){
            checkPoints[i].position = checkPointsV[i];
        }

        timer = 50;
    }
    
    public override void CollectObservations(VectorSensor sensor){
        //sensor.AddObservation(rBody.velocity);
        //sensor.AddObservation(rBody.angularVelocity);
        //sensor.AddObservation(tForm.position);
        //sensor.AddObservation(tForm.rotation);
        sensor.AddObservation(Vector3.Distance(tForm.position, checkPoints[checkpointSpot].position));
    }

    public override void OnActionReceived(ActionBuffers actionBuffers){
        carScript.SetInputs((actionBuffers.DiscreteActions[0])-1,0,actionBuffers.ContinuousActions[0]);
        //AddReward(-0.05f);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[0] = Input.GetAxis("Vertical");
    }
    private void OnTriggerEnter(Collider other) {
        if(other.transform.tag == "Checkpoint"){
            other.transform.position = new Vector3(other.transform.position.x,other.transform.position.y+100,other.transform.position.z);
            AddReward(50);
            timer += 10;
            checkpointSpot += 1;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "death"){
            DroveOff_AI();
        }
    }

    private void Update() {
        if(checkpointSpot == checkPoints.Length){
            AddReward(100);
            EndEpisode();
        }

        timer -= 1*Time.deltaTime;
        if(timer <= 0){
            AddReward(-(Vector3.Distance(tForm.position, checkPoints[checkpointSpot].position)));
            checkpointSpot = 0;
            AddReward(-10);
            EndEpisode();
        }
        
        float afkTimer = 0;
        afkTimer += 1*Time.deltaTime;

        if(rBody.velocity.magnitude <= 0.5f && afkTimer > 2){
            afkTimer = 0;
            AddReward(-(Vector3.Distance(tForm.position, checkPoints[checkpointSpot].position)));
            checkpointSpot = 0;
            AddReward(-10);
            EndEpisode();
        }   
    }   

    public void DroveOff_AI(){
            AddReward(-(Vector3.Distance(tForm.position, checkPoints[checkpointSpot].position)));
            checkpointSpot = 0;
            AddReward(-10);
            EndEpisode();
    }
}
