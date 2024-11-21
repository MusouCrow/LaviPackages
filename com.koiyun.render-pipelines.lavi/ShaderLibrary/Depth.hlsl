#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

TEXTURE2D_FLOAT(_DepthTexture);
SAMPLER(sampler_DepthTexture);

float SampleDepth(float2 uv)
{
    return SAMPLE_DEPTH_TEXTURE(_DepthTexture, sampler_DepthTexture, uv);
}
