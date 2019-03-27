using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ProjectBuilder
{
    static string[] SCENES = FindEnabledEditorScene();
    static string TARGET_DIR = "Build";

    // 번들버전 코드는 무조건 int형으로 만들어야 함!
    // 번들버전코드는 절대 중복되지도 못하고 절대 이전보다 낮은 숫자로는 변경할 수는 없음
    static int bundleVersionCode = 5;

    // 마켓에서 보이는 버전코드인데 수정하지 않더라도 괜찮음
    // 마켓 업데이트가 보이 때문에 몰래 업데이트는 못하겠지만 사용자가 신경 못쓴다면 잠수함 패치가 가능함
    static float gameVersion = 0.2f;

    private static string[] FindEnabledEditorScene()
    {
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    static void AndroidBuild(BuildOptions option = BuildOptions.None)
    {
        //string androidDir = "/Android";
        string androidDir = "/output";

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + androidDir + string.Format("/AndroidBuild_{0}.apk", PlayerSettings.bundleVersion);
        //string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + string.Format("/AndroidBuild_{0}.apk", PlayerSettings.bundleVersion);

        // 번들버전이 같으면 앱이 올라가지 않으니 날짜를 이용해서 빌드버전을 올리는 모양이다.
        //PlayerSettings.Android.bundleVersionCode = (Int32)(DateTime.UtcNow.Subtract(new DateTime(2000, 2, 22))).TotalSeconds;

        //set the other settings from environment variables
        /*
        PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_NAME");
        PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASSWORD");
        PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
        PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASSWORD");
        */
        // 환경변수로 하는 방법이 먹히지 않아 직접적으로 입력해줌
        PlayerSettings.Android.keystoreName = "/Users/Shared/Jenkins/VoxellersTestKey.keystore";
        PlayerSettings.Android.keystorePass = "woong8589";
        PlayerSettings.Android.keyaliasName = "key0";
        PlayerSettings.Android.keyaliasPass = "woong8589";

        // 이 부분은 자동화가 필요함
        // 빌드 버전을 스프레드시트로 이용하는 방법은 여러모로 위험요소가 많다
        // 플러그인 중 하나라도 문제가 생기면 개판난다.
        // 딱 현재 그꼴나서 사용이 불가능하므로 다른 방법을 고려해보는 방법을 생각해봐야한다.
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.bundleVersion = gameVersion.ToString();

        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTargetGroup.Android, BuildTarget.Android, option, "Android_BuildReport");
    }

    static void GenericBuild(string[] scenes, string target_path, BuildTargetGroup buildTargetGroup, BuildTarget build_target, BuildOptions build_options, string buildReportFileName = "BuildReport")
    {
        // 날짜 포맷 결정 방법은 아래 참고.
        // http://www.csharpstudy.com/Tip/Tip-datetime-format.aspx
        DateTime currentTIme = DateTime.Now;
        string dateToFileName = string.Format("{0:yyyy-MM-dd-HHmmss}", currentTIme);

        // identifier 설정. iOS와 Android 모두 하나의 세팅으로 해결 가능한 것으로 보임.
        PlayerSettings.applicationIdentifier = "com.Voxellers.JenkinsTestBuild"; // 빌드 세팅에 필요한 해당 정보 등은 특정한 Custom Editor로 모아 편집이 가능하도록 수정해야할 것으로 보임.

        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, build_target);
        UnityEditor.Build.Reporting.BuildReport res = BuildPipeline.BuildPlayer(scenes, target_path, build_target, build_options);

        // 빌드 번호 설정해줘야 함.
        BuildReportMaker buildReportMaker = new BuildReportMaker(buildReportFileName, res, "/BuildReport");
    }

    [MenuItem("Custom/CI/Build PC")]
    static void PerformPCBuildClient()
    {
        string pcDir = "/PC";
        BuildOptions opt = BuildOptions.None;

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + pcDir + string.Format("/PCBuild_{0}.exe", PlayerSettings.bundleVersion);
        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, opt, "PC_BuildReport");
    }

    [MenuItem("Custom/CI/Build_Android")]
    static void PerformAndroidBuildClient()
    {
        AndroidBuild();
    }

    [MenuItem("Custom/CI/Build_Android_Debug")]
    static void PerformAndroidBuildClientDebug()
    {
        BuildOptions opt = BuildOptions.Development | BuildOptions.ConnectWithProfiler;

        AndroidBuild(opt);
    }

    [MenuItem("Custom/CI/Build_Android_Debug_AutoRun")]
    static void PerformAndroidBuildClientDebugAutoRun()
    {
        BuildOptions opt = BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.ConnectWithProfiler;

        AndroidBuild(opt);
    }

}
