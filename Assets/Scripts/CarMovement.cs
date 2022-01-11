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

    private Inputs currentInputs;
    private Inputs emptyInputs;

    private float additionalAcceleration = 1;

    public float speed = 0;
    private Vector3 carpos = new Vector3(0,0,0);

    private void Start()
    {
        emptyInputs = new Inputs(0, 0, 0);
    }

    void Update()
    {
        print(transform.position + "  " + carpos);
        float v = Vector3.Distance(transform.position, carpos);
        print(v);
        speed = v / Time.deltaTime;
       
        SetInputs(Input.GetAxisRaw("Vertical"), Input.GetKeyDown(KeyCode.Space) ? 1 : 0, Input.GetAxisRaw("Horizontal"));

        UpdateVelocity();


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
        transform.position += transform.forward * Time.deltaTime * carSpeed * currentInputs.Acceleration * additionalAcceleration;
        transform.Rotate(Vector3.up * currentInputs.Horizontal);
    }

    private void LateUpdate()
    {
        if (currentInputs.Acceleration == 0)
        {
            additionalAcceleration = 1;
        }

        carpos = transform.position;


        currentInputs = emptyInputs;
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