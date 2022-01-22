using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CarMovement>() != null)
        {
            transform.parent.parent.GetComponent<Checkpoint>().Fracture();
        }
    }
}
