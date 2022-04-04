using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TesseractController : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI displayText;

    [SerializeField]
    private TextMeshProUGUI countdownText;

    [SerializeField]
    private TextMeshProUGUI limitText;

    [SerializeField]
    private GameObject clearButton;

    private TesseractDriver _tesseractDriver;
    private Button displayButton;

    private float intepretFrequency = 7.5f;
    private float timer = 0;

    public bool ShouldIntepret { get; set; } = false;
    public string CurrentInterpretation { get; private set; }

    private void Start()
    {
        displayButton = displayText.GetComponent<Button>();

        _tesseractDriver = new TesseractDriver();
        Recoginze();
    }

    private void Recoginze()
    {
        _tesseractDriver.CheckTessVersion();
        _tesseractDriver.Setup(Nothing);
    }

    private void Nothing()
    {

    }

    private void Update()
    {
        if (string.IsNullOrEmpty(CurrentInterpretation))
        {
            displayButton.enabled = false;
        }
        else
        {
            displayButton.enabled = true;
        }
    }

    private IEnumerator RecordFrame()
    {
        displayText.enabled = false;
        countdownText.enabled = false;
        limitText.enabled = false;
        clearButton.SetActive(false);

        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        // do something with texture

        SetText(_tesseractDriver.Recognize(texture));

        // cleanup
        Object.Destroy(texture);

        displayText.enabled = true; 
        countdownText.enabled = true;
        limitText.enabled = true;
        clearButton.SetActive(true);
    }

    public void LateUpdate()
    {
        if (ShouldIntepret)
        {
            timer += Time.deltaTime;
            if (timer >= intepretFrequency)
            {
                timer = 0;
                StartCoroutine(RecordFrame());
            }

            countdownText.text = ((Mathf.RoundToInt((intepretFrequency - timer) * 10)) / 10.0f).ToString();
        }
    }

    private void SetText(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        CurrentInterpretation = text;
        displayText.text = string.Format("My name is: {0}", text);
    }
}