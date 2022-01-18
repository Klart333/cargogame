using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenyButtons : MonoBehaviour
{
    public CarMovement carScript;
    public Camera cam;
    public Button startB;
    public Button settingB;
    public Button exitB;

    public void Start(){
       // carScript.SetInputs(1,0,0);
        Debug.Log("start");
        settingB.interactable = false;
        exitB.interactable = false;
    }

    public void Setting(){
        startB.gameObject.SetActive(false);
        exitB.gameObject.SetActive(false);
    }

    public void ExitSettings(){

    }

    public void Exit(){
        //carScript.SetInputs(-1,0,0);
        settingB.interactable = false;
        startB.interactable = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "start"){
            //TODO Change to game scene
        }

        if(other.gameObject.tag == "exit"){
            Debug.Log("exit game");
            Application.Quit();
        }
    }
}
