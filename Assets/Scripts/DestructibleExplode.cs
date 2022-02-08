using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleExplode : MonoBehaviour
{
    [SerializeField]
    private float minForce = 5;

    [SerializeField]
    private float maxForce = 10;

    [SerializeField]
    private float radius = 3f;

    [SerializeField]
    private bool shouldDespawn = true;

    [SerializeField]
    private float shrinkSpeed = 1;

    private Vector3 ogScale = new Vector3(100, 100, 100);

    private float t = 0;

    private void Start()
    {
        ogScale = transform.GetChild(0).localScale;

        for (int i = 0; i < transform.childCount; i++)
        {
            float force = UnityEngine.Random.Range(minForce, maxForce);
            transform.GetChild(i).gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius);
        }
    }

    private void Update()
    {
        t += Time.deltaTime * shrinkSpeed;

        if (t > 1)
        {
            if (shouldDespawn)
            {
                Destroy(gameObject);
            }

            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localScale = ogScale * (1.0f - t);
        }

       
    }
}
