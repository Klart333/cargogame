using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : MonoBehaviour
{
    private bool shown = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void Resume()
    {
        ToggleMenu();
    }

    public void QuitToMenu()
    {
        GameManager.Instance.SetTimeScale(1);
        GameManager.Instance.LoadLevel(0);
    }

    private void ToggleMenu()
    {
        shown = !shown;

        GameManager.Instance.SetTimeScale(shown ? 0 : 1);

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            var child = transform.parent.GetChild(i);
            if (child != this.transform && child.GetComponentInChildren<UITransitionHandler>() == null)
            {
                child.gameObject.SetActive(!shown);
            }
        } 

        transform.GetChild(0).gameObject.SetActive(shown);
    }
}
