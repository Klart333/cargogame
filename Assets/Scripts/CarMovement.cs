using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public const float gravitationalForce = 9.80665f; 

    [Header("Car Physics")]
    [SerializeField]
    private float engineForce = 5;

    [SerializeField]
    private float drag = 1;

    [Tooltip("30x drag")]
    [SerializeField]
    private float rollingResistance = 30;

    [SerializeField]
    private float brakeForce = 5;

    [SerializeField]
    private Transform CG;

    [Range(0.5f, 1.8f)]
    [SerializeField]
    private float wheelFriction = 1.2f;

    [SerializeField]
    private float wheelRadius = 0.5f;

    [SerializeField]
    private AnimationCurve engineTorqueCurve;

    [Header("Susspension")]
    [SerializeField]
    private float springConstant = 4;

    [SerializeField]
    private float springLength = 4;

    [SerializeField]
    private float damping = 10;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Transform[] wheelPositions;

    private new Rigidbody rigidbody;

    private Inputs currentInputs;

    private float lastX = 0;
    private int currentGear = 0;
    private float differentialRatio = 3.42f;
    private float transmissionEfficiency = 0.7f;
    private float rpm = 0;

    private float GearRatio
    {
        get
        {
            switch (currentGear)
            {
                case 0:
                    return 2.66f;
                case 1:
                    return 1.78f;
                case 2:
                    return 1.30f;
                case 3:
                    return 1.00f;
                case 4:
                    return 0.74f;
                case 5:
                    return 0.5f;
                case 6: 
                    return 2.90f; // Reverse
                default:
                    return 2.66f;
            }
        }
        
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        SetInputs(Input.GetAxisRaw("Vertical"), Input.GetKeyDown(KeyCode.Space) ? 1 : 0, Input.GetAxisRaw("Horizontal"));
        UpdateVelocity();

        Sussypension();
    }

    public void SetInputs(float acceleration, float brake, float horizontal)
    {
        currentInputs.Acceleration = acceleration;
        currentInputs.Brake = brake;
        currentInputs.Horizontal = horizontal;
    }

    private void UpdateVelocity()
    {
        Vector3 v = rigidbody.velocity;
        Vector3 u = transform.forward;

        Vector3 F_Long = new Vector3(v.x * u.x, v.y * u.y, v.z * u.z);

        // RPM
        float wheelAngular = rigidbody.velocity.magnitude / wheelRadius;
        rpm = wheelAngular * GearRatio * differentialRatio * (30 / Mathf.PI);
        if (rpm < 1000 && currentInputs.Acceleration != 0)
        {
            rpm = 1000;
        }

        float slipRatio = (wheelAngular * wheelRadius - F_Long.magnitude) / F_Long.magnitude;

        // Weight Transfer
        float c = Mathf.Abs(wheelPositions[0].position.z - CG.transform.position.z);
        float b = Mathf.Abs(wheelPositions[3].position.z - CG.transform.position.z);
        float h = springLength + CG.transform.position.y - wheelPositions[0].position.y;
        float L = Mathf.Abs(wheelPositions[0].position.z - wheelPositions[3].position.z);
        float W = rigidbody.mass * gravitationalForce;

        for (int i = 0; i < 2; i++)
        {
            // Engine Force
            float torque = GetEngineTorque(rpm) * currentInputs.Acceleration * engineForce;
            Vector3 T_drive = (u * torque * GearRatio * differentialRatio * transmissionEfficiency) / wheelRadius;

            // Straight line physics
            Vector3 F_drag = -drag * v * Mathf.Sqrt(v.sqrMagnitude);

            Vector3 F_rr = -rollingResistance * v;

            Vector3 F_drive = T_drive + F_drag + F_rr;
            if (currentInputs.Brake > 0)
            {
                Vector3 F_braking = (rigidbody.velocity).normalized * brakeForce;
                F_drive = F_braking + F_drag + F_rr;
            }

            Vector3 a = F_drive / rigidbody.mass;

            // Weight Transfer
            float weight_Rear = (b / L) * W + (h / L) * rigidbody.mass * a.magnitude;
            float F_Max = wheelFriction * weight_Rear;

            if (F_drive.magnitude > F_Max)
            {
                print("Wheel can not spin that fast at current weight");

                F_drive *= F_Max / F_drive.magnitude;

                a = F_drive / rigidbody.mass;
            }

            v = v + a * Time.deltaTime;
            rigidbody.velocity = v;
        }
    }   

    private float GetEngineTorque(float rpm)
    {
        float torque = engineTorqueCurve.Evaluate(rpm);
        return torque;
    }

    private void Sussypension()
    {
        // Hookes law F = -kx (F = force, k = spring constant, x = spring strecth/compression (distance))

        for (int i = 0; i < wheelPositions.Length; i++)
        {
            if (Physics.Raycast(wheelPositions[i].position, -transform.up, out RaycastHit hit, springLength, layerMask))
            {
                // Spring force
                float x = springLength - hit.distance;
                if (x > 0)
                {
                    float springForce = -springConstant * x;

                    // Damping
                    float compressionSpeed = (x - lastX) / Time.deltaTime;
                    float dampForce = damping * compressionSpeed;
                    float force = springForce - dampForce;

                    if (force < 0)
                    {
                        rigidbody.AddForceAtPosition(-hit.normal * force, wheelPositions[i].position, ForceMode.Force);

                        lastX = x;
                    }
                }
            }
        }
    }
}

public struct Inputs
{
    public float Acceleration;
    public float Brake;
    public float Horizontal;

    public Inputs(float acceleration, float brake, float horizontal)
    {
        Acceleration = acceleration;
        Brake = brake;
        Horizontal = horizontal;
    }
}