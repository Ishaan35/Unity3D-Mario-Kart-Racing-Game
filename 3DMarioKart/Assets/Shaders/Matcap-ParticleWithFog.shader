// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "MatCap/Particles/Alpha Blended With Fog" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Lighting("MatCap Lit", 2D) = "white" {}
	_AdvFog ("Scene Fog", 2D) = "white" {}
	_PreFog ("Pre Fog", float) = 0
}
 
SubShader {
   Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
   LOD 100
      Pass {


	   Blend SrcAlpha OneMinusSrcAlpha  
	   ZWrite Off
	   Offset -10,-10 
         CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members angleBlend)
       #pragma vertex vert
       #pragma fragment frag
       #pragma multi_compile_fog
       #pragma multi_compile ADVFOG_ON ADVFOG_OFF
       #include "UnityCG.cginc"
 
       struct appdata_t {
			fixed2 texcoord : TEXCOORD0;
			fixed4 vertex : POSITION;
			fixed4 normal : NORMAL;
			fixed4 color: COLOR;
			fixed4 tangent : TANGENT;
       };
 
       struct v2f {
         float4 vertex : SV_POSITION;
         fixed2 texcoord : TEXCOORD0;
         fixed2 cap : TEXCOORD3;
         fixed4 scrPos : TEXCOORD2;
         fixed4 color: COLOR;
         UNITY_FOG_COORDS(1)
         fixed3 texcoord1 : TEXCOORD4;
       };
       
		uniform fixed4 _MainTex_ST;
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.texcoord1.xy = TRANSFORM_TEX(v.tangent,_MainTex); 
			o.texcoord1.z = v.tangent.z;
			fixed2 capCoord;					
			fixed3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
			worldNorm = mul((fixed3x3)UNITY_MATRIX_V, worldNorm);
			o.cap.xy = worldNorm.xy * 0.5 + 0.5;
			o.scrPos = ComputeScreenPos(o.vertex);
			UNITY_TRANSFER_FOG(o,o.vertex);         
			o.color = v.color;
			return o;
		}   
		sampler2D _MainTex; 
		sampler2D _Lighting;
		fixed _PreFog;
		#if ADVFOG_ON
		uniform sampler2D _AdvFog;
		#endif		     
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 mc = tex2D(_Lighting, i.cap);
			mc.a = 1;
			fixed4 col = tex2D(_MainTex, i.texcoord.xy);
			fixed4 col2 = tex2D(_MainTex, i.texcoord1.xy);
			col = lerp(col, col2, i.texcoord1.z);
			col.rgb *= i.color*mc*3;
			col.a *= i.color.a;
			#if ADVFOG_ON
			fixed4 advFog = tex2D(_AdvFog,i.scrPos.xy/i.scrPos.w);	
			col.rgb = col.rgb+(advFog*_PreFog);		
			#endif			
			#if ADVFOG_ON
			advFog.rgb *= .75f;
			UNITY_APPLY_FOG_COLOR(i.fogCoord, col, advFog);
			#endif
			#if ADVFOG_OFF
			UNITY_APPLY_FOG_COLOR(i.fogCoord, col, UNITY_LIGHTMODEL_AMBIENT);
			#endif
		 	return col;
		}
     ENDCG
   }

   
   
   
   
   
}
}