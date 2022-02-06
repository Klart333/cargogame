using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBeenRotating : MonoBehaviour
{
    [SerializeField]
    private Axis axis;

    [SerializeField]
    private float rotationSpeed = 1; 

    private void Update()
    {
        switch (axis)
        {
            case Axis.X:
                transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
                break;
            case Axis.Y:
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
                break;
            case Axis.Z:
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                break;
            default:
                break;
        }
    }
}

public enum Axis
{
    X,
    Y,
    Z
}