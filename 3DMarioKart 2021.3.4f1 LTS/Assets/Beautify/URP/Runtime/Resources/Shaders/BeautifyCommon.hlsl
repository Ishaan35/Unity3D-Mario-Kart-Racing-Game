#ifndef __BEAUTIFY_COMMON_INCLUDE
#define __BEAUTIFY_COMMON_INCLUDE

TEXTURE2D_X_FLOAT(_CameraDepthTexture);

float HighPrecisionSampleSceneDepth(float2 uvStereo)
{
    return SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_PointClamp, uvStereo).r;
}


#define BEAUTIFY_GET_DEPTH_01(x) Linear01Depth(x, _ZBufferParams)
#define BEAUTIFY_GET_DEPTH_EYE(x) LinearEyeDepth(x, _ZBufferParams)

#define BEAUTIFY_GET_SCENE_DEPTH_01(x) Linear01Depth(HighPrecisionSampleSceneDepth(x), _ZBufferParams)
#define BEAUTIFY_GET_SCENE_DEPTH_EYE(x) LinearEyeDepth(HighPrecisionSampleSceneDepth(x), _ZBufferParams)

#define BEAUTIFY_GET_CUSTOM_DEPTH_01(s, x) Linear01Depth(SAMPLE_TEXTURE2D_X(s, sampler_PointClamp, x).r, _ZBufferParams)


#if defined(BEAUTIFY_ORTHO)
    #if UNITY_REVERSED_Z
  		#define Linear01Depth(x) (1.0-x)
       	#define LinearEyeDepth(x) ((1.0-x) * _ProjectionParams.z)
    #else
	   	#define Linear01Depth(x) (x)
   		#define LinearEyeDepth(x) (x * _ProjectionParams.z)
    #endif
#endif

// Base for COC - required for Android
#define COC_BASE 128


// Optimization for SSPR
#define uvN uv1
#define uvE uv2
#define uvW uv3
#define uvS uv4


#if defined(UNITY_SINGLE_PASS_STEREO)
    #define BEAUTIFY_VERTEX_CROSS_UV_DATA
    #define BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o)
    #define BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(o)

    #define BEAUTIFY_FRAG_SETUP_CROSS_UV(i) float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0); float2 uvN = i.uv + uvInc.zy; float2 uvE = i.uv + uvInc.xz; float2 uvW = i.uv - uvInc.xz; float2 uvS = i.uv - uvInc.zy;

    #if defined(BEAUTIFY_BLUR_HORIZ)
        #define BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i) float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0); float2 uv1 = i.uv - inc; float2 uv2 = i.uv + inc; float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0); float2 uv3 = i.uv - inc2; float2 uv4 = i.uv + inc2;
    #else
        #define BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i) float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale); float2 uv1 = i.uv - inc; float2 uv2 = i.uv + inc; float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale); float2 uv3 = i.uv - inc2; float2 uv4 = i.uv + inc2;
    #endif

#else
    #define BEAUTIFY_VERTEX_CROSS_UV_DATA float2 uvN : TEXCOORD1; float2 uvW: TEXCOORD2; float2 uvE: TEXCOORD3; float2 uvS: TEXCOORD4;

    #define BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o) float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0); o.uvN = o.uv + uvInc.zy; o.uvE = o.uv + uvInc.xz; o.uvW = o.uv - uvInc.xz; o.uvS = o.uv - uvInc.zy;
    #define BEAUTIFY_FRAG_SETUP_CROSS_UV(i) float2 uv1 = i.uv1; float2 uv2 = i.uv2; float2 uv3 = i.uv3; float2 uv4 = i.uv4;

    #if defined(BEAUTIFY_BLUR_HORIZ)
        #define BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(o) float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0); o.uv1 = o.uv - inc; o.uv2 = o.uv + inc; float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0); o.uv3 = o.uv - inc2; o.uv4 = o.uv + inc2;
    #else
        #define BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(o) float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale); o.uv1 = o.uv - inc; o.uv2 = o.uv + inc; float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale); o.uv3 = o.uv - inc2; o.uv4 = o.uv + inc2;
    #endif
    #define BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i) float2 uv1 = i.uv1; float2 uv2 = i.uv2; float2 uv3 = i.uv3; float2 uv4 = i.uv4;

#endif


#if BEAUTIFY_TURBO
    #define GAMMA_TO_LINEAR(x) FastSRGBToLinear(x)
    #define LINEAR_TO_GAMMA(x) FastLinearToSRGB(x)
#else
    #define GAMMA_TO_LINEAR(x) SRGBToLinear(x)
    #define LINEAR_TO_GAMMA(x) LinearToSRGB(x)
#endif


// Common functions


inline float getLuma(float3 rgb) { 
	const float3 lum = float3(0.299, 0.587, 0.114);
	return dot(rgb, lum);
}


#endif // __BEAUTIFY_COMMON_INCLUDE