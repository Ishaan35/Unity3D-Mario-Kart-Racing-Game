#ifndef BEAUTIFY_CORE_FX
#define BEAUTIFY_CORE_FX

	// Copyright 2020 Ramiro Oliva (Kronnect) - All Rights Reserved.

    #include "BeautifyACESFitted.hlsl"
    #include "BeautifyColorTemp.hlsl"
    #include "BeautifyCommon.hlsl"

    float4 _CompareParams;
    TEXTURE2D_X(_CompareTex);
    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X(_BloomTex);
    TEXTURE2D_X(_ScreenLum);
    TEXTURE2D(_OverlayTex);
    TEXTURE2D(_LUTTex);
    TEXTURE2D_X(_EAHist);
    TEXTURE2D_X(_EALumSrc);
    TEXTURE2D(_BlueNoise);
    float4 _BlueNoise_TexelSize;

    SAMPLER(sampler_MainTex);
    float4 _MainTex_TexelSize;
	float4 _Params;
	float4 _Sharpen;
	float4 _Bloom;
    float4 _Dirt;    // x = brightness based, y = intensity, z = threshold, w = bloom contribution    
    float4 _FXColor;
    float4 _ColorBoost;
    float4 _TintColor;
    float4 _Purkinje;
    float4 _EyeAdaptation;
    float4 _BokehData;
    float4 _BokehData2;
    float4 _Outline;
    float3 _ColorTemp;
    float4 _NightVision;
    float4 _LUTTex_TexelSize;

    #if BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK
        float4 _Vignetting;
        float _VignettingAspectRatio;
        TEXTURE2D_X(_VignettingMask);
    #endif

    #if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT || BEAUTIFY_DEPTH_OF_FIELD
        TEXTURE2D_X(_DoFTex);
        float4 _DoFTex_TexelSize;
    #endif
    //#if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
    //    TEXTURE2D_X(_DepthTexture);
    //    TEXTURE2D_X(_DofExclusionTexture;
    //#endif

 	struct VaryingsBeautify {
    	float4 positionCS : SV_POSITION;
    	float2 uv  : TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsBeautify VertBeautify(Attributes input) {
	    VaryingsBeautify output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;
        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(output)
    	return output;
	}


    float3 getNormal(float depth, float depth1, float depth2, float2 offset1, float2 offset2) {
        float3 p1 = float3(offset1, depth1 - depth);
        float3 p2 = float3(offset2, depth2 - depth);
        float3 normal = cross(p1, p2);
        return normalize(normal);
    }

	float getRandom(float2 uv) {
		return frac(sin(_Time.y + dot(uv, float2(12.9898, 78.233)))* 43758.5453);
	}


    float getCoc(VaryingsBeautify i) {
    #if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
        float depthTex = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DepthTexture, sampler_DepthTexture, i.uv, 0));
        float exclusionDepth = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DofExclusionTexture, sampler_DoFExclusionTexture, i.uv, 0));
        float depth  = BEAUTIFY_GET_SCENE_DEPTH_01(i.depthUV);
        depth = min(depth, depthTex);
        if (exclusionDepth < depth) return 0;
        depth *= _ProjectionParams.z;
    #else
        float depth  = BEAUTIFY_GET_SCENE_DEPTH_EYE(i.uv);
    #endif
        float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
        return 0.5 * _BokehData.y * xd/depth;   // radius of CoC
    }

	void beautifyPass(VaryingsBeautify i, inout float3 rgbM) {

        float2 uv = i.uv;
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float  depthS     = BEAUTIFY_GET_SCENE_DEPTH_01(uvS);
		float  depthW     = BEAUTIFY_GET_SCENE_DEPTH_01(uvW);
		float  depthE     = BEAUTIFY_GET_SCENE_DEPTH_01(uvE);		
		float  depthN     = BEAUTIFY_GET_SCENE_DEPTH_01(uvN);
		float  lumaM      = getLuma(rgbM);

		// daltonize
        #if BEAUTIFY_COLOR_TWEAKS
		float3 rgb0       = 1.0.xxx - saturate(rgbM.rgb);
		       rgbM.r    *= 1.0 + rgbM.r * rgb0.g * rgb0.b * _Params.y;
			   rgbM.g    *= 1.0 + rgbM.g * rgb0.r * rgb0.b * _Params.y;
			   rgbM.b    *= 1.0 + rgbM.b * rgb0.r * rgb0.g * _Params.y;	
			   rgbM      *= lumaM / (getLuma(rgbM) + 0.0001);
        #endif

		// sharpen
		float  maxDepth   = max(depthN, depthS);
		       maxDepth   = max(maxDepth, depthW);
		       maxDepth   = max(maxDepth, depthE);
		float  minDepth   = min(depthN, depthS);
		       minDepth   = min(minDepth, depthW);
		       minDepth   = min(minDepth, depthE);
		float  dDepth     = maxDepth - minDepth + 0.00001;

#if BEAUTIFY_TURBO
        const float  lumaDepth  = 1.0;
#else
		float  lumaDepth  = saturate( _Sharpen.y / dDepth);
#endif

		float3 rgbS       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvS).rgb;
	    float3 rgbW       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvW).rgb;
	    float3 rgbE       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvE).rgb;
	    float3 rgbN       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvN).rgb;
	    
    	float  lumaN      = getLuma(rgbN);
    	float  lumaE      = getLuma(rgbE);
    	float  lumaW      = getLuma(rgbW);
    	float  lumaS      = getLuma(rgbS);
		
    	float  maxLuma    = max(lumaN,lumaS);
    	       maxLuma    = max(maxLuma, lumaW);
