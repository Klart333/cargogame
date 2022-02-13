using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public int SavedTrackIndex = 0;

    [Header("Main")]
    [SerializeField]
    [Range(0f, 50f)]
    private float gameSpeed = 1;

    [Header("Game")]
    [SerializeField]
    private UICountdown countdownText;

    [Header("Debug")]
    [SerializeField]
    private bool startTrack = false;

    private Rigidbody carRigidbody;

    private float countdownTime = 3f;
    private int pressed = 0;
    private float timer = 0;

    public bool TrackDone { get; set; }
    public int SavedCarIndex { get; set; }
    public Material SavedMaterial { get; set; }

    protected override void Awake()
    {
        base.Awake();

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene fromScene, Scene toScene)
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        TrackDone = false;

        if (toScene.buildIndex > 0)
        {
            var countText = Instantiate(countdownText, FindObjectOfType<Canvas>().transform);
            countText.StartCountdown(countdownTime);

            // Spawn car if you've gotten that far, if so: Good job :D Thanks :)
            Car car = FindObjectOfType<CarSpawner>().SpawnCar();

            if (SavedMaterial != null)
            {
                car.GetComponent<SelectionCar>().ApplyMaterial(SavedMaterial);
            }

            carRigidbody = car.GetComponent<Rigidbody>();
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

    public void SaveTime(float time)
    {
        Save.SaveTrackTime(GetTrackIndex(), time);
    }

    public int GetTrackIndex()
    {
        return SceneManager.GetActiveScene().buildIndex - 1;
    }

    public void SetTimeScale(float timeScale)
    {
        gameSpeed = timeScale;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    public void LoadSavedTrack()
    {
        SceneManager.LoadScene(SavedTrackIndex);
    }
}
