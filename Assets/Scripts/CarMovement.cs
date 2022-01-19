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
    private Transform[] wheelMeshes;

    private Vector3[] wheelAngularVelocity = new Vector3[4];
    private Inputs currentInputs;
    private Vector3 a = Vector3.zero;
    private Vector3 v = Vector3.zero;

    private bool[] wheelInAir = new bool[4];
    private float lastX = 0;
    public int currentGear = 0;
    private float differentialRatio = 3.42f;
    private float transmissionEfficiency = 0.7f;
    public float wheelRPM = 0;
    private float upsideDownTimer = 0;
    private bool carInAir = false;
    private bool upside = false;
    private float driveTorque = 0;

    // Weight Transfer
    private float c = 0;
    private float b = 0;
    private float h = 0;
    private float L = 0;
    private float W = 0;

    #region Properties
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
    #endregion

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

        L = Mathf.Abs(wheelPositions[0].localPosition.z - wheelPositions[3].localPosition.z);
        c = L / 2.0f;
        b = L / 2.0f;
        h = Mathf.Abs(CG.transform.position.y - groundPoint.transform.position.y);

        W = rigidbody.mass * gravitationalForce;

        wheelMeshes = new Transform[4];
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            wheelMeshes[i] = wheelPositions[i].GetChild(0);
        }
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
        CheckInAir();

        UpdateGears();

        UpdateVelocity();

        Sussypension();

        UpsideDownRoll();
    }

    private void UpdateGears()
    {
        if (currentInputs.Acceleration < 0)
        {
            currentGear = 6;
            return;
        }
        else if (currentGear == 6)
        {
            currentGear = 1;
        }

        if (V_Longitude > GearVelocityMax(currentGear))
        {
            currentGear += 1;
        }
        else if (V_Longitude < GearVelocityMax(currentGear - 1))
        {
            currentGear -= 1;
        }
    }

    private float GearVelocityMax(int currentGear)
    {
        switch (currentGear)
        {
            case -1:
                return -1;
            case 0:
                return 10;
            case 1:
                return 20;
            case 2:
                return 30;
            case 3:
                return 45;
            case 4:
                return 60;
            case 5:
                return 10000;
            case 6:
                return 10000; // Reverse
            default:
                return -1;
        }
    }

    private void CheckInAir()
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

        LongitudeHeading = transform.forward;
        LateralHeading = -transform.right;

        V_Longitude = Vector3.Dot(v, transform.forward);
        V_Lateral = Vector3.Dot(v, transform.right);

        if (true) // Draw Forces
        {
            Debug.DrawRay(transform.position, LongitudeHeading * V_Longitude, Color.red, 10);
            Debug.DrawRay(transform.position, LateralHeading * V_Lateral, Color.blue, 10);
        }

        #region Wheel rpm

        float wheelAngular = V_Longitude / wheelRadius;
        wheelRPM = wheelAngular * GearRatio * differentialRatio * (30 / Mathf.PI);
        if (wheelRPM < 1000 && currentInputs.Acceleration != 0)
        {
            wheelRPM = 1000;
        }

        float slipRatio = (wheelAngular * wheelRadius - V_Lateral) / V_Longitude;
        if (!float.IsNaN(slipRatio))
        {
            if (currentInputs.Brake == 1)
            {
                slipRatio = -1f;
            }

            float wheelMass = 500;
            float wheelInertia = wheelMass * Mathf.Pow(wheelRadius, 2);
            float angularAcceleration = driveTorque / wheelInertia;

            for (int i = 0; i < 4; i++)
            {
                wheelAngularVelocity[i] -= 0.2f * wheelAngularVelocity[i] * wheelAngularVelocity[i].magnitude * Time.deltaTime;

                if (i < 2)
                {
                    wheelAngularVelocity[i] += Vector3.forward * angularAcceleration * (1 + slipRatio) * Time.deltaTime;
                    wheelMeshes[i].Rotate(wheelAngularVelocity[i]);
                }
                else
                {
                    wheelAngularVelocity[i] += Vector3.forward * angularAcceleration * Time.deltaTime;
                    wheelMeshes[i].Rotate(wheelAngularVelocity[i]);
                }
            }
        }
        
        #endregion

        #region Turning
        for (int i = 0; i < 2; i++)
        {
            wheelPositions[2 + i].transform.localRotation = Quaternion.Euler(new Vector3(0, turningAngle * currentInputs.Horizontal, 0));
        }

        if (!carInAir)
        {
            // Low speed
            float R = L / Mathf.Sin(Mathf.Deg2Rad * turningAngle * currentInputs.Horizontal);
            Omega = (float)Mathf.Abs(V_Longitude) / (float)R; // Rad/s
            if (!torqueTurning)
            {
                float extraSpeed = (4 / (Mathf.Pow(Omega, 2) + 1));
                extraSpeed = Mathf.Clamp(extraSpeed, 1, 10);
                rigidbody.angularVelocity += Vector3.up * Omega * extraSpeed * Time.deltaTime;
            }

            // High-speed
            FrontWheelDelta = AngleBetweenVectors(LongitudeHeading, wheelPositions[3].forward) * Mathf.Sign(currentInputs.Horizontal);

            AlphaFront = Mathf.Atan((V_Lateral + Omega * b) / Mathf.Abs(V_Longitude)) - FrontWheelDelta * Mathf.Sign(V_Longitude);
            AlphaRear = Mathf.Atan((V_Lateral - Omega * c) / Mathf.Abs(V_Longitude));

            if (Mathf.Abs(AlphaFront) < 0.05f)
            {
                AlphaFront = 0;
            }

            if (Mathf.Abs(AlphaFront) < 0.05f)
            {
                AlphaRear = 0;
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
            float totalTorque = rear_Torque + front_Torque;
            AngularAcceleration = totalTorque / inertia;

            if (!float.IsNaN(AngularAcceleration) && torqueTurning)
            {
                rigidbody.angularVelocity += Vector3.up * AngularAcceleration * Time.deltaTime;
            }
            else
            {
                //print("Is null");
            }
        }
        #endregion

        for (int i = 0; i < 2; i++)
        {
            int inAir = 1;
            if (wheelInAir[i])
            {
                inAir = 0;
            }

            // Engine Force
            float engineTorque = GetEngineTorque(wheelRPM) * currentInputs.Acceleration * engineForce * inAir;
            driveTorque = engineTorque * GearRatio * differentialRatio * transmissionEfficiency;
            Vector3 T_drive = transform.forward * driveTorque / wheelRadius;
            print(T_drive);

            #region Straight line physics
            Vector3 F_drag = -drag * v * v.magnitude;

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
            var a_forward = Vector3.Dot(a, transform.forward);
            WeightRear = (c / L) * W + (h / L) * rigidbody.mass * a_forward;
            WeightFront = (b / L) * W - (h / L) * rigidbody.mass * a_forward;


            float F_Max = wheelFriction * WeightRear * engineForce;

            if (F_drive.magnitude > F_Max)
            {
                print("Wheel can not spin that fast at current weight");

                F_drive *= F_Max / F_drive.magnitude;

                a = F_drive / rigidbody.mass;
            }
            #endregion

            v = v + a * Time.deltaTime + (F_lat * Time.deltaTime) / rigidbody.mass;
            //transform.position += v * Time.deltaTime;
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

        if (rigidbody.velocity.magnitude < 0.11f && currentInputs.Acceleration != 0)
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
