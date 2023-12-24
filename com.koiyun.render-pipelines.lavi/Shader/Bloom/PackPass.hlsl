#pragma once

#include "./Include.hlsl"

TEXTURE2D(_ColorMap);

float4 Frag(Varyings input) : SV_Target
{
    return SAMPLE_TEXTURE2D(_ColorMap, sampler_LinearClamp, input.uv);
}