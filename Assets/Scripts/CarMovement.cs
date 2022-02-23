using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    public const float gravitationalForce = 9.80665f;
    [Header("Input")]
    [SerializeField]
    public bool UsePlayerInput = true; 

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
    private float turningPower = 5;

    [SerializeField]
    private AnimationCurve slipRatioCurve;

    [SerializeField]
    private AnimationCurve slipAngleCurve;

    [SerializeField]
    private bool torqueTurning = true;

    [SerializeField]
    private bool wheelsAvailable = true;

    [SerializeField]
    private Axis wheelSpinAxis = Axis.X;

    [SerializeField]
    private float driftCoefficient = 1;

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

    [Header("Debug")]
    [SerializeField]
    private AnimationCurve savedCurve;

    private new Rigidbody rigidbody;
    private NavMeshAgent aiAgent;
    private NavMeshObstacle aiObstacle;
    private Transform[] wheelMeshes;
    private WheelSkid[] skids;

    private Inputs currentInputs;
    private Vector3 a = Vector3.zero;
    private Vector3 v = Vector3.zero;

    private bool[] wheelInAir = new bool[4];
    private float lastX = 0;
    public int currentGear = 0;
    private float differentialRatio = 3.42f;
    private float transmissionEfficiency = 0.7f;
    private bool carInAir = false;
    private float driveTorque = 0;
    private float gearUpTime = 0.2f;
    private bool gearing = false;
    private float gearTimer = 0;
    private bool gearDownPossible = true;
    private float gearDownTimer = 0;
    private float inertia = 4000;

    // Weight Transfer
    private float lengthToRear = 0;
    private float lengthToFront = 0;
    private float height = 0;
    private float lengthRearFront = 0;
    private float gravityForce = 0;
    private float wheelAngular;

    #region Properties
    public Vector3 Velocity { get { return rigidbody.velocity; } }
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
    public float V_Lateral_Rear { get; private set; }
    public float V_Lateral_Front { get; private set; }
    public float F_Cornering { get; private set; }
    public Vector3 F_lat { get; private set; }
    public Vector3 LongitudeHeading { get; private set; }
    public Vector3 LateralHeading { get; private set; }
    public float Omega { get; private set; }
    public float EngineRPM { get { return wheelAngular * GearRatio * differentialRatio * (60 / (2 * Mathf.PI)); } }

    #endregion

    #region Misc
    private float GearRatio
    {
        get
        {
            switch (currentGear)
            {
                case 0:
                    return 2f;
                case 1:
                    return 1.40f;
                case 2:
                    return 1.0f;
                case 3:
                    return 0.74f;
                case 4:
                    return 0.5f;
                case 5: 
                    return 2.90f; // Reverse
                default:
                    return 2.66f;
            }
        }
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        aiAgent = GetComponent<NavMeshAgent>();
        aiObstacle = GetComponent<NavMeshObstacle>();

        lengthRearFront = Mathf.Abs(wheelPositions[0].localPosition.z - wheelPositions[3].localPosition.z);
        lengthToRear = lengthRearFront * 0.5f;
        lengthToFront = lengthRearFront * 0.5f;
        height = Mathf.Abs(CG.transform.position.y - groundPoint.transform.position.y);

        gravityForce = rigidbody.mass * gravitationalForce;

        wheelMeshes = new Transform[4];
        if (wheelsAvailable)
        {
            for (int i = 0; i < wheelMeshes.Length; i++)
            {
                wheelMeshes[i] = wheelPositions[i].GetChild(0);
            }
        }

        skids = new WheelSkid[wheelPositions.Length];
        for (int i = 0; i < wheelPositions.Length; i++)
        {
            skids[i] = wheelPositions[i].GetComponentInChildren<WheelSkid>();
        }
    }

    private void Update()
    {
        if (UsePlayerInput)
        {
            SetInputs(Input.GetAxisRaw("Vertical"), Input.GetKey(KeyCode.Space) ? 1 : 0, Input.GetAxisRaw("Horizontal"));
        }

        if (gearing)
        {
            gearTimer += Time.deltaTime;
            if (gearTimer >= gearUpTime)
            {
                gearTimer = 0;
                gearing = false;
            }
        }

        if (!gearDownPossible)
        {
            gearDownTimer += Time.deltaTime;
            if (gearDownTimer >= gearUpTime + 1f)
            {
                gearDownTimer = 0;
                gearDownPossible = true;
            }
        }
    }

    void FixedUpdate()
    {
        CheckInAir();

        UpdateGears();

        UpdateVelocity();

        Sussypension();
    }

    private void UpdateGears()
    {
        if (currentInputs.Acceleration < 0)
        {
            currentGear = 5;
            return;
        }
        else if (currentGear == 5)
        {
            currentGear = 1;
        }

        if (V_Longitude > GearVelocityMax(currentGear))
        {
            if (currentGear > 0)
            {
                gearDownPossible = false;
                gearing = true;
            }
            
            currentGear += 1;
        }
        else if (V_Longitude < GearVelocityMax(currentGear - 1) && gearDownPossible && currentGear > 0)
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
                return 15;
            case 1:
                return 30;
            case 2:
                return 45;
            case 3:
                return 60;
            case 4:
                return 1000;
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
    #endregion

    private void UpdateVelocity()
    {
        v = rigidbody.velocity;
        float v_y = v.y;
        v.y = 0;

        LongitudeHeading = transform.forward;
        LateralHeading = transform.right;

        V_Longitude = Vector3.Dot(v, transform.forward);
        V_Lateral = Vector3.Dot(v, transform.right);

        float V_Longitude_Rear = Vector3.Dot(v, wheelPositions[0].forward);

        V_Lateral_Rear = Vector3.Dot(v, -wheelPositions[0].right); 
        V_Lateral_Front = Vector3.Dot(v, -wheelPositions[2].right); 

        if (false) // Draw Forces
        {
            Debug.DrawRay(wheelPositions[0].position, wheelPositions[0].forward * V_Lateral_Rear, Color.yellow, 10);
            Debug.DrawRay(wheelPositions[2].position, -wheelPositions[2].right * V_Lateral_Front, Color.green, 10);

            Debug.DrawRay(transform.position, LongitudeHeading * V_Longitude, Color.red, 10);
            Debug.DrawRay(transform.position, LateralHeading * V_Lateral, Color.blue, 10);
        }

        #region Wheel rpm

        wheelAngular = V_Longitude / wheelRadius;

        // Not used
        //float rear_WheelAngular = V_Longitude_Rear / wheelRadius;
        //float slipRatio = (wheelAngular * wheelRadius - V_Longitude) / Mathf.Abs(V_Longitude);
        if (wheelsAvailable)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 dir = Vector3.right;
                switch (wheelSpinAxis)
                {
                    case Axis.X:
                        dir = Vector3.right;
                        break;
                    case Axis.Y:
                        dir = Vector3.up;
                        break;
                    case Axis.Z:
                        dir = Vector3.forward;
                        break;
                    default:
                        break;
                }

                wheelMeshes[i].Rotate(dir * Speed * 10 * Time.deltaTime);
            }
        }

        #endregion

        #region Turning
        if (wheelsAvailable)
        {
            for (int i = 0; i < 2; i++)
            {
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, turningAngle * currentInputs.Horizontal, 0));
                float turningSpeed = 0.1f;
                wheelPositions[2 + i].transform.localRotation = Quaternion.Slerp(wheelPositions[2 + i].transform.localRotation, targetRotation, turningSpeed);
            }
        }

        if (!carInAir)
        {
            float R = lengthRearFront / Mathf.Sin(Mathf.Deg2Rad * turningAngle * currentInputs.Horizontal);
            Omega = (float)V_Longitude / (float)R;
            //Omega = rigidbody.angularVelocity.y;

            // High-speed
            FrontWheelDelta = AngleBetweenVectors(LongitudeHeading, wheelPositions[3].forward)/* * Mathf.Sign(currentInputs.Horizontal)*/;

            AlphaFront = Mathf.Atan((V_Lateral_Front + Omega * lengthToFront) / Mathf.Abs(V_Longitude)) - FrontWheelDelta * Mathf.Sign(V_Longitude);
            AlphaRear = Mathf.Atan((V_Lateral_Rear - Omega * lengthToRear) / Mathf.Abs(V_Longitude));

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

            rear_Torque = -F_Lat_rear * lengthToRear * driftCoefficient;
            front_Torque = /*Mathf.Cos(FrontWheelDelta) **/ F_Lat_front * lengthToFront;
            float totalTorque = rear_Torque + front_Torque;
            AngularAcceleration = totalTorque / inertia;
            AngularAcceleration *= turningPower;

            if (!float.IsNaN(AngularAcceleration) && torqueTurning && Mathf.Abs(Speed) > 1.0f)
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
            int shouldDrive = ShouldDrive() * inAir;
            float engineRPM = wheelAngular * GearRatio * differentialRatio * (60 / (2 * Mathf.PI));
            if (engineRPM < 1000)
            {
                engineRPM = 1000;
            }
            float engineTorque = GetEngineTorque(engineRPM) * currentInputs.Acceleration * engineForce * shouldDrive;
            driveTorque = engineTorque * GearRatio * differentialRatio * transmissionEfficiency;
            Vector3 T_drive = transform.forward * driveTorque / wheelRadius;

            Vector3 F_drag = -drag * v * v.magnitude;

            Vector3 F_rr = -rollingResistance * v;

            Vector3 F_drive = T_drive + F_drag + F_rr;
            if (currentInputs.Brake != 0)
            {
                Vector3 F_braking = -LongitudeHeading * brakeForce * Mathf.Sign(V_Longitude);
                F_drive = F_braking + F_drag + F_rr;
            }

            a = F_drive / rigidbody.mass;

            #region Weight Transfer


            /*float F_Max = wheelFriction * WeightRear * engineForce;

            if (F_drive.magnitude > F_Max)
            {
                //print("Wheel can not spin that fast at current weight");

                F_drive *= F_Max / F_drive.magnitude;
                print(F_drive);

                a = F_drive / rigidbody.mass;
                print(a);
            }*/
            #endregion

            v = v + a * Time.deltaTime + (F_lat * Time.deltaTime) / rigidbody.mass;
            Debug.DrawRay(transform.position, F_lat / rigidbody.mass, Color.black, 10);
            //transform.position += v * Time.deltaTime;
            v.y = v_y;
            rigidbody.velocity = v;
        }

        float driftThreshold = 10;
        if (AngleBetweenVectors(LongitudeHeading.normalized, Velocity.normalized) * Mathf.Rad2Deg > driftThreshold)
        {
            for (int i = 0; i < skids.Length; i++)
            {
                skids[i].AddSkidMark();
            }
        }
    }

    private void LateUpdate()
    {
        var a_forward = Vector3.Dot(a, LongitudeHeading);
        WeightRear = (lengthToRear / lengthRearFront) * gravityForce + (height / lengthRearFront) * rigidbody.mass * a_forward;
        WeightFront = (lengthToFront / lengthRearFront) * gravityForce - (height / lengthRearFront) * rigidbody.mass * a_forward;
    }

    private int ShouldDrive()
    {
        return (gearing ? 0 : 1) * (GameManager.Instance.TrackDone ? 0 : 1);
    }

    private float GetEngineTorque(float rpm)
    {
        float torque = engineTorqueCurve.Evaluate(rpm);
        return torque;
    }

    private float GetSlipForce(float slipRatio)
    {
        return slipRatioCurve.Evaluate(slipRatio);
    }

    private float AngleBetweenVectors(Vector3 v1, Vector3 v2)
    {
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

    private void OnTriggerEnter(Collider other) {
         if(other.GetComponent<LapHandler>() == true){
            
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
