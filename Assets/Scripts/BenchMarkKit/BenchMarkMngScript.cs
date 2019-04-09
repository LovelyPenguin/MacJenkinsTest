using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField]
    private TextMeshProUGUI benchmarkEndText;

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

    private string screenshotPath;
    void Start()
    {
        // 재시작 했을 경우를 대비함
        Time.timeScale = 1f;

        Debug.Log("BenchMark Kit Start");
        if (frameRateText == null || elapsedTimeText == null)
        {
            Debug.LogError("frameRate or ElapsedTime가 비어있습니다.");
        }
        deviceNameText.text =
        "Device Name : " + SystemInfo.deviceModel + "\n" +
        "Processor : " + SystemInfo.processorType + " " + SystemInfo.processorFrequency * 0.001f + " Ghz " + SystemInfo.processorCount + " Thread" + "\n" +
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
            //TakeScreenShot();
            StartCoroutine(captureScreenshot());
            RefreshGallery();
            benchmarkEndText.gameObject.active = true;
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
                // 스크린샷 찍을때 프레임이 드랍된다
                //StartCoroutine(captureScreenshot());
            }
        }

        // 테스트용
        // if (Input.GetKey(KeyCode.Space))
        // {
        //     benchmarkEndText.gameObject.active = true;
        // }
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

            // 어짜피 모바일 기기에서 60fps를 넘겨봤자 의미가 없음 오히려 평균을 구할때 방해만 되기에 제한함
            if (maximumFrameRate > 60)
                maximumFrameRate = 60;
            Debug.Log("Maximum Frame Rate : " + maximumFrameRate);
        }

        if (minimumFrameRate > currentFrame)
        {
            minimumFrameRate = currentFrame;
            Debug.Log("Minimum Frame Rate : " + minimumFrameRate);
        }
    }

    // private void TakeScreenShot()
    // {
    //     if (screenShot)
    //     {
    //         // 에디터에선 이상하게 에러가 뜨는데 모바일에선 정상 작동함
    //         // 사용하기 전 SD카드 권한을 획득해야 함
    //         Debug.Log("Take Screenshot");
    //         string drive = "/mnt/sdcard/DCIM/BenchmarkResult/";

    //         DirectoryInfo di = new DirectoryInfo(drive);

    //         if (di.Exists == false)
    //             di.Create();

    //         string timeStamp = System.DateTime.Now.ToString("yyyy.MM.dd(HH:mm:ss)");
    //         string text = drive + "BenchmarkReult" + timeStamp + ".png";
    //         string pathToSave = text;
    //         ScreenCapture.CaptureScreenshot(pathToSave, 1);
    //         screenShot = false;
    //     }
    // }

    IEnumerator captureScreenshot()
    {
        yield return new WaitForEndOfFrame();
        if (screenShot)
        {
            string albumPath = "/mnt/sdcard/DCIM/BenchmarkResult/";
            DirectoryInfo di = new DirectoryInfo(albumPath);

            if (di.Exists == false)
                di.Create();

            screenshotPath = albumPath + "Benchmark" + System.DateTime.Now.ToString("yyyy.MM.dd(HH:mm:ss)") + ".jpeg";

            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
            //Get Image from screen
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            //Convert to png
            byte[] imageBytes = screenImage.EncodeToPNG();

            //Save image to file
            System.IO.File.WriteAllBytes(screenshotPath, imageBytes);
            screenShot = false;
        }
    }

    // 앨범 새로고침 기능 들어보니 안드로이드든 iOS든 다 작동한다고 한다.
    // 이거 없으면 스샷 볼때마다 폰 껐다 켜야함
    // 다만 구버전에선(ex)젤리빈) 기능하지 않는듯 함
    private void RefreshGallery()
    {
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        
        AndroidJavaObject objIntent = 
        new AndroidJavaObject("android.content.Intent", 
        new object[2]{"android.intent.action.MEDIA_SCANNER_SCAN_FILE", 
        classUri.CallStatic<AndroidJavaObject>("parse", "file://" + screenshotPath)});

        objActivity.Call ("sendBroadcast", objIntent);
    }
}
