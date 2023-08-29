using UnityEngine;
using System.Collections;
#if UNITY_EDITOR_WIN
using UnityEditor;
#endif
[ExecuteInEditMode]
public class MatchOrientation : MonoBehaviour {
	public bool TrackInEditor = false;
	public GameObject ReferenceObj;
	// Use this for initialization
	Vector3 oldPos = Vector3.zero;
	Vector3 pos = Vector3.zero;
	void Start(){
		//transform.parent = CameraLocationTracker.cameraTransform;
	}

	// Update is called once per frame
	void OnRenderObject () {
		if(Application.isPlaying)
		{	transform.position = CameraLocationTracker.cameraLoc;
		}
		#if UNITY_EDITOR_WIN
		if(TrackInEditor)
		{
			//gameObject.transform.rotation = ReferenceObj.transform.rotation;
			if(CameraLocationTracker.cameraTransform)
				transform.position = CameraLocationTracker.cameraLoc;

		}
		#endif
	}

}
