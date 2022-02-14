using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    public event Action OnGot = delegate { };

    [HideInInspector]
    public Quaternion CarRotation;

    [HideInInspector]
    public Vector3 CarVelocity;

    [HideInInspector]
    public Vector3 CarAngularVelocity;

    [HideInInspector]
    public Vector3 CarPosition;

    [SerializeField]
    public int Index;

    [SerializeField]
    private GameObject fracturedPrefab;

    private LapHandler lapHandler;

    private bool gettable = true;
    private bool fractured = false;

    private void Start()
    {
        lapHandler = FindObjectOfType<LapHandler>();
    }

    public void Fracture()
    {
        if (!fractured)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            Instantiate(fracturedPrefab, transform);
            fractured = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fractured)
        {
            return;
        }

        if (gettable && other.GetComponent<CarMovement>() != null && other.tag == "Player")
        {
            CarPosition = other.gameObject.transform.position;
            CarRotation = other.gameObject.transform.rotation;
            var rb = other.gameObject.GetComponent<Rigidbody>();
            CarVelocity = rb.velocity;
            CarAngularVelocity = rb.angularVelocity;

            gettable = false;
            lapHandler.GetCheckPoint(Index);
            OnGot();
        }
    }
}
