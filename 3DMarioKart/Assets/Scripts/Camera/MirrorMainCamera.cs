using UnityEngine;
#if UNITY_EDITOR_WIN
using UnityEditor;
#endif
using System.Collections;
[ExecuteInEditMode]
public class MirrorMainCamera : MonoBehaviour {
	
	public static Texture2D Lighting;
	
	// Update is called once per frame
	
	void Initialize()
	{
		
	}
	
	void OnRenderObject() {
		if(CameraLocationTracker.cameraTransform != null)
			gameObject.transform.rotation = CameraLocationTracker.cameraTransform.rotation;
		
		
	}
	
}
