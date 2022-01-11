using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Car")]
    [SerializeField]
    private float carSpeed = 5;

    [SerializeField]
    private float turnSpeed = 5;

    [SerializeField]
    private float accelerationSpeed = 2;

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

    private Vector3 carPos = new Vector3();
    private Inputs currentInputs;
    private Inputs emptyInputs;

    private float additionalAcceleration = 1;
    private float lastX = 0;
    private float speed = 0;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        emptyInputs = new Inputs(0, 0, 0);
    }

    void FixedUpdate()
    {
        carPos = transform.position;

        SetInputs(Input.GetAxisRaw("Vertical"), Input.GetKeyDown(KeyCode.Space) ? 1 : 0, Input.GetAxisRaw("Horizontal"));
        UpdateVelocity();

        Sussypension();

        // johan
        float distance = Vector3.Distance(transform.position, carPos);
        speed = distance / Time.deltaTime;
    }

    public void SetInputs(float acceleration, float brake, float horizontal)
    {
        currentInputs.Acceleration = acceleration;
        currentInputs.Brake = brake;
        currentInputs.Horizontal = horizontal;
    }

    private void UpdateVelocity()
    {
        additionalAcceleration += Time.deltaTime * accelerationSpeed;

        rigidbody.velocity += transform.forward * Time.deltaTime * carSpeed * currentInputs.Acceleration * additionalAcceleration;

        transform.Rotate(Vector3.up * currentInputs.Horizontal * turnSpeed);
    }

    private void LateUpdate()
    {
        if (currentInputs.Acceleration == 0)
        {
            additionalAcceleration = 1;
        }

        currentInputs = emptyInputs;
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