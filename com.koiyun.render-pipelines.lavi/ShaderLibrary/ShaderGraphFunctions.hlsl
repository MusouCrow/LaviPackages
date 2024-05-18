#pragma once

#include "./Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

TEXTURE2D(_ColorTexture);

#define SHADERGRAPH_RENDERER_BOUNDS_MIN unity_RendererBounds_Min
#define SHADERGRAPH_RENDERER_BOUNDS_MAX unity_RendererBounds_Max
#define SHADERGRAPH_SAMPLE_SCENE_COLOR(uv) shadergraph_SampleSceneColor_Lavi(uv)

float3 shadergraph_SampleSceneColor_Lavi(float2 uv)
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_ColorTexture, sampler_LinearClamp, uv, 0);

    return color;
}
