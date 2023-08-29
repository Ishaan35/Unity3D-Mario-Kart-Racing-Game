using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class LightingControl : MonoBehaviour {
	public bool dynamicPerfAdjust = false;
	public bool Shadows = false;
	public bool AdvancedAmbient = true;
	public bool AdvancedFog = true;
	public Texture2D LitProbe;
	public Texture2D AmbientProbe;
	public Texture2D EdgeLitProbe;
	public Cubemap SkyBoxMap;
	public static bool EnableShadows = true;
	public static bool EnableAdvancedAmbient = true;
	public static bool EnableAdvancedFog = false;
	public GameObject mainSceneLight;
	public Material[] modifiedMaterials;
	public Material LitMat;
	public Material AmbientMat;
	public Material EdgeLitMat;
	public Material SkyboxMat;
	//#pragma multi_compile SHADOW_ON SHADOW_OFF
	//#pragma multi_compile ADVAMBIENT_ON ADVAMBIENT_OFF  
	void UpdateLighting() {
		if(Application.isMobilePlatform){
			mainSceneLight.GetComponent<Light>().shadowBias = .125f;
			//EnableShadows = false;
			if(Screen.height > 800)
				Screen.SetResolution (Screen.width/2,Screen.height/2,true);
			QualitySettings.shadowCascades = 1;
			QualitySettings.shadowDistance = 100;
		}
		LitMat.mainTexture = LitProbe;
		AmbientMat.mainTexture = AmbientProbe;
		EdgeLitMat.mainTexture = EdgeLitProbe;
		SkyboxMat.SetTexture ("_Tex",SkyBoxMap);

		if(EnableShadows)
		{
			foreach(Material mat in modifiedMaterials){
				if(mat)
					mat.EnableKeyword("SHADOW_ON");
			}
			mainSceneLight.SetActive (true);
		}
		else
		{
			foreach(Material mat in modifiedMaterials){
				if(mat)
					mat.DisableKeyword("SHADOW_ON");
			}
			mainSceneLight.SetActive (false);
		}
		if(EnableAdvancedAmbient)
		{
			foreach(Material mat in modifiedMaterials){
				if(mat)
					mat.EnableKeyword("ADVAMBIENT_ON");
			}
		}
		else
		{
			foreach(Material mat in modifiedMaterials){
				if(mat)
					mat.DisableKeyword("ADVAMBIENT_ON");
			}
		}
		if(AdvancedFog)
		{
			foreach(Material mat in modifiedMaterials){
				if(mat){
					mat.EnableKeyword("ADVFOG_ON");
					mat.DisableKeyword("ADVFOG_OFF");
				}
			}
			
		}
		else
		{
			foreach(Material mat in modifiedMaterials){
				if(mat){
					mat.DisableKeyword("ADVFOG_ON");
					mat.EnableKeyword("ADVFOG_OFF");
				}
			}
		}
	}
	void Start(){
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = 30;
		UpdateLighting();
	}

	void OnRenderObject() {
		#if UNITY_EDITOR_WIN
		EnableShadows = Shadows;
		EnableAdvancedAmbient = AdvancedAmbient;
		EnableAdvancedFog = AdvancedFog;
		UpdateLighting();
		#endif
		//Debug.Log("The Advanced Fog is "+ defaultLit.IsKeywordEnabled("ADVFOG_ON"));
	}

}
