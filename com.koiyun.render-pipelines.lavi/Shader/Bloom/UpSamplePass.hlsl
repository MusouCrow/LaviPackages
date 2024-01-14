#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurHTexture);
TEXTURE2D(_BloomBlurVTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 blurH = SAMPLE_TEXTURE2D_LOD(_BloomBlurHTexture, sampler_LinearClamp, input.uv, 0);
    float4 blurV = SAMPLE_TEXTURE2D_LOD(_BloomBlurVTexture, sampler_LinearClamp, input.uv, 0);

    return blurH + blurV * 0.7;
}
