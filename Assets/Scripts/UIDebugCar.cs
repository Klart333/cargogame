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
            text.text = string.Format("Johnmobil speed: {0}, (<color=red>{6}</color>, <color=blue>{7}</color>) " +
                "\n Heading: <color=red>{9}</color>, <color=blue>{10}</color>" +
                "\n F_lat_rear: <color=yellow>{11}</color> " +
                "\n F_lat_front: <color=purple>{12}</color> " +
                "\n F_Lat: {8} " +
                "\n Weight Rear: {5} " +
                "\n Total Torque: {3} " +
                "\n Angular Acceleration: {4} " +
                "\n Front wheel alpha: {1} " +
                "\n Back wheel alpha: {2}" +
                "\n Front wheel delta: {13} ",
                car.Speed, car.AlphaFront, car.AlphaRear, car.TotalTorque, 
                car.AngularAcceleration, car.WeightRear, car.F_Longitude, car.F_Lateral,
                car.F_lat, car.LongitudeHeading, car.LateralHeading, car.F_Lat_rear, 
                car.F_Lat_front, car.FrontWheelDelta);
        }
    }
}
