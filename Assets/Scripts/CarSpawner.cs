using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private Car[] AllCars;

    [SerializeField]
    public Transform SpawnPosition;

    public Car SpawnCar()
    {
        Car theCar = null;
        for (int i = 0; i < AllCars.Length; i++)
        {
            if (AllCars[i].CarIndex == GameManager.Instance.SavedCarIndex)
            {
                theCar = AllCars[i];
            }
        }

        var car = Instantiate(theCar, SpawnPosition.position, SpawnPosition.rotation);
        FindObjectOfType<CameraController>().SetFollow(car.transform);

        return car;
    }
}
