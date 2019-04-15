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
    static string TEMP_DIR = "Temp";
    static string buildDivision;
    static string buildBackupDivisionLocation;

    // 번들버전 코드는 무조건 int형으로 만들어야 함!
    // 번들버전코드는 절대 중복되지도 못하고 절대 이전보다 낮은 숫자로는 변경할 수는 없음
    static int bundleVersionCode;

    // 마켓에서 보이는 버전코드인데 수정하지 않더라도 괜찮음
    // 마켓 업데이트가 보이기 때문에 몰래 업데이트는 못하겠지만 사용자가 신경 못쓴다면 잠수함 패치가 가능함
    static string gameVersion;

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
        GetCSVData csv = new GetCSVData();
        csv.GetNewVersion(out bundleVersionCode, out gameVersion);

        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.bundleVersion = gameVersion.ToString();

        //string androidDir = "/Android";
        string androidDir = "/Output";

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + androidDir + string.Format("/{2}AndroidBuild_{0}_{1}.apk", PlayerSettings.Android.bundleVersionCode, PlayerSettings.bundleVersion, buildDivision);
        string copyAndMove = Path.GetFullPath(".") + sep + TARGET_DIR + buildBackupDivisionLocation + string.Format("/{2}AndroidBuild_{0}_{1}.apk", PlayerSettings.Android.bundleVersionCode, PlayerSettings.bundleVersion, buildDivision);

        DirectoryInfo outputDirectory = new DirectoryInfo(Path.GetFullPath(".") + sep + TARGET_DIR + androidDir);
        DirectoryInfo backupDirectory = new DirectoryInfo(Path.GetFullPath(".") + sep + TARGET_DIR + buildBackupDivisionLocation);
        DirectoryInfo tempDirectory = new DirectoryInfo(Path.GetFullPath(".") + sep + TEMP_DIR);
        //string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + string.Format("/AndroidBuild_{0}.apk", PlayerSettings.bundleVersion);

        // Output 디렉토리가 없을때
        FolderSershAndCreate(outputDirectory);
        // Backup 디렉토리가 없을때
        FolderSershAndCreate(backupDirectory);
        // Temp 디렉토리가 없을때 
        FolderSershAndCreate(tempDirectory);

        /*
        // 젠킨스에선 환경변수가 작동하지 않음
        PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_NAME");
        PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASSWORD");
        PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
        PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASSWORD");
        */

        PlayerSettings.Android.keystoreName = "/Users/Shared/Jenkins/Development/VoxellersTestKey.keystore";
        PlayerSettings.Android.keystorePass = "woong8589";
        PlayerSettings.Android.keyaliasName = "key0";
        PlayerSettings.Android.keyaliasPass = "woong8589";

        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTargetGroup.Android, BuildTarget.Android, option, "Android_BuildReport" + "_" + PlayerSettings.Android.bundleVersionCode + "_" + PlayerSettings.bundleVersion);

        // /Output에서 나온 앱을 Backup폴더로 복사함
        File.Copy(BUILD_TARGET_PATH, copyAndMove, false);
    }

    static void GenericBuild(string[] scenes, string target_path, BuildTargetGroup buildTargetGroup, BuildTarget build_target, BuildOptions build_options, string buildReportFileName = "BuildReport")
    {
        // 날짜 포맷 결정 방법은 아래 참고.
        // http://www.csharpstudy.com/Tip/Tip-datetime-format.aspx
        // 이거 안쓰이는 것 같은데?
        DateTime currentTIme = DateTime.Now;
        string dateToFileName = string.Format("{0:yyyy.MM.dd(HHmmss)}", currentTIme);

        // identifier 설정. iOS와 Android 모두 하나의 세팅으로 해결 가능한 것으로 보임.
        PlayerSettings.applicationIdentifier = "com.Voxellers.JenkinsTestBuild"; // 빌드 세팅에 필요한 해당 정보 등은 특정한 Custom Editor로 모아 편집이 가능하도록 수정해야할 것으로 보임.

        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, build_target);
        UnityEditor.Build.Reporting.BuildReport res = BuildPipeline.BuildPlayer(scenes, target_path, build_target, build_options);

        char sep = Path.DirectorySeparatorChar;
        string REPORT_TARGET_PATH = Path.GetFullPath(".") + sep + "/BuildReport";

        DirectoryInfo di = new DirectoryInfo(REPORT_TARGET_PATH);
        FolderSershAndCreate(di);

        // 빌드 번호 설정해줘야 함.
        BuildReportMaker buildReportMaker = new BuildReportMaker(buildReportFileName, res, REPORT_TARGET_PATH);
    }

    // 폴더가 있는지 검사하고 없으면 만들어주는 함수
    static private void FolderSershAndCreate(DirectoryInfo folder)
    {
        if (!folder.Exists)
            folder.Create();
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
        buildDivision = "Irregular";
        buildBackupDivisionLocation = "/IrregularBuildBackup";
        AndroidBuild();
    }

    static void RegularAndroidBuild()
    {
        buildDivision = "Regular";
        buildBackupDivisionLocation = "/RegularBuildBackup";
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
