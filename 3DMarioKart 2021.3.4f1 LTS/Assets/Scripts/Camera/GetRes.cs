using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class GetRes : MonoBehaviour {
	bool resSet = false;
	void Start()
	{
		gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = SetRes.SceneBuffer;
	}

	// Update is called once per frame
	void Update() {
		if(!resSet)
		{
			if(!gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture)
				gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = SetRes.SceneBuffer;
			float pos = (Camera.main.nearClipPlane + 0.01f);
			transform.localPosition = pos*Vector3.forward;
			float h = Mathf.Tan(Camera.main.fieldOfView*Mathf.Deg2Rad*0.5f)*pos*2f;
			transform.localScale = new Vector3(h*Camera.main.aspect,h,0f);
			resSet = true;
		}
	}
}

