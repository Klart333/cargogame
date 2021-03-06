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
    public Material SavedMaterial { get; set; } = null;
    public int SavedAccesoryIndex { get; set; } = -1;

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

        FindObjectOfType<UITransitionHandler>().InSceneTransition();

        TrackDone = false;

        if (toScene.buildIndex > 0 && startTrack)
        {
            var countText = Instantiate(countdownText, FindObjectOfType<Canvas>().transform);
            countText.StartCountdown(countdownTime);

            // Spawn car if you've gotten that far, if so: Good job :D Thanks :)
            Car car = FindObjectOfType<CarSpawner>().SpawnCar();

            if (SavedMaterial != null)
            {
                car.GetComponent<SelectionCar>().ApplyMaterial(SavedMaterial);

                if (toScene.buildIndex == 0)
                {
                    SavedMaterial = null;
                }
            }

            if (SavedAccesoryIndex != -1)
            {
                try
                {
                    int i = SavedAccesoryIndex > 0 ? SavedAccesoryIndex - 1 : SavedAccesoryIndex;
                    car.GetComponent<CarAccesories>().AddAccesory(i);
                }
                catch (Exception e)
                {
                    throw;
                }
                

                if (toScene.buildIndex == 0)
                {
                    SavedAccesoryIndex = -1;
                }
                
            }

            carRigidbody = car.GetComponent<Rigidbody>();
            carRigidbody.isKinematic = true;

            StartCoroutine(Countdown());
        }
        else
        {
            Cursor.visible = true;
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

    public void SaveTime(float time, out bool best)
    {
        int trackIndex = GetTrackIndex();
        best = Save.SaveTrackTime(trackIndex, time);
        if (best && trackIndex <= Save.AmountOfTracks)
        {
            GlobalHighscores.Instance.AddNewHighscore(time, trackIndex);
            FindObjectOfType<CloneHandler>().SaveInputs();
        }
    }

    public int GetTrackIndex()
    {
        return SceneManager.GetActiveScene().buildIndex - 1;
    }

    public void SetTimeScale(float timeScale)
    {
        gameSpeed = timeScale;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    public void ReloadScene()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadSavedTrack()
    {
        LoadLevel(SavedTrackIndex);
    }

    public void LoadLevel(int index)
    {
        StartCoroutine(LoadingLevel(index));
    }

    private IEnumerator LoadingLevel(int index)
    {
        UITransitionHandler transitionHandler = FindObjectOfType<UITransitionHandler>();
        if (transitionHandler != null)
        {
            transitionHandler.SceneTransition();
            yield return new WaitForSeconds(0.7f);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
