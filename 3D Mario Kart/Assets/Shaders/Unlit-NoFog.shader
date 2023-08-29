﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Unlit/Unlit No Fog" {
Properties {
   _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
   _AdvFog ("Scene Fog", 2D) = "white" {}
}
 
SubShader {
   Tags {"RenderType"="Opaque" "IgnoreProjector"="True"}
   LOD 100

   Pass {
     CGPROGRAM
       #pragma vertex vert
       #pragma fragment frag
     
       #include "UnityCG.cginc"
 
       struct appdata_t {
         float4 vertex : POSITION;
         float2 texcoord : TEXCOORD0;
       };
 
       struct v2f {
         float4 vertex : SV_POSITION;
         half2 texcoord : TEXCOORD0;
       };
 
       sampler2D _MainTex;
       float4 _MainTex_ST;
     
       v2f vert (appdata_t v)
       {
         v2f o;
         o.vertex = UnityObjectToClipPos(v.vertex);
         o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
         return o;
       }
     
       fixed4 frag (v2f i) : COLOR
       {
         fixed4 col = tex2D(_MainTex, i.texcoord);
         return col;
       }
     ENDCG
   }
}
}