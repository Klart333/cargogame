using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;

public class StartMenyButtons : MonoBehaviour
{
    public CarMovement carScript;
    public Camera cam;
    public Button startB;
    public Button settingB;
    public Button exitB;
    public Button exitSettingsB;
    public CinemachineVirtualCamera settingsVCam;
    //public CinemachineVirtualCamera normalVCam;

    [Header("Different shit")]
    [SerializeField]
    private GameObject mainField;

    [SerializeField]
    private GameObject settingsField;

    private float moveDuration = 1.5f;

    private bool inSettings = false;

    public void SettingsB(){
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, moveDuration);
        settingsVCam.Priority = 100;
        StartCoroutine(WaitThenDoShit());
    }

    private IEnumerator WaitThenDoShit()
    {
        mainField.SetActive(false);
        yield return new WaitForSeconds(moveDuration);

        settingsField.SetActive(true);
    }

    public void ExitSettingsB(){
        settingsVCam.Priority = 1;

        StartCoroutine(WaitThenDoDifferentShit());
    }

    private IEnumerator WaitThenDoDifferentShit()
    {
        settingsField.SetActive(false);
        yield return new WaitForSeconds(moveDuration);

        mainField.SetActive(true);
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
    }

    public void ExitB(){
        carScript.enabled = true;
        carScript.SetInputs(-1,0,0);
        settingB.interactable = false;
        startB.interactable = false;
    }

    private void OnTriggerEnter(Collider other) {
        //if(other.gameObject.tag == "start"){
        //    SceneManager.LoadScene("GameScene");
        //}
        //
        if(other.gameObject.tag == "exit"){
            Debug.Log("exit game");
            Application.Quit();
        }
    }

}
