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

    [SerializeField]
    private GameObject groundPoint;

    [Header("Turning")]
    [SerializeField]
    private float turningAngle = 30f;

    [SerializeField]
    private float corneringStiffness = 1;

    [SerializeField]
    private AnimationCurve slipRatioCurve;

    [SerializeField]
    private AnimationCurve slipAngleCurve;

    [SerializeField]
    private float inertia = 100;

    [SerializeField]
    private bool torqueTurning = true;

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
    private Vector3 a = Vector3.zero;
    private Vector3 v = Vector3.zero;

    private bool[] wheelInAir = new bool[4];
    private float lastX = 0;
    private int currentGear = 0;
    private float differentialRatio = 3.42f;
    private float transmissionEfficiency = 0.7f;
    private float rpm = 0;
    private float direction = 0;
    private float upsideDownTimer = 0;
    private bool carInAir = false;
    private bool upside = false;

    // Weight Transfer
    private float c = 0;
    private float b = 0;
    private float h = 0;
    private float L = 0;
    private float W = 0;
 
    public float Speed { get { return new Vector3(v.x, 0, v.z).magnitude; } }
    public float AlphaRear { get; private set; } = 0;
    public float F_Lat_front { get; private set; }
    public float F_Lat_rear { get; private set; }
    public float FrontWheelDelta { get; private set; }
    public float AlphaFront { get; private set; } = 0;
    public float rear_Torque { get; private set; } = 0;
    public float front_Torque { get; private set; } = 0;
    public float TotalTorque { get { return front_Torque + rear_Torque; } }
    public float AngularAcceleration { get; private set; }
    public float WeightRear { get; private set; } = 0;
    public float WeightFront { get; private set; } = 0;
    public float V_Longitude { get; private set; } 
    public float V_Lateral { get; private set; }
    public float F_Cornering { get; private set; }
    public Vector3 F_lat { get; private set; }
    public Vector3 LongitudeHeading { get; private set; }
    public Vector3 LateralHeading { get; private set; }
    public float Omega { get; private set; }

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
        h = Mathf.Abs(CG.transform.position.y - groundPoint.transform.position.y);
        L = Mathf.Abs(wheelPositions[0].localPosition.z - wheelPositions[3].localPosition.z);

        W = rigidbody.mass * gravitationalForce;
    }

    private void Update()
    {
        if (UsePlayerInput)
        {
            SetInputs(Input.GetAxisRaw("Vertical"), Input.GetKeyDown(KeyCode.Space) ? 1 : 0, Input.GetAxisRaw("Horizontal"));
        }
    }

    void FixedUpdate()
    {
        int num = 0;
        for (int i = 0; i < wheelInAir.Length; i++)
        {
            if (wheelInAir[i])
            {
                num++;
            }
        }

        if (num >= wheelInAir.Length)
        {
            carInAir = true;
        }
        else
        {
            carInAir = false;
        }

        UpdateVelocity();

        Sussypension();

        UpsideDownRoll();
    }

    public void SetInputs(float acceleration, float brake, float horizontal)
    {
        currentInputs.Acceleration = acceleration;
        currentInputs.Brake = brake;
        currentInputs.Horizontal = horizontal;
    }

    private void UpdateVelocity()
    {
        v = rigidbody.velocity;
        float v_y = v.y;
        v.y = 0;
        float speed = v.magnitude; 

        LongitudeHeading = transform.forward * direction;
        LateralHeading = -Vector3.Project(v, transform.right).normalized;

        V_Longitude = Vector3.Project(v, transform.forward).magnitude;
        V_Lateral = Vector3.Project(v, transform.right).magnitude;
        if (V_Lateral < 0.1f)
        {
            V_Lateral = 0;
        }


        if (true) // Draw Forces
        {
            //Debug.DrawRay(transform.position, LateralHeading, Color.blue, 10);
            //Debug.DrawRay(transform.position, LongitudeHeading, Color.red, 10);

            Debug.DrawRay(transform.position, LongitudeHeading * V_Longitude, Color.red, 10);
            Debug.DrawRay(transform.position, LateralHeading * V_Lateral, Color.blue, 10);
        }
        
        // RPM
        float wheelAngular = speed / wheelRadius;
        rpm = wheelAngular * GearRatio * differentialRatio * (30 / Mathf.PI);
        if (rpm < 1000 && currentInputs.Acceleration != 0)
        {
            rpm = 1000;
        }

        float slipRatio = (wheelAngular * wheelRadius - V_Lateral) / V_Longitude; // Not implemented

        // Turning
        for (int i = 0; i < 2; i++)
        {
            wheelPositions[2 + i].transform.localRotation = Quaternion.Euler(new Vector3(0, turningAngle * currentInputs.Horizontal, 0));
        }

        if (!carInAir)
        {
            // Low speed
            float R = L / Mathf.Sin(Mathf.Deg2Rad * turningAngle * currentInputs.Horizontal);
            Omega = (float)speed / (float)R; // Rad/s
            float extraSpeed = (4 / (Mathf.Pow(Omega, 2) + 1));
            extraSpeed = Mathf.Clamp(extraSpeed, 1, 10);
            rigidbody.angularVelocity += Vector3.up * Omega * extraSpeed * Time.deltaTime;

            // High-speed
            float beta = Mathf.Atan(V_Longitude / V_Lateral);

            FrontWheelDelta = AngleBetweenVectors(LongitudeHeading, wheelPositions[3].forward) * Mathf.Sign(currentInputs.Horizontal);

            if (V_Longitude > 0.5f)
            {
                AlphaFront = Mathf.Atan((V_Lateral + rigidbody.angularVelocity.magnitude * b) / Mathf.Abs(V_Longitude)) - FrontWheelDelta;
                AlphaRear = Mathf.Atan((V_Lateral - rigidbody.angularVelocity.magnitude * c) / Mathf.Abs(V_Longitude));
            }
            else
            {
                AlphaRear = 0;
                AlphaFront = 0;
            }
            

            F_Lat_front = 0;
            F_Lat_rear = 0;
            if (!float.IsNaN(AlphaFront))
            {
                F_Lat_front = slipAngleCurve.Evaluate(AlphaFront * Mathf.Rad2Deg) / 5000.0f * WeightFront;
            }

            if (!float.IsNaN(AlphaRear))
            {
                F_Lat_rear = slipAngleCurve.Evaluate(AlphaRear * Mathf.Rad2Deg) / 5000.0f * WeightRear;
            }

            F_Cornering = F_Lat_rear + Mathf.Cos(FrontWheelDelta) * F_Lat_front;

            F_lat = Vector3.zero;
            if (!float.IsNaN(F_Cornering))
            {
                F_lat = LateralHeading * F_Cornering * corneringStiffness;
            }

            rear_Torque = -F_Lat_rear * c;
            front_Torque = Mathf.Cos(FrontWheelDelta) * F_Lat_front * b;
            AngularAcceleration = (rear_Torque + front_Torque) / inertia;

            if (!float.IsNaN(AngularAcceleration) && torqueTurning)
            {
                rigidbody.angularVelocity += Vector3.up * AngularAcceleration * Time.deltaTime;
            }
            else
            {
                //print("Is null");
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int inAir = 1;
            if (wheelInAir[i])
            {
                inAir = 0;
            }

            // Engine Force
            float torque = GetEngineTorque(rpm) * currentInputs.Acceleration * engineForce * inAir;
            Vector3 T_drive = (transform.forward * torque * GearRatio * differentialRatio * transmissionEfficiency) / wheelRadius;

            #region Straight line physics
            Vector3 F_drag = -drag * v * speed;

            Vector3 F_rr = -rollingResistance * v;

            Vector3 F_drive = T_drive + F_drag + F_rr;
            if (currentInputs.Brake > 0)
            {
                Vector3 F_braking = -LongitudeHeading * brakeForce;
                F_drive = F_braking + F_drag + F_rr;
            }
            #endregion

            a = F_drive / rigidbody.mass;

            #region Weight Transfer
            WeightRear = (b / L) * W + (h / L) * rigidbody.mass * a.magnitude;
            WeightFront = (c / L) * W - (h / L) * rigidbody.mass * a.magnitude;

            float F_Max = wheelFriction * WeightRear * engineForce;

            if (F_drive.magnitude > F_Max)
            {
                //print("Wheel can not spin that fast at current weight");

                F_drive *= F_Max / F_drive.magnitude;

                a = F_drive / rigidbody.mass;
            }
            #endregion

            v = v + a * Time.deltaTime + (F_lat * Time.deltaTime) / rigidbody.mass;
            //transform.position += v * Time.deltaTime;
            v.y = v_y;
            rigidbody.velocity = v;

            direction = 1; // Figure out the projecting shit
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
        float v3 = Mathf.Acos((Vector3.Dot(v1, v2) / (Vector3.SqrMagnitude(v1) * Vector3.SqrMagnitude(v2))));
        if (float.IsNaN(v3))
        {
            return 0;
        }
        else
        {
            return v3;
        }
    }

    private void Sussypension()
    {
        // Hookes law F = -kx (F = force, k = spring constant, x = spring strecth/compression (distance))

        for (int i = 0; i < wheelPositions.Length; i++)
        {
            if (Physics.Raycast(wheelPositions[i].position, -transform.up, out RaycastHit hit, springLength, layerMask))
            {
                wheelInAir[i] = false;

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
            else
            {
                wheelInAir[i] = true;
            }
        }
    }

    private void UpsideDownRoll()
    {
        if (upside && upsideDownTimer > -0.8f)
        {
            transform.Rotate(transform.forward * currentInputs.Horizontal * 5);
            upsideDownTimer -= Time.deltaTime;
            return;
        }

        if (rigidbody.velocity.magnitude < 0.08 && currentInputs.Acceleration != 0)
        {
            upsideDownTimer += Time.deltaTime;
            if (upsideDownTimer > 0.2f)
            {
                upside = true;
                transform.Rotate(transform.forward * currentInputs.Horizontal * 5);
            }
        }
        else
        {
            upside = false;
            upsideDownTimer = 0;
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
