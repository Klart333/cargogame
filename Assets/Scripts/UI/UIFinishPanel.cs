using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIFinishPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private Image[] stars;

    private UILapTimer lapTimer;

    private void Start()
    {
        lapTimer = FindObjectOfType<UILapTimer>();

        timeText.text = lapTimer.DisplayTime(lapTimer.Timer);

        int index = GameManager.Instance.GetTrackIndex();
        for (int i = 0; i < stars.Length; i++)
        {
            if (lapTimer.Timer < Save.AllStarTimes[index].Times[i])
            {
                stars[i].color = Color.yellow;
            }
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);   
    }
}
