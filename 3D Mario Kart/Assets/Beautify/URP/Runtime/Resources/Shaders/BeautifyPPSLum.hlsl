#ifndef BEAUTIFY_PPSLUM_FX
#define BEAUTIFY_PPSLUM_FX

	// Copyright 2020 Kronnect - All Rights Reserved.
    #include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D_X(_BloomTex);
	float4 	  _BloomTex_TexelSize;
    TEXTURE2D_X(_CombineTex);
	TEXTURE2D_X(_BloomTex1);
	TEXTURE2D_X(_BloomTex2);
	TEXTURE2D_X(_BloomTex3);
	TEXTURE2D_X(_BloomTex4);
	float4    _MainTex_TexelSize;
	float4    _MainTex_ST;
    float4 	  _Bloom;
	float4 	  _BloomWeights;
	float4 	  _BloomWeights2;
	float4    _BloomTint;
    float4 	  _AFTint;
	float     _BlurScale;
    float3    _AFData;

	#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
	    float      _BloomDepthThreshold;
        float      _AFDepthThreshold;
	#endif

	#if BEAUTIFY_BLOOM_USE_LAYER
	    TEXTURE2D_X_FLOAT(_BloomSourceDepth);
		float _BloomLayerZBias;
	#endif


	struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};

	struct VaryingsLum {
		float4 positionCS : SV_POSITION;
		float2 uv: TEXCOORD0;

        UNITY_VERTEX_OUTPUT_STEREO
	};

	struct VaryingsCrossLum {
		float4 positionCS : SV_POSITION;
		float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA

        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsLum VertLum(Attributes input) {

	    VaryingsLum output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;

        return output;
	}

	inline float Brightness(float3 c) {
		return max(c.r, max(c.g, c.b));
	}

	float4 FragLum (VaryingsLum i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

	    float4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
		c = clamp(c, 0.0.xxxx, _BloomWeights2.zzzz);
   		#if UNITY_COLORSPACE_GAMMA
		    c.rgb = GAMMA_TO_LINEAR(c.rgb);
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		    float depth01 = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH
		    c.rgb *= (1.0 - depth01 * _BloomDepthThreshold);
        #elif BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
            c.rgb *= (1.0 - depth01 * _AFDepthThreshold);
		#endif
        
		#if BEAUTIFY_BLOOM_USE_LAYER
			float depth02 = BEAUTIFY_GET_CUSTOM_DEPTH_01(_BloomSourceDepth, i.uv);
			float isTransparent = (depth02 >= 1) && any(c.rgb>0);
			float nonEclipsed = isTransparent || (depth01 > depth02 - _BloomLayerZBias);
			c.rgb *= nonEclipsed;
		#endif
		c.a = Brightness(c.rgb);
        #if defined(USE_AF_THRESHOLD)
            c.rgb = max(c.rgb - _AFData.yyy, 0);
        #else
		    c.rgb = max(c.rgb - _Bloom.www, 0);
        #endif
   		return c;
   	}

   	VaryingsCross VertCross(Attributes v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
        o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o)

		return o;
	}

	VaryingsCrossLum VertCrossLum(Attributes v) {
		VaryingsCrossLum o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
        o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o)

		return o;
	}

   	float4 FragLumAntiflicker(VaryingsCrossLum i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		float4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		float4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		float4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);

		c1 = clamp(c1, 0.0.xxxx, _BloomWeights2.zzzz);
		c2 = clamp(c2, 0.0.xxxx, _BloomWeights2.zzzz);
		c3 = clamp(c3, 0.0.xxxx, _BloomWeights2.zzzz);
		c4 = clamp(c4, 0.0.xxxx, _BloomWeights2.zzzz);

		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		    float depth01 = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
		#endif
        
		#if BEAUTIFY_BLOOM_USE_DEPTH
            float depthAtten = 1.0 - depth01 * _BloomDepthThreshold;
		    c1.rgb *= depthAtten;
		    c2.rgb *= depthAtten;
		    c3.rgb *= depthAtten;
		    c4.rgb *= depthAtten;
        #elif BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
            float depthAtten = 1.0 - depth01 * _AFDepthThreshold;
            c1.rgb *= depthAtten;
            c2.rgb *= depthAtten;
            c3.rgb *= depthAtten;
            c4.rgb *= depthAtten;
        #endif
                        
		#if BEAUTIFY_BLOOM_USE_LAYER
	        float depth02 = BEAUTIFY_GET_CUSTOM_DEPTH_01(_BloomSourceDepth, i.uv);
		    float isTransparent = (depth02 >= 1) && any(c1.rgb>0);
		    float nonEclipsed = isTransparent || (depth01 > depth02 - _BloomLayerZBias );
		    c1.rgb *= nonEclipsed;
		    c2.rgb *= nonEclipsed;
		    c3.rgb *= nonEclipsed;
		    c4.rgb *= nonEclipsed;
		#endif
		
		c1.a = Brightness(c1.rgb);
		c2.a = Brightness(c2.rgb);
		c3.a = Brightness(c3.rgb);
		c4.a = Brightness(c4.rgb);
	    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);

	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    c1 = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        
   		#if UNITY_COLORSPACE_GAMMA
		    c1.rgb = GAMMA_TO_LINEAR(c1.rgb);
		#endif

        #if defined(USE_AF_THRESHOLD)
            c1.rgb = max(c1.rgb - _AFData.yyy, 0);
        #else
            c1.rgb = max(c1.rgb - _Bloom.www, 0);
        #endif

   		return c1;
	}

	float4 FragBloomCompose (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		float4 b0 = SAMPLE_TEXTURE2D_X( _BloomTex  , sampler_LinearClamp, i.uv );
		float4 b1 = SAMPLE_TEXTURE2D_X( _BloomTex1 , sampler_LinearClamp, i.uv );
		float4 b2 = SAMPLE_TEXTURE2D_X( _BloomTex2 , sampler_LinearClamp, i.uv );
		float4 b3 = SAMPLE_TEXTURE2D_X( _BloomTex3 , sampler_LinearClamp, i.uv );
		float4 b4 = SAMPLE_TEXTURE2D_X( _BloomTex4 , sampler_LinearClamp, i.uv );
		float4 b5 = SAMPLE_TEXTURE2D_X( _MainTex   , sampler_LinearClamp, i.uv );
		float4 pixel = b0 * _BloomWeights.x + b1 * _BloomWeights.y + b2 * _BloomWeights.z + b3 * _BloomWeights.w + b4 * _BloomWeights2.x + b5 * _BloomWeights2.y;
		pixel.rgb = lerp(pixel.rgb, Brightness(pixel.rgb) * _BloomTint.rgb, _BloomTint.a);
		return pixel;
	}

	float4 FragResample(VaryingsCross i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		float4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		float4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		float4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);
			    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);
	    
	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    float4 v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        #if defined(COMBINE_BLOOM)
            float4 o = SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv);
            v += o;
        #endif
        return v;
	}

	float4 FragResampleAF(VaryingsCross i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		float4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		float4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		float4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);
			    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);
	    
	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    float4 v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
	    v.rgb = lerp(v.rgb, Brightness(c1.rgb) * _AFTint.rgb, _AFTint.a);
	    v.rgb *= _AFData.xxx;
        
        #if defined(COMBINE_BLOOM)
            float4 o = SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv);
            v += o;
        #endif
        
	    return v;
	}

	
    float4 FragCombine(Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        float4 c2 = SAMPLE_TEXTURE2D_X(_CombineTex, sampler_LinearClamp, i.uv);
        return c1 + c2;
    }
    
    
	float4 FragDebugBloom (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		return SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv) * _Bloom.xxxx;
	}
	
	float4 FragResampleFastAF(Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		float4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
	    c.rgb = lerp(c.rgb, Brightness(c.rgb) * _AFTint.rgb, _AFTint.a);
	    c.rgb *= _AFData.xxx;
	    return c;
	}
	
	VaryingsCross VertBlur(Attributes v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
    	o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(o)

    	return o;
	}
	
	float4 FragBlur (VaryingsCross i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i)

		float4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv) * 0.2270270270
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2)) * 0.3162162162
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4)) * 0.0702702703;
   		return pixel;
	}	

#endif