using UnityEngine;
#if UNITY_EDITOR_WIN
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
[ExecuteInEditMode]
public class CameraLocationTracker : MonoBehaviour {
	public string RuntimeCamTag = "RenderCamera";
	public bool DebugReport = false;
	public static Vector3 cameraLoc;
	public static Transform cameraTransform;
	public static int frameCount = 0;
	public static float Aspect;
	public static float FOV;
	public static GameObject CameraToTrack;
	// Update is called once per frame
	void Start () {
		if(Application.isPlaying)
		{
			CameraToTrack = GameObject.FindGameObjectWithTag(RuntimeCamTag);
			FOV = CameraToTrack.GetComponent<Camera>().fieldOfView;
			Aspect = CameraToTrack.GetComponent<Camera>().aspect;
		}
		if(CameraToTrack && Aspect == 0)
			Aspect = CameraToTrack.GetComponent<Camera>().aspect;

	}
	Vector3 lastCamLoc;
	void OnRenderObject() {
		#if UNITY_EDITOR_WIN
		if(!Application.isPlaying) 
		{
			if(SceneView.currentDrawingSceneView)
			{//Transform SceneCam = SceneView.currentDrawingSceneView.camera.transform;
				cameraLoc =  SceneView.currentDrawingSceneView.camera.transform.position;
				lastCamLoc = cameraLoc;
				FOV =  SceneView.currentDrawingSceneView.camera.fieldOfView;
				cameraTransform = SceneView.currentDrawingSceneView.camera.transform;
				Aspect = SceneView.currentDrawingSceneView.camera.aspect;
			}
			else {
				cameraLoc = lastCamLoc;
			}
			if(DebugReport)
				Debug.Log ("The currently tracked Camera is located at: " + cameraLoc);
		}
		#endif
		if(Application.isPlaying) {
			if(!CameraToTrack){
				CameraToTrack = GameObject.FindGameObjectWithTag(RuntimeCamTag);
				if(CameraToTrack)
				{
					FOV = CameraToTrack.GetComponent<Camera>().fieldOfView;
					Aspect = CameraToTrack.GetComponent<Camera>().aspect;
				}
			}

			if(CameraToTrack)
				cameraLoc = CameraToTrack.transform.position;
			if(!CameraToTrack)
				cameraLoc = Vector3.zero;
			if(!cameraTransform && CameraToTrack)
				cameraTransform = CameraToTrack.transform;
			if(CameraToTrack && Time.timeSinceLevelLoad < 2)
				Aspect = CameraToTrack.GetComponent<Camera>().aspect;
			if(DebugReport)
				Debug.Log ("The currently tracked Camera is located at: " + cameraLoc);
		}

		frameCount ++;
	}
}
