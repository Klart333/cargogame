using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAccesories : MonoBehaviour
{
    [SerializeField]
    private Transform[] accesoriesPositions;

    [SerializeField]
    private Accesory[] accesories;

    private Accesory currentAccesory;

    public int Index { get; set; }

    public void AddAccesory(int accesoryIndex)
    {
        Accesory accesory = accesories[accesoryIndex];
        currentAccesory = Instantiate(accesory, accesoriesPositions[accesory.PositionIndex]);
    }

    public void RemoveAccesory()
    {
        if (currentAccesory != null)
        {
            Destroy(currentAccesory.gameObject);
            currentAccesory = null;
        }
    }
}
