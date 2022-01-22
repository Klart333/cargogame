using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject[] toHide;

    [SerializeField]
    private CinemachineVirtualCamera[] cameras;

    [SerializeField]
    private GameObject[] tracks;

    [SerializeField]
    private GameObject regularTrack;

    [SerializeField]
    private GameObject car;

    [Header("Titles")]
    [SerializeField]
    private string[] titles;

    [SerializeField]
    private TextMeshProUGUI titleText;

    private int currentIndex = 0;

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

    public void DisplayLevel(int index)
    {
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
        ToggleUI(true);

        car.SetActive(true);
        regularTrack.SetActive(true);

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
        SelectLevel(currentIndex + 1);
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
