#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurLowTexture);
TEXTURE2D(_BloomBlurHighTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 low = SAMPLE_TEXTURE2D_LOD(_BloomBlurLowTexture, sampler_LinearClamp, input.uv, 0);
    float4 high = SAMPLE_TEXTURE2D_LOD(_BloomBlurHighTexture, sampler_LinearClamp, input.uv, 0);

    return lerp(high, low, 0.5);
}
