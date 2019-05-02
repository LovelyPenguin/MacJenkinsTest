using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotMng : MonoBehaviour
{

    private static ScreenShotMng instance;

    [SerializeField]
    private Camera myCam;
    private bool takeScreenShotOnNextFrame;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        myCam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnPostRender()
    {
        if (takeScreenShotOnNextFrame)
        {
            takeScreenShotOnNextFrame = false;
            RenderTexture renderTexture = myCam.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/BenchmarkResult" + System.DateTime.Now.ToString("yyyy.MM.dd(HHmmss)") + ".png", byteArray);
            Debug.Log("Saved Screenshot");

            RenderTexture.ReleaseTemporary(renderTexture);
            myCam.targetTexture = null;
        }
    }

    private void TakeScreenShot(int width, int height)
    {
        myCam.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenShotOnNextFrame = true;
    }

    public static void TakeScreenShot_Static(int width, int height)
    {
        instance.TakeScreenShot(width, height);
    }
}
