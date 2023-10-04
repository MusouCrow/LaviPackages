#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float4 blurH = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
    float4 blurV = SAMPLE_TEXTURE2D(_BloomBlurTexture, sampler_LinearClamp, input.uv);
    half brightness = Max3(blurV.r, blurV.g, blurV.b);

    return blurH * 0.6 + blurV * lerp(0.4, 0.7, saturate(brightness));
}
