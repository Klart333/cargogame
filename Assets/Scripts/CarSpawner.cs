using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private Car[] AllCars;

    [SerializeField]
    private Transform spawnPosition;

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

        print("Spawned Car");
        var car = Instantiate(theCar, spawnPosition.position, spawnPosition.rotation);
        FindObjectOfType<CameraController>().SetFollow(car.transform);

        return car;
    }
}
