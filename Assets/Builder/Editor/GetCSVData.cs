﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
//using UnityEngine;

public class GetCSVData
{

    static string csvFilePath = "/Users/Shared/Jenkins/Development/VersionSheetTestExample.csv";

    private int setBundleVersionCode;
    private string setVersion;

    private void CSVOpen()
    {
        List<Dictionary<string, object>> data = CSVReader.Read(csvFilePath);

        for (int i = 0; i < data.Count; i++)
        {
            //Debug.Log("index " + i.ToString() + ": " + data[i]["Date"] + " " + data[i]["BundleVersionCode"] + " " + data[i]["Version"]);
            setBundleVersionCode = int.Parse(data[i]["BundleVersionCode"].ToString());
            setVersion = data[i]["Version"].ToString();
        }
    }

    private void CSVSave()
    {
        CSVOpen();

        ChangeToTextFile(csvFilePath, ".txt");
        csvFilePath = csvFilePath.Replace(".csv", ".txt");

        // 코드입력
        using (StreamWriter outputFile = new StreamWriter(csvFilePath, true))
        {
            // outputFile.WriteLine("{0},{1},{2}", System.DateTime.Now.ToString("yyyy.MM.dd") + System.DateTime.Now.ToString("(HH:mm:ss)"), setBundleVersionCode + 1, setVersion);
            int tempBundleVersion = setBundleVersionCode+1;
            string day = System.DateTime.Now.ToString("yyyy.MM.dd");
            string time = System.DateTime.Now.ToString("(HH:mm:ss)");
            outputFile.WriteLine("{0},{1},{2}", day+time, tempBundleVersion, day + "-" + tempBundleVersion);
        }
        // 코드입력

        ChangeToTextFile(csvFilePath, ".csv");
        csvFilePath = csvFilePath.Replace(".txt", ".csv");
    }

    // 파일의 확장자 변경
    // 담부턴 주석 좀 잘 쓰자
    private static void ChangeToTextFile(string filePath, string changeExtention)
    {
        File.Move(filePath, Path.ChangeExtension(filePath, changeExtention));
    }

    public void GetNewVersion(out int bundleVersion, out string version)
    {
        CSVSave();
        CSVOpen();
        bundleVersion = setBundleVersionCode;
        version = setVersion;
    }
}
