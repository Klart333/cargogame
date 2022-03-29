using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarNameController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainField;

    [SerializeField]
    private GameObject carField;

    [SerializeField]
    private CinemachineVirtualCamera carVCam;

    [SerializeField]
    private TesseractController tesseract;

    private float moveDuration = 1.5f;

    private void Awake()
    {
        PlayerPrefs.SetInt("SetName", 0);
    }

    private void OnMouseDown()
    {
        if (PlayerPrefs.GetInt("SetName") == 1)
        {
            return;
        }

        PlayerPrefs.SetInt("SetName", 1);
        GoEnterName();
    }

    public void GoEnterName()
    {
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, moveDuration);
        carVCam.Priority = 100;
        StartCoroutine(WaitThenDoShit());
    }

    private IEnumerator WaitThenDoShit()
    {
        mainField.SetActive(false);
        yield return new WaitForSeconds(moveDuration);

        carField.SetActive(true);
        FindObjectOfType<CarNamePainter>().ShouldDraw = true;
        tesseract.ShouldIntepret = true;
    }

    public void ExitName()
    {
        carVCam.Priority = 1;

        StartCoroutine(WaitThenDoDifferentShit());
    }

    private IEnumerator WaitThenDoDifferentShit()
    {
        carField.SetActive(false);
        yield return new WaitForSeconds(moveDuration);


        mainField.SetActive(true);
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        FindObjectOfType<CarNamePainter>().ShouldDraw = false;
        tesseract.ShouldIntepret = false;

        PlayerPrefs.SetString("Name", tesseract.CurrentInterpretation);
        FindObjectOfType<PaintParent>().SavePaint();
    }
}
