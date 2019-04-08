using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCreater : MonoBehaviour
{

    [SerializeField]
    private float time = 1f;
    private float saveTime;

    [SerializeField]
    private GameObject cube;
	[SerializeField]
	private Transform cubeSave;

    void Start()
    {
        if (cube == null)
        {
			Debug.LogError("Cube is null");
        }
    }

    void Update()
    {
        saveTime += Time.deltaTime;

        if (saveTime >= time && cube != null)
        {
            Instantiate(cube, transform.position, Quaternion.identity, cubeSave);
            saveTime = 0f;
        }
    }
}
