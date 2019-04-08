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
		currentFrame += Time.deltaTime;
		elapsedTimeText.text = currentFrame.ToString();
    }
}
