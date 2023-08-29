#ifndef BEAUTIFY_PPSEA_FX
#define BEAUTIFY_PPSEA_FX

	// Copyright 2020 Kronnect - All Rights Reserved.
    #include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D_X(_EALumSrc);
    TEXTURE2D_X(_EAHist);
	float4    _MainTex_TexelSize;
	float4    _MainTex_ST;
	float4    _EyeAdaptation;

	struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};

   	VaryingsCross VertCross(Attributes v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
        o.uv = v.uv;

        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o)
		return o;
	}

    float4 FragScreenLum (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        #if UNITY_COLORSPACE_GAMMA
            c.rgb = GAMMA_TO_LINEAR(c.rgb);
        #endif
        c.r = log(1.0 + getLuma(c.rgb));
        return c.rrrr;
    }  
    
    float4 FragReduceScreenLum (VaryingsCross i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i);

        float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
        float4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
        float4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
        float4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);
        c1.g = max( c1.g, max( c2.g, max( c3.g, c4.g )));
        c1.r = (c1.r + c2.r + c3.r + c4.r) * 0.25;
        return c1;
    }       

    float4 FragBlendScreenLum (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv = UnityStereoTransformScreenSpaceTex(float2(0.5, 0.5));

        float4 c     = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv);
        float4 p     = SAMPLE_TEXTURE2D_X(_EAHist, sampler_LinearClamp, uv);
        float speed  = c.r < p.r ? _EyeAdaptation.z: _EyeAdaptation.w;
        c.a = speed * unity_DeltaTime.x;
        return c;
    }  
    
    float4 FragBlend (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, 0.5.xx);
        c.a = 1.0;
        return c;
    }  

#endif