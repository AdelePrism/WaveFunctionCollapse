#ifndef CUSTOM_SCENE_COLOR_SAMPLING_INCLUDED
#define CUSTOM_SCENE_COLOR_SAMPLING_INCLUDED

// Required to access the camera opaque texture
TEXTURE2D(_CameraOpaqueTexture);
SAMPLER(sampler_point_clamp_CameraOpaqueTexture);
// Function to sample the scene color at full resolution (LOD 0)
void SampleSceneColor_float(float2 uv, out float4 color)
{
   color = SAMPLE_TEXTURE2D_LOD(_CameraOpaqueTexture, sampler_point_clamp_CameraOpaqueTexture, uv, 0);
}

#endif // CUSTOM_SCENE_COLOR_SAMPLING_INCLUDED