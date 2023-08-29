using UnityEngine;
using System.Collections;

public class AspectRatioFix : MonoBehaviour {
	public float initialAspect = 1.5f; 
	Vector3 StartLocation;
	float XPos;

	// Use this for initialization
	void Start () {
		StartLocation = gameObject.transform.localPosition;
		XPos = StartLocation.x/initialAspect;
		XPos *= CameraLocationTracker.Aspect*1.05f;
		gameObject.transform.localPosition = new Vector3(XPos,StartLocation.y,StartLocation.z);
		Debug.Log (CameraLocationTracker.Aspect);
	}
	void Update()
	{
		if(Time.timeSinceLevelLoad < 2)
		{
			XPos = StartLocation.x/initialAspect;
			XPos *= CameraLocationTracker.Aspect*1.05f;
			gameObject.transform.localPosition = new Vector3(XPos,StartLocation.y,StartLocation.z);
		}

	}

}
