using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIColorSelection : MonoBehaviour
{
    [SerializeField]
    private int colorIndex = 0;

    private CarSelectionHandler carSelection;

    private void Start()
    {
        carSelection = FindObjectOfType<CarSelectionHandler>();
        carSelection.OnCarChanged += CarSelection_OnCarChanged;
    }

    private void CarSelection_OnCarChanged(int carIndex)
    {
        var colors = Save.GetUnlockedColors(carIndex);
        gameObject.SetActive(!colors[carIndex]);
    }

    public void SelectColor()
    {
        carSelection.SelectColor(colorIndex);
    }
}
