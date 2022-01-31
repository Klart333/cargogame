using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFinishPanel : MonoBehaviour
{
    public void Finish()
    {
        SceneManager.LoadScene(0);   
    }
}
