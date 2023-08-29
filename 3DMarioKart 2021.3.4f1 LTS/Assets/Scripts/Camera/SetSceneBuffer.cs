using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SetSceneBuffer : MonoBehaviour {
	// Update is called once per frame

	void Update () {
		if(!GetComponent<Camera>().targetTexture)
			GetComponent<Camera>().targetTexture = SetRes.SceneBuffer;
	}
}
