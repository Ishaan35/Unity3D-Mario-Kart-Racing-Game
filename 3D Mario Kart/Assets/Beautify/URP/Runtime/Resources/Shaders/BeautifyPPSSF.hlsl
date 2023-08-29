#ifndef BEAUTIFY_PPSSF_FX
#define BEAUTIFY_PPSSF_FX		

	// Copyright 2020 Kronnect - All Rights Reserved.
    #include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D(_FlareTex);

	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	float4	  _SunPos;
	float4    _SunData;	// x = sunIntensity, y = disk size, z = ray difraction, w = ray difraction amount
	float4    _SunCoronaRays1;  // x = length, y = streaks, z = spread, w = angle offset
	float4    _SunCoronaRays2;  // x = length, y = streaks, z = spread, w = angle offset
	float4    _SunGhosts1;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4    _SunGhosts2;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4    _SunGhosts3;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4    _SunGhosts4;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
   	float3    _SunHalo;  // x = offset, y = amplitude, z = intensity
   	float3    _SunTint;
	float3	  _SunPosRightEye;
   	
	void rotate(inout float2 uv, float ang) {
		float2 sico;
		sincos(ang, sico.x, sico.y);
		float2 cosi = float2(sico.y, -sico.x);
		uv = float2(dot(cosi, uv), dot(sico, uv));
	}
	
   	float3 sunflare(float2 uv) {


		// general params
   		float2 sunPos = unity_StereoEyeIndex == 0 ? _SunPos.xy : _SunPosRightEye.xy;

		float2 depthSunPos = sunPos;
   		float depth  = BEAUTIFY_GET_SCENE_DEPTH_01(depthSunPos);
		if (depth<1) return 0;

   		float2 grd = uv - sunPos;
		float aspectRatio = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
   		grd.y *= aspectRatio; 
   		float len = length(grd);

   		// sun disk
   		float s0 = pow( 1.0 + saturate(_SunData.y - len), 75) - 1.0;
        
   		// corona rays
		float gang = _SunPos.w; //atan2(0.5 - sunPos.y, sunPos.x - 0.5);
   		float ang = atan2(grd.y, grd.x) + gang;
   		float ray1 = _SunCoronaRays1.z + abs(_SunCoronaRays1.x * cos(_SunCoronaRays1.w + ang * _SunCoronaRays1.y));	// design
   		ray1 *= pow( 1.0 + len, 1.0/_SunCoronaRays1.x);	
   		s0 += 1.0 / ray1;

   		float ray2 = _SunCoronaRays2.z + abs(_SunCoronaRays2.x * sin(_SunCoronaRays2.w + ang * _SunCoronaRays2.y));	// design
   		ray2 *= pow( 1.0 + len, 1.0/_SunCoronaRays2.x);	
   		s0 += 1.0 / ray2;
   		
   		s0 *= _SunData.x;
   		
   		float3 flare = s0.xxx;

		#if !defined(UNITY_SINGLE_PASS_STEREO) && !defined(UNITY_STEREO_INSTANCING_ENABLED) && !defined(UNITY_STEREO_MULTIVIEW_ENABLED)
   		// ghosts circular (not compatible with single pass stereo due to how projection works)

   		float2 ghost1Pos  = 1.0 - sunPos;
   		grd = uv - ghost1Pos + (ghost1Pos - 0.5) * _SunGhosts1.z;
		grd.y *= aspectRatio;

		float g0 = saturate(_SunGhosts1.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare += g0 * _SunGhosts1.w / len;

   		float2 ghost2Pos  = 1.0 - sunPos;
   		grd = uv - ghost2Pos + (ghost2Pos - 0.5) * _SunGhosts2.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts2.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts2.w / len;

   		float2 ghost3Pos  = 1.0 - sunPos;
   		grd = uv - ghost3Pos + (ghost3Pos - 0.5) * _SunGhosts3.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts3.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts3.w / len;

   		float2 ghost4Pos  = 1.0 - sunPos;
   		grd = uv - ghost4Pos + (ghost4Pos - 0.5) * _SunGhosts4.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts4.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts4.w / len;

   		#endif

		// light rays
		float2 uv2 = uv - sunPos;
		float clen = length(uv2);
		rotate(uv2, gang);
		uv2.x *= aspectRatio;
		uv2.x *= 0.1;
		uv2 /= len;
		float lr = saturate(SAMPLE_TEXTURE2D(_FlareTex, sampler_LinearRepeat, uv2 + _SunPos.zz).r - _SunData.w);
		float3 rays = lr * sin(float3(len, len + 0.1, len + 0.2) * 3.1415927);
		float atten = pow(1.0 + clen, 13.0);
		rays *= _SunData.z / atten;
		flare += rays;

		// halo
		float hlen = clamp( (len - _SunHalo.x) * _SunHalo.y, 0, 3.1415927);
		float3 halo = pow(sin(float3(hlen, hlen + 0.1, hlen + 0.2)), 12.0.xxx);
		halo *= _SunHalo.z / atten;
		flare += halo; 
		
		return flare * _SunTint;
   	}  
   	
  	float4 FragSF (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        //i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

   		return float4(sunflare(i.uv), 1.0);
   	}  

  	float4 FragSFAdditive (Varyings i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        //i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

  		float4 p = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
   		return p + float4(sunflare(i.uv), 1.0);
   	}  

#endif