using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SetChild : MonoBehaviour {

// Use this for initialization
	void OnEnable () {
		GetComponent<Camera>().targetTexture = SetRes.SceneBuffer;
	}
	// Update is called once per frame
	void Update () {

		if(!GetComponent<Camera>().targetTexture)
			GetComponent<Camera>().targetTexture = SetRes.SceneBuffer;
	}
}
