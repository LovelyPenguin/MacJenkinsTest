using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GetDateScript : MonoBehaviour {
    [SerializeField]
    private uint bundleVersionCode = 90;
    [SerializeField]
    private float version = 0.9f;
	// Use this for initialization
	void Start () {
        string date = System.DateTime.Now.ToString("yyyy.MM.dd") + System.DateTime.Now.ToString("(HH:mm:ss)") + "-" + bundleVersionCode.ToString() + "-" + version.ToString();
        Debug.Log(date);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
