using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public const float gravitationalForce = 9.80665f;

    [Header("Input")]
    [SerializeField]
    private bool UsePlayerInput = true; 

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

    [Header("Turning")]
    [SerializeField]
    private float turningAngle = 30f;

    [SerializeField]
    private float corneringStiffness = 1;

    [SerializeField]
    private AnimationCurve slipRatioCurve;

    [SerializeField]
    private AnimationCurve slipAngleCurve;

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
    private float weightRear = 0;
    private float weightFront = 0;
    private Vector3 a = Vector3.zero;

    // Weight Transfer
    private float c = 0;
    private float b = 0;
    private float h = 0;
    private float L = 0;
    private float W = 0;

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

        c = Mathf.Abs(wheelPositions[0].localPosition.z - CG.transform.localPosition.z);
        b = Mathf.Abs(wheelPositions[3].localPosition.z - CG.transform.localPosition.z);
        h = springLength + CG.transform.localPosition.y - wheelPositions[0].localPosition.y;
        L = Mathf.Abs(wheelPositions[0].localPosition.z - wheelPositions[3].localPosition.z);
        W = rigidbody.mass * gravitationalForce;
    }

    void FixedUpdate()
    {
        if (UsePlayerInput)
        {
            SetInputs(Input.GetAxisRaw("Vertical"), Input.GetKeyDown(KeyCode.Space) ? 1 : 0, Input.GetAxisRaw("Horizontal"));
        }

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
        float v_y = v.y;
        v.y = 0;

        float speed = v.magnitude; 
        Vector3 u = transform.forward;

        Vector3 LongitudeHeading = Vector3.Project(v, transform.forward).normalized;
        Vector3 LateralHeading = Vector3.Project(v, transform.right).normalized;

        float F_Longitude = v.z;
        float F_Lateral = v.x;
        
        if (true) // Draw Forces
        {
            //Debug.DrawRay(transform.position, LateralHeading, Color.blue, 10);
            //Debug.DrawRay(transform.position, LongitudeHeading, Color.red, 10);

            Debug.DrawRay(transform.position, LongitudeHeading * F_Longitude, Color.red, 10);
            Debug.DrawRay(transform.position, LateralHeading * F_Lateral, Color.blue, 10);
        }
        
        // RPM
        float wheelAngular = speed / wheelRadius;
        rpm = wheelAngular * GearRatio * differentialRatio * (30 / Mathf.PI);
        if (rpm < 1000 && currentInputs.Acceleration != 0)
        {
            rpm = 1000;
        }

        float slipRatio = (wheelAngular * wheelRadius - F_Lateral) / F_Longitude; // Not implemented

        // Turning
        for (int i = 0; i < 2; i++)
        {
            wheelPositions[2 + i].transform.localRotation = Quaternion.Euler(new Vector3(0, turningAngle * currentInputs.Horizontal, 0));
        }

        // Low speed
        float R = L / Mathf.Sin(Mathf.Deg2Rad * turningAngle * currentInputs.Horizontal);
        float omega = (float)speed / (float)R; // Rad/s

        //rigidbody.angularVelocity = Vector3.up * omega;

        // High-speed
        float beta = Mathf.Atan(F_Longitude / F_Lateral);

        float frontWheelDelta = AngleBetweenVectors(u, wheelPositions[3].forward);
        float alpha_front = Mathf.Atan((F_Lateral + omega * b) / Mathf.Abs(F_Longitude)) - (frontWheelDelta * Math.Sign(F_Longitude));
        float alpha_rear = Mathf.Atan((F_Lateral - omega * c) / Mathf.Abs(F_Longitude));

        float F_Lat_front = 0;
        float F_Lat_rear = 0;
        if (!float.IsNaN(alpha_front))
        {
            F_Lat_front = slipAngleCurve.Evaluate(alpha_front) / 5000.0f * weightFront;
            F_Lat_rear = slipAngleCurve.Evaluate(alpha_rear) / 5000.0f * weightRear;
        }

        float F_Cornering = F_Lat_rear + Mathf.Cos(frontWheelDelta) * F_Lat_front;
        print("Cornering force: " + F_Cornering + " alpha_front: " + alpha_front + " alpha_rear: " + alpha_rear);
        //print("Cornering force: " + F_Cornering + " alpha_front: " + alpha_front + " alpha_rear: " + alpha_rear);

        Vector3 F_lat = Vector3.zero;
        if (!float.IsNaN(F_Cornering))
        {
            F_lat = LateralHeading * F_Cornering;
            print(F_lat);
        }

        float rear_Torque = -F_Lat_rear * c;
        float front_Torque = Mathf.Cos(frontWheelDelta) * F_Lat_front * b;
        float inertia = rigidbody.mass * a.magnitude;
        float angularAcceleration = (rear_Torque + front_Torque) / inertia;

        if (!float.IsNaN(angularAcceleration))
        {
            //rigidbody.angularVelocity += Vector3.up * angularAcceleration * Time.deltaTime;
        }
        else
        {
            //print("Is null");
        }

        for (int i = 0; i < 2; i++)
        {
            // Engine Force
            float torque = GetEngineTorque(rpm) * currentInputs.Acceleration * engineForce;
            Vector3 T_drive = (u * torque * GearRatio * differentialRatio * transmissionEfficiency) / wheelRadius;

            // Straight line physics
            Vector3 F_drag = -drag * v * speed;

            Vector3 F_rr = -rollingResistance * v;

            Vector3 F_drive = T_drive + F_drag + F_rr;
            if (currentInputs.Brake > 0)
            {
                Vector3 F_braking = -LongitudeHeading * brakeForce;
                F_drive = F_braking + F_drag + F_rr;
            }

            a = F_drive / rigidbody.mass;

            // Weight Transfer
            weightRear = (b / L) * W + (h / L) * rigidbody.mass * a.magnitude;
            weightFront = (c / L) * W - (h / L) * rigidbody.mass * a.magnitude;
            float F_Max = wheelFriction * weightRear * engineForce;

            if (F_drive.magnitude > F_Max)
            {
                //print("Wheel can not spin that fast at current weight");

                F_drive *= F_Max / F_drive.magnitude;

                a = F_drive / rigidbody.mass;
            }

            v = v + a * Time.deltaTime + F_lat * Time.deltaTime;
            v.y = v_y;
            rigidbody.velocity = v;
        }
    }   

    private float GetEngineTorque(float rpm)
    {
        float torque = engineTorqueCurve.Evaluate(rpm);
        return torque;
    }

    private float AngleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        v1 = v1.normalized;
        v2 = v2.normalized;
        return Mathf.Acos((Vector3.Dot(v1, v2) / (Vector3.SqrMagnitude(v1) * Vector3.SqrMagnitude(v2))));
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
