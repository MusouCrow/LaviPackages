#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float4 blurH = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
    float4 blurV = SAMPLE_TEXTURE2D(_BloomBlurTexture, sampler_LinearClamp, input.uv);
    float3 hsv = RgbToHsv(blurV.rgb);
    hsv.g += hsv.g * 0.2; // 饱和度
    hsv.g = saturate(hsv.g);
    blurV.rgb = HsvToRgb(hsv);

    return blurH + blurV * 0.7;
}
