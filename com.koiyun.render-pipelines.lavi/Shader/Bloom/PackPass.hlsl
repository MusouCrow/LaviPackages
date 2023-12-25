#pragma once

#include "./Include.hlsl"

TEXTURE2D(_ColorTexture);

float4 Frag(Varyings input) : SV_Target
{
    return SAMPLE_TEXTURE2D(_ColorTexture, sampler_LinearClamp, input.uv);
}