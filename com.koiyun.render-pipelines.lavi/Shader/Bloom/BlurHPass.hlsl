#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurTexture);

float4 Frag(Varyings input) : SV_Target
{
    return SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, input.uv, 0);
}