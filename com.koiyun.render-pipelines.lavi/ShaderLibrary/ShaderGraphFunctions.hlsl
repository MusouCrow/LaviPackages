#pragma once

#include "./Core.hlsl"
#include "./Shadow.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

TEXTURE2D(_ColorTexture);

#define SHADERGRAPH_RENDERER_BOUNDS_MIN unity_RendererBounds_Min
#define SHADERGRAPH_RENDERER_BOUNDS_MAX unity_RendererBounds_Max
#define SHADERGRAPH_SAMPLE_SCENE_COLOR(uv) shadergraph_SampleSceneColor_Lavi(uv)
#define SHADERGRAPH_MAIN_LIGHT_DIRECTION shadergraph_MainLightDirection_Lavi

float3 shadergraph_SampleSceneColor_Lavi(float2 uv)
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_ColorTexture, sampler_LinearClamp, uv, 0);

    return color.rgb;
}

float3 shadergraph_MainLightDirection_Lavi()
{
    return _LightDirection;
}