#if !BEAUTIFY_TURBO
    	       maxLuma    = max(maxLuma, lumaE);
#endif
	    float  minLuma    = min(lumaN,lumaS);
	           minLuma    = min(minLuma, lumaW);
#if !BEAUTIFY_TURBO
	           minLuma    = min(minLuma, lumaE);
#endif
               minLuma   -= 0.000001;
	    float  lumaPower  = 2.0 * lumaM - minLuma - maxLuma;
		float  lumaAtten  = saturate(_Sharpen.w / (maxLuma - minLuma));
#if BEAUTIFY_TURBO
        const float depthClamp = 1.0;
#else
		float  depthClamp = abs(depthW - _Params.z) < _Params.w;
#endif

#if BEAUTIFY_SHARPEN
		       rgbM      *= 1.0 + clamp(lumaPower * lumaAtten * lumaDepth * _Sharpen.x, -_Sharpen.z, _Sharpen.z) * depthClamp;
#endif

        #if BEAUTIFY_DEPTH_OF_FIELD || BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
            float4 dofPix     = SAMPLE_TEXTURE2D_X(_DoFTex, sampler_LinearClamp, uv);
            #if UNITY_COLORSPACE_GAMMA
               dofPix.rgb = LINEAR_TO_GAMMA(dofPix.rgb);
            #endif
            if (_DoFTex_TexelSize.z < _MainTex_TexelSize.z) {
                float  CoC = getCoc(i) / 16;
                dofPix.a   = lerp(CoC, dofPix.a, _DoFTex_TexelSize.z / _MainTex_TexelSize.z);
            }
            rgbM = lerp(rgbM, dofPix.rgb, saturate(dofPix.a * 16)); 
        #endif

        #if BEAUTIFY_NIGHT_VISION || (BEAUTIFY_OUTLINE && !defined(BEAUTIFY_OUTLINE_SOBEL))
		    float3 uvNormalDisp    = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
                 float depth       = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
                 float3 normalNW   = getNormal(depth, depthN, depthW, uvNormalDisp.zy, float2(-uvNormalDisp.x, -uvNormalDisp.z));
        #endif

        #if BEAUTIFY_OUTLINE
            #if !defined(BEAUTIFY_OUTLINE_SOBEL)
                float3 normalSE   = getNormal(depth, depthS, depthE, -uvNormalDisp.zy,  uvNormalDisp.xz);
                float  dnorm      = dot(normalNW, normalSE);
                rgbM              = lerp(rgbM, _Outline.rgb, (float)(dnorm  < _Outline.a));
            #else
                float4 uv4 = float4(uv, 0, 0);
                #if BEAUTIFY_NIGHT_VISION || BEAUTIFY_THERMAL_VISION
                    float3 rgbS       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.zyzz).rgb;
                    float3 rgbN       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.zyzz).rgb;
                    float3 rgbW       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xzzz).rgb;
                    float3 rgbE       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xzzz).rgb;
                #endif
                float3 rgbSW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xyzz).rgb;    // was tex2Dlod
                float3 rgbNE = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xyzz).rgb;
                float3 rgbSE = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + float4( uvInc.x, -uvInc.y, 0, 0)).rgb;
                float3 rgbNW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + float4(-uvInc.x,  uvInc.y, 0, 0)).rgb;
                float3 gx  = rgbSW * -1.0;
                       gx += rgbSE *  1.0;
                       gx += rgbW  * -2.0;
                       gx += rgbE  *  2.0;
                       gx += rgbNW * -1.0;
                       gx += rgbNE *  1.0;
                float3 gy  = rgbSW * -1.0;
                       gy += rgbS  * -2.0;
                       gy += rgbSE * -1.0;
                       gy += rgbNW *  1.0;
                       gy += rgbN  *  2.0;
                       gy += rgbNE *  1.0;
                float olColor = (length(gx * gx + gy * gy) - _Outline.a) > 0.0;
                rgbM = lerp(rgbM, _Outline.rgb, olColor); 
            #endif
        #endif

		#if UNITY_COLORSPACE_GAMMA && (BEAUTIFY_BLOOM || BEAUTIFY_NIGHT_VISION || BEAUTIFY_DIRT || BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE || BEAUTIFY_TONEMAP_ACES)
	    	rgbM = GAMMA_TO_LINEAR(rgbM);
		#endif

		#if BEAUTIFY_BLOOM
    		rgbM += SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, uv).rgb * _Bloom.xxx;
		#endif

        #if BEAUTIFY_DIRT
            float3 scrLum = SAMPLE_TEXTURE2D_X(_ScreenLum, sampler_LinearClamp, uv).rgb;
            #if BEAUTIFY_BLOOM
                scrLum *= _Dirt.www;
            #endif

            #if defined(UNITY_SINGLE_PASS_STEREO)
                float2 dirtUV = float2(uv.x * 2.0, uv.y); // TODO: ?? should ignore Single Pass
            #else
                float2 dirtUV = uv;
            #endif
            float4 dirt = SAMPLE_TEXTURE2D(_OverlayTex, sampler_LinearRepeat, dirtUV);
            rgbM       += saturate(0.5.xxx - _Dirt.zzz + scrLum) * dirt.rgb * _Dirt.y;
        #endif

        #if BEAUTIFY_NIGHT_VISION
   	 	       lumaM      = getLuma(rgbM);	// updates luma
   		float  nvbase     = saturate(normalNW.z - 0.8); // minimum ambient self radiance (useful for pitch black)
   			   nvbase    += lumaM;						// adds current lighting
   			   nvbase    *= nvbase * (0.5 + nvbase);	// increase contrast
   			   rgbM	      = nvbase * _NightVision.rgb;
   			   rgbM      *= frac(floor(uv.y * _MainTex_TexelSize.w)*0.25)>0.4;	// scan lines
   			   rgbM	     *= 1.0 + getRandom(uv) * 0.3 - 0.15;				// noise
	 	#endif


        #if BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE
            float4 avgLum = SAMPLE_TEXTURE2D_X(_EAHist, sampler_LinearClamp, 0.5.xx);
        #endif

        #if BEAUTIFY_EYE_ADAPTATION
            float srcLum  = SAMPLE_TEXTURE2D_X(_EALumSrc, sampler_LinearClamp, 0.5.xx).r;
            float  diff   = srcLum / (avgLum.r + 0.0001);
            float pixLum  = max(0,log(1.0 + getLuma(rgbM)));
            diff   = pow(pixLum / (avgLum.r + 0.0001), abs(diff-1.0));
            diff   = clamp(diff, _EyeAdaptation.x, _EyeAdaptation.y);
            rgbM   = rgbM * diff;
        #endif

        #if BEAUTIFY_TONEMAP_ACES
             rgbM *= _FXColor.r;
             rgbM    = ACESFitted(rgbM);
             rgbM *= _FXColor.g;
        #endif

		#if UNITY_COLORSPACE_GAMMA && (BEAUTIFY_BLOOM || BEAUTIFY_NIGHT_VISION || BEAUTIFY_DIRT || BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE || BEAUTIFY_TONEMAP_ACES)
    		rgbM    = LINEAR_TO_GAMMA(rgbM);
		#endif

        #if BEAUTIFY_LUT
            #if !UNITY_COLORSPACE_GAMMA
            rgbM = LINEAR_TO_GAMMA(rgbM);
            #endif
        
