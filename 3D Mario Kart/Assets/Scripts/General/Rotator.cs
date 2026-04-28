using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Rotator : MonoBehaviour {
	[SerialiseField] private float Rate = .25f;
	[SerialiseField] private bool rotateVertically = true;
	[SerialiseField] private bool rotateinEditor = false;
	// Use this for initialization
	
	
	// Update is called once per frame
	private void Update () {

		if(Application.isPlaying){
			if(rotateVertically)
				gameObject.transform.RotateAround (transform.position,transform.up,Rate*Time.deltaTime*60);
			else
				gameObject.transform.RotateAround (transform.position,transform.forward,Rate*Time.deltaTime*60);

		}
	}
	public void OnRenderObject(){
		if(!Application.isPlaying && rotateinEditor){
			if(rotateVertically)
				gameObject.transform.RotateAround (transform.position,transform.up,Rate*Time.deltaTime*60);
			else
				gameObject.transform.RotateAround (transform.position,transform.forward,Rate*Time.deltaTime*60);
		}
	}

}
