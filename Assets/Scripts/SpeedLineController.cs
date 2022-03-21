using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLineController : MonoBehaviour
{
    [SerializeField]
    private Vector2 minMaxCarSpeed = new Vector2(30, 80);

    [SerializeField]
    private Vector2 minMaxParticleSpeed = new Vector2(5, 15);

    [SerializeField]
    private Vector2 minMaxEmission = new Vector2(50, 150);

    [SerializeField]
    private Vector2 minMaxSize = new Vector2(0.75f, 1f);

    [SerializeField]
    private Color minColor;

    [SerializeField]
    private Color maxColor;

    private ParticleSystem psys;
    private CarMovement car;

    private ParticleSystem.MainModule main;
    private ParticleSystem.EmissionModule emission;

    private new bool enabled = false;

    private void Start()
    {
        car = FindObjectOfType<CarMovement>();
        psys = GetComponent<ParticleSystem>();

        main = psys.main;
        emission = psys.emission;
    }

    void Update()
    {
        if (car.Speed >= minMaxCarSpeed.x)
        {
            if (!enabled)
            {
                enabled = true;
                psys.Play();
            }

            float percent = (float)(car.Speed - minMaxCarSpeed.x) / (float)(minMaxCarSpeed.y - minMaxCarSpeed.x);

            main.startSpeed = Lerp(minMaxParticleSpeed.x, minMaxParticleSpeed.y, percent);
            main.startSize = Lerp(minMaxSize.x, minMaxSize.y, percent);
            main.startColor = Color.Lerp(minColor, maxColor, percent);
            emission.rateOverTime = Lerp(minMaxEmission.x, minMaxEmission.y, percent);
        }
        else if (enabled)
        {
            enabled = false;
            psys.Stop();
        }
    }

    private float Lerp(float p1, float p2, float t)
    {
        return (p1 * (1 - t)) + (p2 * t);
    }
}
