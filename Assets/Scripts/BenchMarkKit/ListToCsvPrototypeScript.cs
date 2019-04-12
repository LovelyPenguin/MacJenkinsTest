// 프로토타입이라 아직 적용은 안하고 테스트만 진행했음
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;
using System.IO;

public class ListToCsvPrototypeScript : MonoBehaviour
{
    // PerformanceCounter cpuCounter;
    // PerformanceCounter ramCounter;
    List<string> penguin = new List<string>();
    float time;
    string info;
    int number;
    string filePath = "C:/Users/yun_pyo_Lee/Documents/List.txt";
    void Start()
    {
        Debug.Log("Write start");
        penguin.Add("Time,Random,number");
    }

    void Update()
    {
        time += Time.deltaTime;

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        number++;
        string tempInfo =
        time.ToString() + "," +
        Random.Range(0, 100).ToString() + "," +
        number.ToString();
        penguin.Add(tempInfo);
        //}
        if (Input.GetKeyDown(KeyCode.S))
        {
            // for (int i = 0; i < penguin.Count; i++)
            // {
            // 	Debug.Log(penguin[i]);
            // }
            using (TextWriter tw = new StreamWriter(filePath))
            {
                foreach (string s in penguin)
                {
                    tw.WriteLine(s);
                }
            }
			File.Move(filePath, Path.ChangeExtension(filePath, ".csv"));
        }
    }//
}
