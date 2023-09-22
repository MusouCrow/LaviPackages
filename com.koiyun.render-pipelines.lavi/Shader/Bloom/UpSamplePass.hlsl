#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float4 blurH = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
    float4 blurV = SAMPLE_TEXTURE2D(_BloomBlurTexture, sampler_LinearClamp, input.uv);

    return lerp(blurH, blurV, 0.3);
}
