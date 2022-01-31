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

    [SerializeField]
    private Transform endPosition;

    private Vector3 startPosition;

    private List<GameObject> spawnedCars = new List<GameObject>();

    private Vector3 moveDir = Vector3.zero;

    private float movementPerMovement = 0;
    private float totalDist = 0;

    private void Start()
    {
        startPosition = spawnPositions[spawnPositions.Length - 1].position;

        totalDist = Vector3.Distance(startPosition, endPosition.position);
        movementPerMovement = totalDist / (float)normalCars.Length;
        moveDir = (startPosition - endPosition.position).normalized;
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
        for (int i = 0; i < spawnedCars.Count; i++)
        {
            spawnedCars[i].transform.position += moveDir * movementPerMovement;
            if (Vector3.Distance(spawnedCars[i].transform.position, startPosition) > totalDist)
            {
                spawnedCars[i].transform.position = startPosition;
            }
        }
    }
}
