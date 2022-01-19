using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private GameObject fracturedCopy;

    private void OnCollisionEnter(Collision collision)
    {
        CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
        if (carMovement != null)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());

/*          Vector3 normal = collision.GetContact(0).normal;
            Vector3 force = -normal * Vector3.Dot(carMovement.Velocity, normal) * carMovement.GetComponent<Rigidbody>().mass;*/

            var tree = Instantiate(fracturedCopy, transform);

            Destroy(transform.GetChild(0).gameObject);
/*
            var rbs = tree.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                if (Mathf.Abs(rbs[i].transform.position.y - collision.gameObject.transform.position.y) < 0.2f)
                {
                    rbs[i].AddForce(force / 10, ForceMode.Impulse);
                }
            }*/
        }
    }
}
