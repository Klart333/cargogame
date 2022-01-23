using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Main")]
    [SerializeField]
    [Range(0f, 50f)]
    private float gameSpeed = 1;

    [Header("Game")]
    [SerializeField]
    private UICountdown countdownText;

    [SerializeField]
    private float countdownTime = 3.9f;

    private Rigidbody carRigidbody;

    private int pressed = 0;
    private float timer = 0;

    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene fromScene, Scene toScene)
    {
        if (toScene.buildIndex > 0)
        {
            var countText = Instantiate(countdownText, FindObjectOfType<Canvas>().transform);
            countText.StartCountdown(countdownTime);

            // Spawn car if you've gotten that far, if so: Good job :D

            carRigidbody = FindObjectOfType<CarMovement>().GetComponent<Rigidbody>();
            carRigidbody.isKinematic = true;

            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(countdownTime);
        carRigidbody.isKinematic = false;

        FindObjectOfType<LapHandler>().StartLap();
    }

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

    public void SetTimeScale(float timeScale)
    {
        gameSpeed = timeScale;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