//            const float3 lutST = float3(1.0/1024, 1.0/32, 32-1);
	    float3 lutST = float3(_LUTTex_TexelSize.x, _LUTTex_TexelSize.y, _LUTTex_TexelSize.w - 1);

            float3 lookUp = saturate(rgbM) * lutST.zzz;
            lookUp.xy = lutST.xy * (lookUp.xy + 0.5);
            float slice = floor(lookUp.z);
            lookUp.x += slice * lutST.y;
            float2 lookUpNextSlice = float2(lookUp.x + lutST.y, lookUp.y);
            float3 lut = lerp(SAMPLE_TEXTURE2D(_LUTTex, sampler_LinearClamp, lookUp.xy).rgb, SAMPLE_TEXTURE2D(_LUTTex, sampler_LinearClamp, lookUpNextSlice).rgb, lookUp.z - slice);
            rgbM = lerp(rgbM, lut, _FXColor.a);
            
            #if !UNITY_COLORSPACE_GAMMA
                rgbM = GAMMA_TO_LINEAR(rgbM);
            #endif
        #endif


 		// sepia
        #if BEAUTIFY_COLOR_TWEAKS
		float3 sepia      = float3(
   		            	   			dot(rgbM, float3(0.393, 0.769, 0.189)),
               						dot(rgbM, float3(0.349, 0.686, 0.168)),
               						dot(rgbM, float3(0.272, 0.534, 0.131))
               					  );
        rgbM      = lerp(rgbM, sepia, _Params.x);
        #endif

        // saturate
        float maxComponent = max(rgbM.r, max(rgbM.g, rgbM.b));
        float minComponent = min(rgbM.r, min(rgbM.g, rgbM.b));
        float sat = saturate(maxComponent - minComponent);
        rgbM *= 1.0 + _ColorBoost.z * (1.0 - sat) * (rgbM - getLuma(rgbM));
        rgbM = lerp(rgbM, rgbM * _TintColor.rgb, _TintColor.a);
        rgbM = (rgbM - 0.5.xxx) * _ColorBoost.y + 0.5.xxx;
        rgbM *= _ColorBoost.x;

        #if BEAUTIFY_COLOR_TWEAKS
            lumaM    = getLuma(rgbM);
            float3 kelvin = KelvinToRGB(_ColorTemp.x);
            rgbM = lerp(rgbM, rgbM * kelvin, _ColorTemp.y);
            //rgbM *= getLuma(rgbM) / lumaM; // energy preservation (optional)
        #endif

        #if BEAUTIFY_PURKINJE
              lumaM    = getLuma(rgbM);
        float3 shifted  = saturate(float3(lumaM / (1.0 + _Purkinje.x * 1.14), lumaM, lumaM * (1.0 + _Purkinje.x * 2.99)));
              rgbM     = lerp(shifted, rgbM, saturate(exp(avgLum.g) - _Purkinje.y));
        #endif

