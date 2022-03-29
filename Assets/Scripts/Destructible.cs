using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private GameObject fracturedCopy;

    [SerializeField]
    private bool meshInChild = false;

    [SerializeField]
    private bool shouldShrink = false;

    private bool destroyed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyed)
        {
            return;
        }

        destroyed = true;

        CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
        if (carMovement != null)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());

            var tree = Instantiate(fracturedCopy, transform);
            tree.transform.localPosition = Vector3.zero;

            if (shouldShrink)
            {
                tree.transform.rotation = Quaternion.identity * Quaternion.Euler(0, transform.eulerAngles.y, 0);
                tree.transform.localScale /= transform.localScale.x;
            }

            if (meshInChild)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            else
            {
                Destroy(GetComponent<MeshRenderer>());
                Destroy(GetComponent<MeshFilter>());
            }
        }
    }
}
