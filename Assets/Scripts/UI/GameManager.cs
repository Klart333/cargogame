using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    [Range(0f, 50f)]
    private float gameSpeed = 1;

    private int pressed = 0;
    private float timer = 0;

    private void Update()
    {
        if (Time.timeScale != gameSpeed)
        {
            Time.timeScale = gameSpeed;
        }

        CheckReload();
    }

    private void CheckReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            pressed += 1;
            if (pressed >= 2)
            {
                ReloadScene();
            }
        }

        if (pressed > 0)
        {
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                timer = 0;
                pressed = 0;
            }
        }
    }

    public void SaveTime(float timer)
    {
        
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
