// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "MatCap/Particles/Edged Alpha Blended With Fog" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_EdgeIntensity ("Edge Intensity", float) = 1
	_PreFog ("Pre Fog", float) = 0
	_Lighting("MatCap Lit", 2D) = "white" {}
	_EdgeLit ("Edge Lit", 2D) = "white" {}	
	_AdvFog ("Scene Fog", 2D) = "white" {}
}
 
SubShader {
   Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
   LOD 100
Pass {
   Blend  OneMinusDstColor One, One Zero
   ZWrite Off
         CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members angleBlend)
       #pragma vertex vert
       #pragma fragment frag
       #include "UnityCG.cginc"
 
       struct appdata_t {
			fixed2 texcoord : TEXCOORD0;
			fixed4 vertex : POSITION;
			fixed4 normal : NORMAL;
			fixed4 color: COLOR;
       };
 
       struct v2f {
         float4 vertex : SV_POSITION;
         fixed2 texcoord : TEXCOORD0;
         fixed2 cap : TEXCOORD3;
         fixed4 scrPos : TEXCOORD2;
         fixed4 color: COLOR;
         fixed3 texcoord1 : TEXCOORD4;
       };
       
		uniform fixed4 _MainTex_ST;
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			fixed2 capCoord;					
			fixed3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
			worldNorm = mul((fixed3x3)UNITY_MATRIX_V, worldNorm);
			o.cap.xy = worldNorm.xy * 0.5 + 0.5;
			o.scrPos = ComputeScreenPos(o.vertex);
			o.color = v.color;
			return o;
		}   
		sampler2D _MainTex; 
		sampler2D _Lighting;
		sampler2D _EdgeLit;
		fixed _EdgeIntensity; 
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 col = tex2D(_MainTex, i.texcoord.xy);
			col.rgb *= 0;
			fixed innercol = (1-col.a)*_EdgeIntensity;
			col.a *= i.color.a;
			fixed4 edgeLit = tex2D(_EdgeLit,i.scrPos.xy/i.scrPos.w);	
			col.rgb = (edgeLit)*col.a*_EdgeIntensity;

		 	return col;
		}
     ENDCG
   }   
   Pass {
   Blend SrcAlpha OneMinusSrcAlpha  
     ZWrite Off
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
		sampler2D _EdgeLit;
		#if ADVFOG_ON
		uniform sampler2D _AdvFog;
		#endif		     
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 mc = tex2D(_Lighting, i.cap*1.2);
			mc.a = 1;
			fixed4 col = tex2D(_MainTex, i.texcoord.xy);
			col.rgb *= i.color*(mc*2);
			fixed4 edgeLit = tex2D(_EdgeLit,i.scrPos.xy/i.scrPos.w);
			col.a *= i.color.a;
			#if ADVFOG_ON
			fixed4 advFog = tex2D(_AdvFog,i.scrPos.xy/i.scrPos.w);	
			col.rgb = lerp(col.rgb,(advFog-.25f),edgeLit.r*2);

			#endif			
			#if ADVFOG_ON
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