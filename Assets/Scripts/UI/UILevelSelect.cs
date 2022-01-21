using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject[] toHide;

    public void SelectLevel(int index)
    {
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }


        StartCoroutine(SelectingLevel(index));
    }

    private IEnumerator SelectingLevel(int index)
    {
        FindObjectOfType<CarMovement>().SetInputs(1, 0, 0);

        yield return new WaitForSeconds(2.8f);

        SceneManager.LoadScene(index);
    }
}
