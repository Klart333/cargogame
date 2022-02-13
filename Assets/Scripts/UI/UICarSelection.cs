using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICarSelection : MonoBehaviour
{
    private CarSelectionHandler carSelectionHandler;

    private void Start()
    {
        carSelectionHandler = FindObjectOfType<CarSelectionHandler>();
    }

    public void ToggleUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void NextCar()
    {
        carSelectionHandler.MoveCars();
    }

    public void Play()
    {
        carSelectionHandler.StartGame();
    }
}
