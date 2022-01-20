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
    public Button exitSettingsB;

    [SerializeField]
    private Transform camStartPos;
    
    [SerializeField]
    private Transform camEndPos;

    private bool inSettings = false;

    public void StartB(){
        carScript.SetInputs(1,0,0);
        Debug.Log("start");
        settingB.interactable = false;
        exitB.interactable = false;
    }

    public void SettingsB(){
        startB.gameObject.SetActive(false);
        exitB.gameObject.SetActive(false);
        settingB.gameObject.SetActive(false);
        StartCoroutine(LerpTo());
    }

    public void ExitSettingsB(){
        StartCoroutine(LerpFrom());
    }

    public void ExitB(){
        carScript.SetInputs(-1,0,0);
        settingB.interactable = false;
        startB.interactable = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "start"){
            SceneManager.LoadScene("GameScene");
        }

        if(other.gameObject.tag == "exit"){
            Debug.Log("exit game");
            Application.Quit();
        }
    }
    

    private void Update() {
        if(inSettings == true){
            exitSettingsB.gameObject.SetActive(true);
            //TODO Settings buttons
        }else{
            exitSettingsB.gameObject.SetActive(false);
        }
    }

    float lerpDuration = 1.5f; 
    float valueToLerp1;
    float valueToLerp2;
    float valueToLerp3;

    IEnumerator LerpTo()
    {
        float timeElapsed = 0;

        inSettings = true;

        while (timeElapsed < lerpDuration)
        {
            valueToLerp1 = Mathf.Lerp(camStartPos.position.x, camEndPos.position.x, timeElapsed / lerpDuration);
            valueToLerp2 = Mathf.Lerp(camStartPos.position.y,camEndPos.position.y,timeElapsed/lerpDuration);
            valueToLerp3 = Mathf.Lerp(camStartPos.position.z,camEndPos.position.z,timeElapsed/lerpDuration);
            timeElapsed += Time.deltaTime;
            
            cam.transform.position = new Vector3(valueToLerp1,valueToLerp2,valueToLerp3);

            yield return null;
        }
    }
    IEnumerator LerpFrom()
    {
        float timeElapsed = 0;

        inSettings = false;

        while (timeElapsed < lerpDuration)
        {
            valueToLerp1 = Mathf.Lerp(camEndPos.position.x, camStartPos.position.x, timeElapsed / lerpDuration);
            valueToLerp2 = Mathf.Lerp(camEndPos.position.y,camStartPos.position.y,timeElapsed/lerpDuration);
            valueToLerp3 = Mathf.Lerp(camEndPos.position.z,camStartPos.position.z,timeElapsed/lerpDuration);
            timeElapsed += Time.deltaTime;
            
            cam.transform.position = new Vector3(valueToLerp1,valueToLerp2,valueToLerp3);

            yield return null;
        }
    }

}