#if BEAUTIFY_VIGNETTING
            float2 vd = float2(i.uv.x  - 0.5, (i.uv.y - 0.5) * _VignettingAspectRatio);
            rgbM = lerp(_Vignetting.rgb, rgbM, saturate( saturate((dot(vd, vd) - _Vignetting.a) / (_Purkinje.w - _Vignetting.a) ) - _Purkinje.z)) ;
#elif BEAUTIFY_VIGNETTING_MASK
            float2 vd = float2(i.uv.x - 0.5, (i.uv.y - 0.5) * _VignettingAspectRatio);
            float  vmask = SAMPLE_TEXTURE2D_X(_VignettingMask, sampler_LinearClamp, uv).a;
            rgbM = lerp(rgbM, lumaM * _Vignetting.rgb, saturate(_Purkinje.z + vmask * _Vignetting.a * dot(vd, vd)));
#endif

#if BEAUTIFY_DITHER
    #if BEAUTIFY_TURBO
		float3 dither     = dot(float2(171.0, 231.0), i.uv * _MainTex_TexelSize.zw).xxx;
		       dither     = frac(dither / float3(103.0, 71.0, 97.0)) - 0.5.xxx;
		       rgbM      += dither * _ColorBoost.w;
    #else
        float2 noiseUV = i.uv * _BlueNoise_TexelSize.xy * _MainTex_TexelSize.zw;
        float3 blueNoise = SAMPLE_TEXTURE2D(_BlueNoise, sampler_PointRepeat, noiseUV).rgb;
        rgbM += (blueNoise - 0.5.xxx) * _ColorBoost.w * (1.0 - lumaM);
    #endif
        rgbM = max(0.0.xxx, rgbM);
#endif

	}

	float4 FragBeautify (VaryingsBeautify i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv     = UnityStereoTransformScreenSpaceTex(i.uv);

   		float4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv);
   		beautifyPass(i, pixel.rgb);
   		return pixel;
	}


    float4 FragCompare (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv     = UnityStereoTransformScreenSpaceTex(i.uv);

        // separator line + antialias
        float2 dd     = i.uv - 0.5.xx;
        float  co     = dot(_CompareParams.xy, dd);
        float  dist   = distance( _CompareParams.xy * co, dd );
        float4 aa     = saturate( (_CompareParams.w - dist) / abs(_MainTex_TexelSize.y) );

        float4 pixel  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv);
        float4 pixelNice = SAMPLE_TEXTURE2D_X(_CompareTex, sampler_MainTex, i.uv);
        
        // are we on the beautified side?
        float t       = dot(dd, _CompareParams.yz) > 0;
        pixel         = lerp(pixel, pixelNice, t);
        return pixel + aa;
    }


	half4 FragCopy (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv     = UnityStereoTransformScreenSpaceTex(i.uv);
        #if defined(USE_BILINEAR)
            return SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv);
        #else
		    return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
        #endif
	}

#endif // BEAUTIFY_CORE_FX