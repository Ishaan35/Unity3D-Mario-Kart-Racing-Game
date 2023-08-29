using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SetRes : MonoBehaviour {
	public static int pixelScale = 1;
	public static RenderTexture SceneBuffer;
	// Use this for initialization
	void Start () {
		//Debug.Log (Screen.height);
		pixelScale = 2;
		if(Screen.height > 1000)
			pixelScale = 3;
		if(Screen.height > 1400)
			pixelScale = 4;
		//Debug.Log (pixelScale);
		SceneBuffer = new RenderTexture(Screen.width/pixelScale,Screen.height/pixelScale,24);
		SceneBuffer.filterMode = FilterMode.Point;
		SceneBuffer.depth = 24;
		SceneBuffer.autoGenerateMips = false;
		SceneBuffer.antiAliasing = 1;
		SceneBuffer.name = "SceneBuffer";
		GetComponent<Camera>().targetTexture = SceneBuffer;
		GameObject.FindGameObjectWithTag ("UICamera").GetComponent<Camera>().targetTexture = SceneBuffer;

	}
	
	// Update is called once per frame
	void Update () {
		if(!GetComponent<Camera>().targetTexture)
		{	
			GetComponent<Camera>().targetTexture = SceneBuffer;
			GameObject.FindGameObjectWithTag ("UICamera").GetComponent<Camera>().targetTexture = SceneBuffer;

		}
	}
}
