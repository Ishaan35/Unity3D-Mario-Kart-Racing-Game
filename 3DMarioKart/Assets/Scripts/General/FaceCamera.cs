using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour {
	public bool DebugReport = false;
	Vector3 cameraDir;
	// Update is called once per frame
	void OnRenderObject() {
		if(CameraLocationTracker.cameraTransform != null)
		{
			Vector3 CamPos = transform.position + CameraLocationTracker.cameraTransform.rotation* Vector3.back;
			cameraDir = CamPos - (transform.position+Vector3.up/2f);
			CamPos.y = 0.3f;
			transform.LookAt(CamPos,Vector3.up);
		}

		float angle = Vector3.Angle(cameraDir.normalized,Vector3.up);
		angle = 90-angle;
		angle = angle/90;
		if(DebugReport)
			Debug.Log (cameraDir + "is the camera location, normalized Angle to camera is " +angle);
		angle = Mathf.Clamp01((angle*1.5f)-.5f);

		transform.localScale = new Vector3(1f,1f+angle*.375f,1f);

		//Debug.Log ("Script Active");
	}
}
