using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAccesoryButton : MonoBehaviour
{
    [SerializeField]
    private bool remover = false;
    [SerializeField]
    private int index;

    private CarSelectionHandler carSelection;

    private void Start()
    {
        carSelection = FindObjectOfType<CarSelectionHandler>();
        carSelection.OnCarChanged += CarSelection_OnCarChanged;
    }

    private void CarSelection_OnCarChanged(int carIndex)
    {
        if (remover)
        {
            return;
        }

        bool[] accesories = Save.GetUnlockedAccesories(carIndex);
        gameObject.SetActive(accesories[index]);
    }

    public void AddAccesorie()
    {
        carSelection.RemoveAccesory();
        carSelection.SelectAccesory(index);
    }

    public void RemoveAccesorie()
    {
        carSelection.RemoveAccesory();
    }
}
