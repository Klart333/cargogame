using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayHandler : MonoBehaviour
{
    [Header("Main")]
    [SerializeField]
    private float displayFrequency = 10;

    [Header("Stats")]
    [SerializeField]
    private float angularMax = 40;

    [SerializeField]
    private UILineRenderer angularAccelerationLine;

    private CarMovement car;
    
    private float timer = 0;

    private void Start()
    {
        car = FindObjectOfType<CarMovement>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1.0f / displayFrequency)
        {
            timer = 0;
            DisplayStats();
        }
    }

    private void DisplayStats()
    {
        // Angular Acceleration
        float percentAngular = (float)car.AngularAcceleration / (float)angularMax;
        angularAccelerationLine.AddPoint(percentAngular);
    }
}
