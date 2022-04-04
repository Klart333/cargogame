using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using System.Linq;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject[] toHide;

    [SerializeField]
    private CinemachineVirtualCamera[] cameras;

    [SerializeField]
    private GameObject[] tracks;

    [SerializeField]
    private Material black;

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
    private TextMeshProUGUI playText;

    [SerializeField]
    private Button aiButton;

    [SerializeField]
    private GameObject selectLevel;

    [SerializeField]
    private GameObject gamemodes;

    [Header("PB")]
    [SerializeField]
    private TextMeshProUGUI[] wrTexts;

    [SerializeField]
    private TextMeshProUGUI pbText;

    [SerializeField]
    private Image[] stars;

    private UITransitionHandler transitionHandler;

    private List<bool> completedTracks = new List<bool>();
    private int currentIndex = 0;
    private Highscore[] savedHighscores;

    

    private void Start()
    {
        transitionHandler = FindObjectOfType<UITransitionHandler>();

        LockLevels();
    }

    private void LockLevels()
    {
        completedTracks = Save.GetCompletedTracks();

        for (int i = 1; i < tracks.Length; i++)
        {
            if (completedTracks[i - 1])
            {
                continue;
            }

            MeshRenderer[] renderers = tracks[i].GetComponentsInChildren<MeshRenderer>();
            for (int g = 0; g < renderers.Length; g++)
            {
                var mats = renderers[g].sharedMaterials;
                for (int h = 0; h < mats.Length; h++)
                {
                    mats[h] = black;
                }

                renderers[g].sharedMaterials = mats;
            }
        }
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
        transitionHandler.SmolTransition();

        yield return new WaitForSeconds(0.5f);

        currentIndex = index;
        if (currentIndex >= tracks.Length)
        {
            currentIndex = 0;
        }

        titleText.text = titles[currentIndex];
        if (currentIndex == 0 || completedTracks[currentIndex - 1]) // If it's unlocked
        {
            playText.text = "This one seems good";
            playText.GetComponent<Button>().enabled = true;
        }
        else
        {
            playText.text = "It's locked, mate";
            playText.GetComponent<Button>().enabled = false;
        }

        if (currentIndex == 0)
        {
            aiButton.interactable = true;
        }
        else
        {
            aiButton.interactable = false;
        }

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
        transitionHandler.SmolTransition();

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

        if (savedHighscores != null)
        {
            ActuallyDisplayWR();
        }
        else
        {
            StartDisplaying();
        }
    }

    private void StartDisplaying()
    {
        GlobalHighscores.Instance.StartCoroutine(GlobalHighscores.Instance.DownloadHighscores(GetHighscores));

        for (int i = 0; i < wrTexts.Length; i++)
        {
            if (string.IsNullOrEmpty(wrTexts[i].text))
            {
                wrTexts[i].text = "please chill";
            }
            
        }
    }

    private void GetHighscores(Highscore[] highscores)
    {
        savedHighscores = highscores;
        StartCoroutine(PleaseChill());
    }

    private IEnumerator PleaseChill()
    {
        yield return new WaitForSeconds(0.1f);
        ActuallyDisplayWR();
    }

    private void ActuallyDisplayWR()
    {
        List<float> scores = new List<float>();
        List<int> takenScores = new List<int>();

        for (int i = 0; i < savedHighscores.Length; i++)
        {
            if (savedHighscores[i].TrackIndex == currentIndex)
            {
                scores.Add(savedHighscores[i].Score);
            }
        }
        scores.Sort();

        for (int i = 0; i < savedHighscores.Length; i++)
        {
            if (savedHighscores[i].TrackIndex == currentIndex)
            {
                int minuteTens, minutes, tens, seconds, tenths, hundreths;
                ParseTime(savedHighscores[i].Score, out minuteTens, out minutes, out tens, out seconds, out tenths, out hundreths);

                int placement = -1;
                for (int g = 0; g < scores.Count; g++)
                {
                    if (savedHighscores[i].Score == scores[g] && !takenScores.Contains(g))
                    {
                        placement = g;
                        takenScores.Add(g);

                        break;
                    }
                }

                wrTexts[placement].text = string.Format("{6} - {0}{1}:{2}{3}:{4}{5}", minuteTens, minutes, tens, seconds, tenths, hundreths, savedHighscores[i].Name);

                float time = Save.GetTrackTime(currentIndex);
                if (Mathf.Abs(time - savedHighscores[i].Score) < 0.05f && savedHighscores[i].Name == PlayerPrefs.GetString("Name"))
                {
                    pbText.text = string.Format("that's you ↑");

                    for (int g = 0; g < stars.Length; g++)
                    {
                        stars[g].color = Color.yellow;
                    }
                }
            }
        }

        for (int i = 0; i < wrTexts.Length; i++)
        {
            if (wrTexts[i].text == "please chill")
            {
                wrTexts[i].text = "No one :(";
            }
        }
        
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

            return;
        }

        int minuteTens, minutes, tens, seconds, tenths, hundreths;
        ParseTime(time, out minuteTens, out minutes, out tens, out seconds, out tenths, out hundreths);
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

    private void ParseTime(float time, out int minuteTens, out int minutes, out int tens, out int seconds, out int tenths, out int hundreths)
    {
        float _time = time;
        minuteTens = Mathf.FloorToInt(_time / 600.0f);
        _time -= minuteTens * 600;
        minutes = Mathf.FloorToInt(_time / 60.0f);
        _time -= minutes * 60;
        tens = Mathf.FloorToInt(_time / 10.0f);
        _time -= tens * 10;
        seconds = Mathf.FloorToInt(_time);
        _time -= seconds;
        tenths = Mathf.FloorToInt(_time * 10);
        _time -= tenths / 10.0f;
        hundreths = Mathf.FloorToInt(_time * 100);
    }

    public void SelectLevelTimeAttack()
    {
        GameManager.Instance.SavedTrackIndex = currentIndex + 1;
        StartCoroutine(TransitionToCar());
    }

    public void SelectLevelAI()
    {
        GameManager.Instance.SavedTrackIndex = currentIndex + 1 + Save.AmountOfTracks;
        StartCoroutine(TransitionToCar());
    }

    private IEnumerator TransitionToCar()
    {
        transitionHandler.SmolTransition();

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

        GameManager.Instance.LoadLevel(index);
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
