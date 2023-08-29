Shader "Hidden/Kronnect/Beautify" {
Properties {
    _MainTex("", any) = "" {}
    _FlareTex("Flare Texture", 2D) = "white" {}
    _OverlayTex("Lens Dirt Texture", 2D) = "black" {}
    _Color("", Color) = (1,1,1)
    _BlueNoise("Blue Noise", 2D) = "black" {}
    _BokehData("", Vector) = (1,1,1,1)
    _BokehData2("", Vector) = (1,1,1,1)
    _BokehData3("", Vector) = (1,1,1,1)
}

HLSLINCLUDE
    #pragma target 3.0
ENDHLSL


Subshader {	

    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
    LOD 100
    ZWrite Off ZTest Always Blend Off Cull Off

    HLSLINCLUDE
    #pragma target 3.0
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    ENDHLSL

  Pass { // 0 Raw Copy (Point Filtering)
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragCopy
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 1 Compare View
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragCompare
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 2  Main Beautify Pass (core)
      HLSLPROGRAM
      #pragma vertex VertBeautify
      #pragma fragment FragBeautify
      #pragma multi_compile_local __ BEAUTIFY_TONEMAP_ACES
      #pragma multi_compile_local __ BEAUTIFY_LUT BEAUTIFY_NIGHT_VISION
	  #pragma multi_compile_local __ BEAUTIFY_BLOOM
      #pragma multi_compile_local __ BEAUTIFY_DIRT
      #pragma multi_compile_local __ BEAUTIFY_DEPTH_OF_FIELD 
      #pragma multi_compile_local __ BEAUTIFY_PURKINJE
      #pragma multi_compile_local __ BEAUTIFY_VIGNETTING BEAUTIFY_VIGNETTING_MASK
      #pragma multi_compile_local __ BEAUTIFY_EYE_ADAPTATION
      #pragma multi_compile_local __ BEAUTIFY_OUTLINE
      #pragma multi_compile_local __ BEAUTIFY_COLOR_TWEAKS
      #pragma multi_compile_local __ BEAUTIFY_TURBO
      #pragma multi_compile_local __ BEAUTIFY_DITHER
      #pragma multi_compile_local __ BEAUTIFY_SHARPEN

      #include "BeautifyCore.hlsl"

	  //#pragma multi_compile_local __ BEAUTIFY_DEPTH_OF_FIELD BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      ENDHLSL
  }

  Pass { // 3 Extract luminance
      HLSLPROGRAM
      #pragma vertex VertLum
      #pragma fragment FragLum
      #pragma multi_compile_local __ BEAUTIFY_TURBO
	  #pragma multi_compile_local __ BEAUTIFY_BLOOM_USE_DEPTH
	  #pragma multi_compile_local __ BEAUTIFY_BLOOM_USE_LAYER
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 4 Debug bloom
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragDebugBloom
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 5 Blur horizontally
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #define BEAUTIFY_BLUR_HORIZ
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }    
      
  Pass { // 6 Blur vertically
	  HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #define BEAUTIFY_BLUR_VERT
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }    

  Pass { // 7 Bloom compose
	  HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragBloomCompose
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }   

  Pass { // 8 Resample
	  HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragResample
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 9 Combine resample
	  HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragResample
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }   

  Pass { // 10 Bloom extract luminance with antiflicker
	  HLSLPROGRAM
      #pragma vertex VertCrossLum
      #pragma fragment FragLumAntiflicker
      #pragma multi_compile_local __ BEAUTIFY_TURBO
      #pragma multi_compile_local __ BEAUTIFY_BLOOM_USE_DEPTH
      #pragma multi_compile_local __ BEAUTIFY_BLOOM_USE_LAYER
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

   Pass { // 11 Resample Anamorphic Flares
	  HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragResampleAF
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }

  Pass { // 12 Combine AF
	  HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragCombine
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 13 Compute Screen Lum
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragScreenLum
      #pragma multi_compile_local __ BEAUTIFY_TURBO
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }      
  
  Pass { // 14 Reduce Screen Lum
      HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragReduceScreenLum
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }  

  Pass { // 15 Blend Screen Lum
      Blend SrcAlpha OneMinusSrcAlpha
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragBlendScreenLum
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }      
  
  Pass { // 16 Simple Blend
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragBlend
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }  

  Pass { // 17 AF Lum
      HLSLPROGRAM
      #pragma vertex VertLum
      #pragma fragment FragLum
      #pragma multi_compile_local __ BEAUTIFY_TURBO
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #pragma multi_compile_local __ BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
      #define USE_AF_THRESHOLD
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 18 AF Lum AntiFlicker
      HLSLPROGRAM
      #pragma vertex VertCrossLum
      #pragma fragment FragLumAntiflicker
      #pragma multi_compile_local __ BEAUTIFY_TURBO
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #pragma multi_compile_local __ BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
      #define USE_AF_THRESHOLD
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

 Pass { // 19 Sun Flares
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragSF
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }
  
 Pass { // 20 Sun Flares Additive
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragSFAdditive
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }

 Pass { // 21 DoF CoC
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local __ BEAUTIFY_TURBO
      //#pragma multi_compile_local __ BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 
 
  Pass { // 22 DoF CoC Debug
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragCoCDebug
      #pragma fragmentoption ARB_precision_hint_fastest
      //#pragma multi_compile_local __ BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 
 
  Pass { // 23 DoF Blur
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

 Pass { // 24 DoF Blur wo/Bokeh
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragBlurNoBokeh
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

Pass { // 25 DoF Blur Horizontally
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #define BEAUTIFY_BLUR_HORIZ
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

Pass { // 26 DoF Blur Vertically
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #define BEAUTIFY_BLUR_VERT
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

Pass { // 27 Raw Copy (Bilinear Filtering)
      HLSLPROGRAM
      #pragma vertex Vert
      #pragma fragment FragCopy
      #define USE_BILINEAR
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

}
FallBack Off
}
