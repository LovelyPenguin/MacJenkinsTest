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

    private float currentTime;
    private float currentFrame;

    // 테스트 할 시간
    [SerializeField]
    private float targetTime;

    // 기록의 기준이 될 최저 프레임
    [SerializeField]
    private float targetFrameRate;
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
    }

    void Update()
    {
        if (currentTime < targetTime)
            currentFrame = (int)(1f / Time.unscaledDeltaTime);
        frameRateText.text = currentFrame.ToString();

        currentTime += Time.deltaTime;
        elapsedTimeText.text = currentTime.ToString();

        if (currentTime >= targetTime)
        {
            Time.timeScale = 0f;
            Debug.Log("Test End");
        }
        if (currentFrame < targetFrameRate && currentTime >= 2f)
        {
            string tempText;
            tempText = "Time : " + currentTime + "\n" + "Frame : " + currentFrame + "\n" + "-----------------" + "\n";
            frameLogText.text += tempText;
            Debug.LogWarning(tempText);
        }
    }
}
