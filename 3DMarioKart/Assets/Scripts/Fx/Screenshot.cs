using UnityEngine;
using System.Collections;

public class Screenshot : MonoBehaviour {
	void Update() {
		if(Input.GetKeyDown(KeyCode.F9))
		   {
			ScreenCapture.CaptureScreenshot(Time.realtimeSinceStartup + "_Screenshot.png");
		}
	}
}