using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitBenchMarkScript : MonoBehaviour
{
	public string sceneName;
    // Use this for initialization
    void Start()
    {

    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
