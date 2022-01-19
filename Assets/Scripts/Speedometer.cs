using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private Image speedometerFill;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private float topSpeed = 60;

    private CarMovement car;

    private void Start()
    {
        car = FindObjectOfType<CarMovement>();
    }

    private void Update()
    {
        text.text = Mathf.RoundToInt(Mathf.Abs(car.V_Longitude * 2)).ToString();
        float percent = (Mathf.Abs(car.V_Longitude) + topSpeed * 0.1f) / (topSpeed + topSpeed * 0.2f);

        speedometerFill.fillAmount = percent;
    }
}
