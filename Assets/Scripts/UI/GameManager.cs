using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    [Range(0f, 50f)]
    private float gameSpeed = 1;

    private void Update()
    {
        if (Time.timeScale != gameSpeed)
        {
            Time.timeScale = gameSpeed;
        }
    }
}
