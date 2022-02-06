using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootOrb : MonoBehaviour
{
    [SerializeField]
    private GameObject shatteredOrb;

    private void OnCollisionEnter(Collision collision)
    {
        CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
        if (carMovement != null)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());

            var tree = Instantiate(shatteredOrb, transform.GetChild(2));

            Destroy(transform.GetChild(0).gameObject);
            Destroy(transform.GetChild(1).gameObject);
        }
    }
}
