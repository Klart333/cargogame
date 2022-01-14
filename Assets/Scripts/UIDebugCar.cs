using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugCar : MonoBehaviour
{
    [SerializeField]
    private float updateFrequency = 0.1f;

    private Text text;
    private CarMovement car;

    private float timer = 1;
 
    void Start()
    {
        text = GetComponent<Text>();
        car = FindObjectOfType<CarMovement>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateFrequency)
        {
            timer = 0;
            text.text = string.Format("Johnmobil speed: {0}, (<color=red>{6}</color>, <color=blue>{7}</color>) \n Weight Rear: {5} \n Total Torque: {3} \n Angular Acceleration: {4} \n Front wheel alpha: {1} \n Back wheel alpha: {2}", car.Speed, car.AlphaFront, car.AlphaRear, car.TotalTorque, car.AngularAcceleration, car.WeightRear, car.F_Longitude, car.F_Lateral);
        }
    }
}
