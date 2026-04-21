#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void CalculateMainLight_float(float3 WorldPos, 
		out float3 Direction, out float3 Color, out float Diffuse) {
#if defined(SHADERGRAPH_PREVIEW)
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    Diffuse = 1;
#else
	#if SHADOWS_SCREEN
		half4 clipPos = TransformWorldToHClip(WorldPos);
		half4 shadowCoord = ComputeScreenPos(clipPos);
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
	#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	Diffuse = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
#endif
}

void CalculateAdditionalLights_float(float3 WorldPos, float3 WorldNormal, float3 WorldView, float MainDiffuse, float3 MainColor,
		out float Diffuse, out float3 Color) {
	Diffuse = MainDiffuse;
	Color = MainColor;
#ifndef SHADERGRAPH_PREVIEW
	int lightCount = GetAdditionalLightsCount();
	for (int i = 0; i < lightCount; i++) {
		Light light = GetAdditionalLight(i, WorldPos);
		half NdotL = saturate(dot(WorldNormal, light.direction));
		half atten = light.distanceAttenuation * light.shadowAttenuation;
		half thisDiffuse = atten * NdotL;
		Diffuse -= thisDiffuse;
		Color += light.color * thisDiffuse;
	}
#endif
	Color = Diffuse <= 0 ? MainColor : Color / Diffuse;
}

#endif