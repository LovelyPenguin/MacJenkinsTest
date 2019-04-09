using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BenchMarkMngScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI frameRateText;
    [SerializeField]
    private TextMeshProUGUI elapsedTimeText;
    [SerializeField]
    private TextMeshProUGUI deviceNameText;
    [SerializeField]
    private TextMeshProUGUI frameLogText;
    [SerializeField]
    private TextMeshProUGUI maxFrameRateText;
    [SerializeField]
    private TextMeshProUGUI minFrameRateText;
    [SerializeField]
    private TextMeshProUGUI avgFrameRateText;

    private float currentTime;
    private float currentFrame;

    // 테스트 할 시간
    [SerializeField]
    private float targetTime;
    // 기록의 기준이 될 최저 프레임
    [SerializeField]
    private float targetFrameRate;
    // 벤치마크 대기 시간
    [SerializeField]
    private float disableTime = 2f;

    private bool testAvailable = true;
    private bool screenShot = true;
    private float saveFrameRate = 0f;
    private float minimumFrameRate;
    private float maximumFrameRate;
    private float averageFrameRate = 0f;
    void Start()
    {
        Debug.Log("BenchMark Kit Start");
        if (frameRateText == null || elapsedTimeText == null)
        {
            Debug.LogError("frameRate or ElapsedTime가 비어있습니다.");
        }
        deviceNameText.text =
        "Device Name : " + SystemInfo.deviceModel + "\n" +
        "Processor : " + SystemInfo.processorType + "\n" +
        "Memory Size : " + SystemInfo.systemMemorySize + "\n" +
        "Graphics Processor : " + SystemInfo.graphicsDeviceName + "\n" +
        "Graphics Memory Size : " + SystemInfo.graphicsMemorySize;

        currentFrame = 0f;
        currentTime = 0f;
        maximumFrameRate = 0f;
        minimumFrameRate = 60f;
    }

    void Update()
    {
        // Benchmark End
        if (currentTime >= targetTime)
        {
            Time.timeScale = 0f;
            testAvailable = false;
            Debug.Log("Average Frame Rate : " + averageFrameRate);
            TakeScreenShot();
        }

        // Benchmark Running
        if (testAvailable)
        {
            UpdateText();
        }

        // Benchmark running after 2 second
        if (currentTime >= disableTime)
        {
            GetMinMaxFrameRate();
            
            maxFrameRateText.text = maximumFrameRate.ToString();
            minFrameRateText.text = minimumFrameRate.ToString();

            averageFrameRate = (maximumFrameRate + minimumFrameRate) / 2;
            avgFrameRateText.text = averageFrameRate.ToString();

            if (currentFrame < targetFrameRate && testAvailable)
            {
                WriteFrameLog();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // string text = "BenchmarkEnd" + System.DateTime.Now.ToString("yyyy.MM.dd(HH:mm:ss)") + ".png";
            // ScreenCapture.CaptureScreenshot(text, 1);

            Debug.Log(Application.persistentDataPath);
        }
    }

    private void UpdateText()
    {
        currentFrame = (int)(1f / Time.unscaledDeltaTime);
        frameRateText.text = currentFrame.ToString();

        currentTime += Time.deltaTime;
        elapsedTimeText.text = currentTime.ToString();
    }

    private void WriteFrameLog()
    {
        string tempText;
        tempText = "Time : " + currentTime + ", " + "Frame : " + currentFrame + "\n";
        frameLogText.text += tempText;
        Debug.LogWarning(tempText);
    }

    private void GetMinMaxFrameRate()
    {
        if (maximumFrameRate < currentFrame)
        {
            maximumFrameRate = currentFrame;
            Debug.Log("Maximum Frame Rate : " + maximumFrameRate);
        }

        if (minimumFrameRate > currentFrame)
        {
            minimumFrameRate = currentFrame;
            Debug.Log("Minimum Frame Rate : " + minimumFrameRate);
        }
    }

    private void TakeScreenShot()
    {
        if (screenShot)
        {
            Debug.Log("Take Screenshot");
            string text = "BenchmarkEnd" + System.DateTime.Now.ToString("yyyy.MM.dd(HH:mm:ss)") + ".png";
            ScreenCapture.CaptureScreenshot(text, 1);
            screenShot = false;
        }
    }
}
