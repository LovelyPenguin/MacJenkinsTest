using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class DisplayVersion : MonoBehaviour {
    [SerializeField]
    private Text bundleVersionText;
    [SerializeField]
    private Text versionText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //bundleVersionText.text = Application.version;
        versionText.text = Application.version;
    }
}
