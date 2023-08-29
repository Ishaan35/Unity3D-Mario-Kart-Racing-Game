Shader "Hidden/Kronnect/Beautify/TextureLerp"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("LUT 1", 2D) = "black" {}
        [NoScaleOffset] _LUT2 ("LUT 2", 2D) = "black" {}
        _Phase("Phase", Float) = 0
    }
    SubShader
    {
        ZTest Always
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _LUT2;
            float _Phase;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 lut1 = tex2D(_MainTex, i.uv);
                half4 lut2 = tex2D(_LUT2, i.uv);
                return lerp(lut1, lut2, _Phase);
            }
            ENDCG
        }
    }
}
