using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarSelectionHandler : MonoBehaviour
{
    public event Action<int> OnCarChanged = delegate { };

    [Header("Lists")]
    [SerializeField]
    private SelectionCar[] allCars;

    [SerializeField]
    private int[] carNaturalColors;

    [SerializeField]
    private Material[] colorMaterials;

    [Header("Movement")]
    [SerializeField]
    private Transform mainPosition;

    [SerializeField]
    private Transform outOfSight;

    [Header("Setup")]
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    private List<SelectionCar> spawnedCars = new List<SelectionCar>();

    private SelectionCar currentCar;
    private CarAccesories currentCarAccesory;

    private UICarSelection carSelectionUI;

    private Vector3 moveDir = Vector3.zero;

    private float movementPerMovement = 0;
    private float distToOutOfSight = 0;
    private float totalDist = 0;
    private int amountSpawned = 0;
    private bool movable = true;

    private void Start()
    {
        carSelectionUI = FindObjectOfType<UICarSelection>();

        distToOutOfSight = Vector3.Distance(mainPosition.position, outOfSight.position);
        movementPerMovement = distToOutOfSight;
        moveDir = (outOfSight.position - mainPosition.position).normalized;

        OnCarChanged += CarSelectionHandler_OnCarChanged;
    }

    private void CarSelectionHandler_OnCarChanged(int carIndex)
    {
        currentCarAccesory = currentCar.GetComponent<CarAccesories>();
    }

    public void ToggleToCarSelection()
    {
        vcam.Priority = 100;

        carSelectionUI.ToggleUI();
        StartCoroutine(FuckingWait());
    }

    private IEnumerator FuckingWait()
    {
        yield return null;
        var cars = Save.GetUnlockedCars();
        if (!cars[0])
        {
            cars[0] = true;
            Save.SetUnlockedCars(0);
        }

        for (int i = 0; i < cars.Length; i++)
        {
            var colors = Save.GetUnlockedColors(i);
            if (!colors[carNaturalColors[i]])
            {
                colors[carNaturalColors[i]] = true;
                Save.SetUnlockedColors(i, colors);
            }
        }

        SpawnCars(cars);
    }

    public void SpawnCars(bool[] unlocked)
    {
        amountSpawned = 0;
        for (int i = 0; i < unlocked.Length; i++)
        {
            if (unlocked[i])
            {
                Vector3 spawnPosition = mainPosition.position - Vector3.forward * distToOutOfSight * amountSpawned;
                var _car = Instantiate(allCars[i], spawnPosition, Quaternion.identity);
                spawnedCars.Add(_car);
                amountSpawned++;
            }
        }
        currentCar = spawnedCars[0];
        currentCarAccesory = currentCar.GetComponent<CarAccesories>();
        OnCarChanged(currentCar.CarIndex);

        totalDist = (amountSpawned - 2) * distToOutOfSight;
    }

    public void MoveCars()
    {
        if (!movable)
        {
            return;
        }

        if (amountSpawned == 1)
        {
            return;
        }

        movable = false;
        StartCoroutine(Movable());

        for (int i = 0; i < spawnedCars.Count; i++)
        {
            StartCoroutine(MoveCar(spawnedCars[i].transform, spawnedCars[i].transform.position + moveDir * movementPerMovement));
        }

        int newIndex = spawnedCars.IndexOf(currentCar) + 1;
        if (newIndex >= spawnedCars.Count)
        {
            currentCar = spawnedCars[0];
        }
        else
        {
            currentCar = spawnedCars[newIndex];
        }
        OnCarChanged(currentCar.CarIndex);
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

        if (Vector3.Distance(car.transform.position, mainPosition.position) > totalDist)
        {
            car.transform.position = mainPosition.position - moveDir * distToOutOfSight;
        }
    }

    private IEnumerator Movable()
    {
        yield return new WaitForSeconds(1.5f);
        movable = true;
    }

    public void SelectColor(int colorIndex)
    {
        currentCar.ApplyMaterial(colorMaterials[colorIndex]);
    }

    public void SelectAccesory(int accesoryIndex)
    {
        currentCarAccesory.AddAccesory(accesoryIndex);

    }
    public void RemoveAccesory()
    {
        currentCarAccesory.RemoveAccesory();
    }

    public void StartGame()
    {
        GameManager.Instance.SavedMaterial = currentCar.AppliedMat;
        GameManager.Instance.SavedCarIndex = currentCar.CarIndex;
        GameManager.Instance.SavedAccesoryIndex = currentCarAccesory.Index;
        GameManager.Instance.LoadSavedTrack();
    }
}
