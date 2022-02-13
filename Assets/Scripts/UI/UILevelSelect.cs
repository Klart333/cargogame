using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class UILevelSelect : MonoBehaviour
{
    private const int AILevels = 3;

    [SerializeField]
    private GameObject[] toHide;

    [SerializeField]
    private CinemachineVirtualCamera[] cameras;

    [SerializeField]
    private GameObject[] tracks;

    [Header("Regular")]
    [SerializeField]
    private GameObject regularTrack;

    [SerializeField]
    private GameObject car;

    [Header("Titles")]
    [SerializeField]
    private string[] titles;

    [SerializeField]
    private TextMeshProUGUI titleText;

    [Header("Text")]
    [SerializeField]
    private GameObject selectLevel;

    [SerializeField]
    private GameObject gamemodes;

    [Header("PB")]
    [SerializeField]
    private TextMeshProUGUI pbText;

    [SerializeField]
    private Image[] stars;

    private UITransitionHandler transitionHandler;

    private int currentIndex = 0;

    private void Start()
    {
        transitionHandler = FindObjectOfType<UITransitionHandler>();
    }

    public void ToggleUI(bool show)
    {
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].SetActive(show);
        }

        transform.GetChild(0).gameObject.SetActive(!show);
    }

    public void DisplayNext()
    {
        DisplayLevel(++currentIndex);
    }

    public void GoIntoLevelSelect()
    {
        StartCoroutine(GoIntoLevelSelect_Routine());
    }

    private IEnumerator GoIntoLevelSelect_Routine()
    {
        DisplayLevel(0);

        yield return new WaitForSeconds(0.5f);

        ToggleUI(false);
    }

    public void DisplayLevel(int index)
    {
        StartCoroutine(DisplayLevel_Transition(index));
    }

    private IEnumerator DisplayLevel_Transition(int index)
    {
        transitionHandler.Transition();

        yield return new WaitForSeconds(0.5f);

        currentIndex = index;
        if (currentIndex >= tracks.Length)
        {
            currentIndex = 0;
        }

        titleText.text = titles[currentIndex];

        //regularTrack.SetActive(false);
        //car.SetActive(false);

        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == currentIndex)
            {
                cameras[i].Priority = 100;
            }
            else
            {
                cameras[i].Priority = -1;
            }
        }

        for (int i = 0; i < tracks.Length; i++)
        {
            if (i == currentIndex)
            {
                tracks[i].SetActive(true);
            }
            else
            {
                tracks[i].SetActive(true);
            }
        }
    }

    public void GoBack()
    {
        StartCoroutine(GoBack_Transition());
    }

    private IEnumerator GoBack_Transition()
    {
        transitionHandler.Transition();

        yield return new WaitForSeconds(0.5f);

        ToggleUI(true);

        car.SetActive(true);
        regularTrack.SetActive(true);

        selectLevel.SetActive(true);
        gamemodes.SetActive(false);

        for (int i = 0; i < tracks.Length; i++)
        {
            //tracks[i].SetActive(false);
        }

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = -1;
        }
    }

    public void SelectThisLevel()
    {
        selectLevel.SetActive(false);
        gamemodes.SetActive(true);

        DisplayPB();
    }

    private void DisplayPB()
    {
        float time = Save.GetTrackTime(currentIndex);
        if (time == -1)
        {
            pbText.text = string.Format("PB - None");

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].color = Color.white;
            }
        }
        else
        {
            float _time = time;
            int minuteTens = Mathf.FloorToInt(_time / 600.0f);
            _time -= minuteTens * 600;
            int minutes = Mathf.FloorToInt(_time / 60.0f);
            _time -= minutes * 60;
            int tens = Mathf.FloorToInt(_time / 10.0f);
            _time -= tens * 10;
            int seconds = Mathf.FloorToInt(_time);
            _time -= seconds;
            int tenths = Mathf.FloorToInt(_time * 10);
            _time -= tenths / 10.0f;
            int hundreths = Mathf.FloorToInt(_time * 100);

            pbText.text = string.Format("PB - {0}{1}:{2}{3}:{4}{5}", minuteTens, minutes, tens, seconds, tenths, hundreths);

            for (int i = 0; i < Save.AllStarTimes[currentIndex].Times.Length; i++)
            {
                if (time < Save.AllStarTimes[currentIndex].Times[i])
                {
                    stars[i].color = Color.yellow;
                }
                else
                {
                    stars[i].color = Color.white;
                }
            }
        }
    }

    public void SelectLevelTimeAttack()
    {
        GameManager.Instance.SavedTrackIndex = currentIndex + 1;
        StartCoroutine(TransitionToCar());
    }

    public void SelectLevelAI()
    {
        GameManager.Instance.SavedTrackIndex = currentIndex + 1 + AILevels;
        StartCoroutine(TransitionToCar());
    }

    private IEnumerator TransitionToCar()
    {
        transitionHandler.Transition();

        yield return new WaitForSeconds(0.5f);

        ToggleUI(false);
        transform.GetChild(0).gameObject.SetActive(false);
        gamemodes.SetActive(false);

        CarSelectionHandler car = FindObjectOfType<CarSelectionHandler>();
        car.ToggleToCarSelection();

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = -1;
        }
    }

    public void SelectLevel(int index)
    {
        car.SetActive(true);
        regularTrack.SetActive(true);

        ToggleUI(false);

        for (int i = 0; i < tracks.Length; i++)
        {
            //tracks[i].SetActive(false);
        }

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = -1;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        StartCoroutine(SelectingLevel(index));
    }

    private IEnumerator SelectingLevel(int index)
    {
        yield return new WaitForSeconds(2);

        FindObjectOfType<CarMovement>().SetInputs(1, 0, 0);

        yield return new WaitForSeconds(2.8f);

        SceneManager.LoadScene(index);
    }
}

[System.Serializable]
public struct StarTimes
{
    public float[] Times;

    public StarTimes(float[] times)
    {
        Times = times;
    }
}
