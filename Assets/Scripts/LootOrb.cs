using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootOrb : MonoBehaviour
{
    [SerializeField]
    private GameObject shatteredOrb;

    [SerializeField]
    private Vector3 attachedOffset = new Vector3(0, 2, 0.2f);

    private HingeJoint hinge;
    private new Rigidbody rigidbody;

    private bool attached = false;

    private void Start()
    {
        hinge = GetComponent<HingeJoint>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (attached)
        {
            CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                return;
            }

            Destroy(GetComponent<HingeJoint>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());

            var shattered = Instantiate(shatteredOrb, transform.GetChild(2));

            Destroy(transform.GetChild(0).gameObject);
            Destroy(transform.GetChild(1).gameObject);
        }
        else
        {
            CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                attached = true;

                rigidbody.isKinematic = false;
                transform.position = carMovement.transform.position + attachedOffset;
                hinge.connectedBody = carMovement.GetComponent<Rigidbody>();
            }
        }
        
    }
}
