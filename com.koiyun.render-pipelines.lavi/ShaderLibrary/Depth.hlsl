#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

TEXTURE2D_FLOAT(_DepthTexture);

float SampleDepth(float2 uv, int lod)
{
    return SAMPLE_DEPTH_TEXTURE_LOD(_DepthTexture, sampler_LinearClamp, uv, lod);
}
