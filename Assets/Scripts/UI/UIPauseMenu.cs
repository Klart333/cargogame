using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene(0);
    }

    private void ToggleMenu()
    {
        shown = !shown;

        GameManager.Instance.SetTimeScale(shown ? 0 : 1);

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            var child = transform.parent.GetChild(i);
            if (child != this.transform)
            {
                child.gameObject.SetActive(!shown);
            }
        } 

        transform.GetChild(0).gameObject.SetActive(shown);
    }
}
