#ifndef BEAUTIFY_COLOR_TEMP
#define BEAUTIFY_COLOR_TEMP

const float LuminancePreservationFactor = 1.0;

const float PI2 = 6.2831853071;

// kelvin = { 1000 - 40000 }
float3 KelvinToRGB(float kelvin) {
  const float3x3 cold = float3x3(float3(0.0, -2902.1955373783176, -8257.7997278925690),
	                                      float3(0.0, 1669.5803561666639, 2575.2827530017594),
	                                      float3(1.0, 1.3302673723350029, 1.8993753891711275));
  const float3x3 hot = float3x3(float3(0.0, -2902.1955373783176, -8257.7997278925690),
	                                      float3(0.0, 1669.5803561666639, 2575.2827530017594),
	                                      float3(1.0, 1.3302673723350029, 1.8993753891711275));

  float3x3 m = (kelvin <= 6500.0) ? cold : hot;

  float3 m0 = float3(m[0][0], m[0][1], m[0][2]);
  float3 m1 = float3(m[1][0], m[1][1], m[1][2]);
  float3 m2 = float3(m[2][0], m[2][1], m[2][2]);
  float3 cm = lerp(( m0 / (clamp(kelvin, 1000.0, 40000.0).xxx + m1) + m2),
                   1.0.xxx,
                   smoothstep(1000.0, 0.0, kelvin));
  return cm;
}

#endif // BEAUTIFY_COLOR_TEMP