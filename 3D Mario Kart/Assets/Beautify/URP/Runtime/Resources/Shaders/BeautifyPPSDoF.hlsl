#ifndef BEAUTIFY_PPSDOF_FX
#define BEAUTIFY_PPSDOF_FX

	// Copyright 2020 Kronnect - All Rights Reserved.
    #include "BeautifyCommon.hlsl"
    
    TEXTURE2D_X(_DepthTexture);
    TEXTURE2D_X(_DofExclusionTexture);    
    TEXTURE2D_X(_MainTex);

    float4    _MainTex_TexelSize;
    float4    _MainTex_ST;
    float4    _BokehData;
    float4    _BokehData2;
    float3    _BokehData3;
    float     _BlurScale;

    struct VaryingsDoFCross {
	    float4 positionCS : SV_POSITION;
        float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
    };

    
    float getCoc(Varyings i) {
        #if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
            float depthTex = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DepthTexture, sampler_PointClamp, i.uvNonStereo, 0));
            float exclusionDepth = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DofExclusionTexture, sampler_PointClamp, i.uvNonStereo, 0));
            float depth  = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
            depth = min(depth, depthTex);
            if (exclusionDepth < depth) return 0;
            depth *= _ProjectionParams.z;
        #else
            float depth  = BEAUTIFY_GET_SCENE_DEPTH_EYE(i.uv);
        #endif
        if (depth>_BokehData3.y) return 0;
        float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
        return 0.5 * _BokehData.y * xd/depth;    // radius of CoC
    }
                
    float4 FragCoC (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 pixel  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        pixel         = clamp(pixel, 0.0.xxxx, _BokehData3.xxxx);
        #if UNITY_COLORSPACE_GAMMA
            pixel.rgb     = GAMMA_TO_LINEAR(pixel.rgb);
        #endif
        float coc = getCoc(i) / COC_BASE;
        return float4(pixel.rgb, coc);
    }    
    
    float4 FragCoCDebug (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 pixel  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        pixel          = clamp(pixel, 0.0.xxxx, _BokehData3.xxxx);
        float  CoC   = getCoc(i) / COC_BASE;
        pixel.a      = min(CoC, pixel.a);
        return pixel.aaaa;
    }

    float4 FragBlur (Varyings i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 sum     = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv );
        float  samples = ceil(sum.a * COC_BASE);

        float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
               dir    *= max(1.0, samples / _BokehData2.y);
        float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
        float2 disp0   = dir.xy * (frac(jitter) + 0.5);
        float4 disp1   = float4(i.uv + disp0, 0, 0);
        float4 disp2   = float4(i.uv - disp0, 0, 0);
        float  w       = 1.0;

        const int sampleCount = (int)min(_BokehData2.y, samples);
        UNITY_UNROLL
        for (int k=1;k<16;k++) {
            if (k<sampleCount) {

                float4 pixel1       = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, disp1.xy, 0);
                float  bt1         = saturate(pixel1.a * COC_BASE - k);
                       pixel1.rgb += _BokehData2.www * max(pixel1.rgb - _BokehData2.zzz, 0.0.xxx);
                       sum        += pixel1 * bt1;
                       w           += bt1;
                       disp1      += dir;

                       float4 pixel2 = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, disp2.xy, 0);
                       float  bt2  = saturate(pixel2.a * COC_BASE - k);
                       pixel2.rgb += _BokehData2.www * max(pixel2.rgb - _BokehData2.zzz, 0.0.xxx);
                       sum        += pixel2 * bt2;
                       w          += bt2;
                       disp2      -= dir;
            }
        }
        return sum / w;
    }

    float4 FragBlurNoBokeh (Varyings i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 sum     = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv );
        float samples  = ceil(sum.a * COC_BASE);
        float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
               dir    *= max(1.0, samples / _BokehData2.y);
        float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
        float2 disp0   = dir.xy * (frac(jitter) + 0.5);
        float4 disp1   = float4(i.uv + disp0, 0, 0);
        float4 disp2   = float4(i.uv - disp0, 0, 0);
        float  w       = 1.0;

        const int sampleCount = (int)min(_BokehData2.y, samples);
        UNITY_UNROLL
        for (int k=1;k<16;k++) {
            if (k<sampleCount) {
                    float4 pixel1      = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, disp1.xy, 0);
                float  bt1         = saturate(pixel1.a * COC_BASE - k);                
                       sum        += bt1 * pixel1;
                       w           += bt1;
                       disp1      += dir;
                    float4 pixel2      = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, disp2.xy, 0);
                float  bt2  = saturate(pixel2.a * COC_BASE - k);                
                       sum        += bt2 * pixel2;
                       w          += bt2;
                       disp2      -= dir;
            }
        }
        return sum / w;
    }

    VaryingsDoFCross VertBlur(Attributes input) {
        VaryingsDoFCross output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;

        BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(output);

        return output;
    }


   float4 FragBlurCoC (VaryingsDoFCross i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i)

        float depth   = BEAUTIFY_GET_SCENE_DEPTH_EYE(i.uv);
        float depth1   = BEAUTIFY_GET_SCENE_DEPTH_EYE(uv1);
        float depth2   = BEAUTIFY_GET_SCENE_DEPTH_EYE(uv2);
        float depth3   = BEAUTIFY_GET_SCENE_DEPTH_EYE(uv3);
        float depth4   = BEAUTIFY_GET_SCENE_DEPTH_EYE(uv4);

        const float f = 10;
        float w1      = saturate((depth - depth1)/f) * 0.3162162162; 
        float w2      = saturate((depth - depth2)/f) * 0.3162162162; 
        float w3      = saturate((depth - depth3)/f) * 0.0702702703; 
        float w4      = saturate((depth - depth4)/f) * 0.0702702703; 

        float coc1    = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1).a;
        float coc2    = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2).a;
        float coc3    = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3).a;
        float coc4    = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4).a;

        float w0      = 0.2270270270;

        half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);

        float coc     = (pixel.a * w0 + coc1 * w1 + coc2 * w2 + coc3 * w3 + coc4 * w4) / (w0 + w1 + w2 + w3 + w4);
        pixel.a = coc;
        return pixel;
    }   


#endif // BEAUTIFY_PPSDOF_FX


