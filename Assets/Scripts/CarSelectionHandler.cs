using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelectionHandler : MonoBehaviour
{
    [Header("Cars")]
    [SerializeField]
    private GameObject[] darkCars;

    [SerializeField]
    private GameObject[] normalCars;

    [Header("Movement")]
    [SerializeField]
    private Transform[] spawnPositions;

    private List<GameObject> spawnedCars = new List<GameObject>();

    private Vector3 moveDir = Vector3.zero;
    private Vector3 endPosition;
    private Vector3 startPosition;

    private float movementPerMovement = 0;
    private float totalDist = 0;
    private bool movable = true;

    private void Start()
    {
        endPosition = spawnPositions[0].position;
        startPosition = spawnPositions[spawnPositions.Length - 1].position;

        totalDist = Vector3.Distance(startPosition, endPosition);
        movementPerMovement = totalDist / ((float)normalCars.Length - 1);
        moveDir = (endPosition - startPosition).normalized;
        SpawnCars(new bool[] { true, false, true, true, false, true});
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            MoveCars();
        }
    }

    public void SpawnCars(bool[] unlocked)
    {
        for (int i = 0; i < unlocked.Length; i++)
        {
            GameObject car = unlocked[i] ? normalCars[i] : darkCars[i];

            var _car = Instantiate(car, spawnPositions[i]);
            spawnedCars.Add(_car);
        }
    }

    public void MoveCars()
    {
        if (!movable)
        {
            return;
        }

        movable = false;
        StartCoroutine(Movable());

        for (int i = 0; i < spawnedCars.Count; i++)
        {
            StartCoroutine(MoveCar(spawnedCars[i].transform, spawnedCars[i].transform.position + moveDir * movementPerMovement));
        }
    }

    private IEnumerator MoveCar(Transform car, Vector3 target)
    {
        float speed = 0.75f;
        float t = 0;

        Vector3 ogPos = car.transform.position;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            car.transform.position = Vector3.Slerp(ogPos, target, Mathf.SmoothStep(0.0f, 1.0f, t));

            yield return null;
        }

        car.transform.position = target;

        if (Vector3.Distance(car.transform.position, startPosition) > totalDist)
        {
            car.transform.position = startPosition;
        }
    }

    private IEnumerator Movable()
    {
        yield return new WaitForSeconds(1.5f);
        movable = true;
    }
}
