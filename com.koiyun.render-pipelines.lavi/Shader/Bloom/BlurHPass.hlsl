#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurMap);

float4 Frag(Varyings input) : SV_Target
{
    return SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, input.uv);
}