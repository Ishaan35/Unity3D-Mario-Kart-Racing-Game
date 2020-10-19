using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Rotator : MonoBehaviour {
	public float Rate = .25f;
	public bool rotateVertically = true;
	public bool rotateinEditor = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Application.isPlaying){
			if(rotateVertically)
				gameObject.transform.RotateAround (transform.position,transform.up,Rate*Time.deltaTime*60);
			else
				gameObject.transform.RotateAround (transform.position,transform.forward,Rate*Time.deltaTime*60);

		}
	}
	void OnRenderObject(){
		if(!Application.isPlaying && rotateinEditor){
			if(rotateVertically)
				gameObject.transform.RotateAround (transform.position,transform.up,Rate*Time.deltaTime*60);
			else
				gameObject.transform.RotateAround (transform.position,transform.forward,Rate*Time.deltaTime*60);
		}
	}

}
