using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDebugCar : MonoBehaviour
{
    [SerializeField]
    private float updateFrequency = 0.1f;

    private TextMeshProUGUI text;
    private CarMovement car;

    private float timer = 1;
 
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
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
                "\n F_lat_rear: <color=yellow>{11}</color>" +
                "\n F_lat_front: <color=purple>{12}</color>" +
                "\n F_Lat: {8} " +
                "\n Weight Rear: <color=yellow>{5}</color> " +
                "\n Weight Front: <color=purple>{18}</color> " +
                "\n Total Torque: {3} (<color=yellow>{16}</color> <color=purple>{17}</color>)" +
                "\n Angular Acceleration: {4} " +
                "\n Front wheel alpha: {1} " +
                "\n Back wheel alpha: {2}" +
                "\n Front wheel delta: {13} " + 
                "\n Cornering: {14} " +
                "\n Omega: {15} ",
                car.Speed, car.F_Lat_rear, car.F_Lat_front, car.TotalTorque, 
                car.AngularAcceleration, car.WeightRear, car.V_Longitude, car.V_Lateral,
                car.F_lat, car.LongitudeHeading, car.LateralHeading, car.F_Lat_rear, 
                car.F_Lat_front, car.FrontWheelDelta, car.F_Cornering, car.Omega,
                car.rear_Torque, car.front_Torque, car.WeightFront);
        }
    }
}
