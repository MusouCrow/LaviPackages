#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurHMap);
TEXTURE2D(_BloomBlurVMap);

float4 Frag(Varyings input) : SV_Target
{
    float4 blurH = SAMPLE_TEXTURE2D(_BloomBlurHMap, sampler_LinearClamp, input.uv);
    float4 blurV = SAMPLE_TEXTURE2D(_BloomBlurVMap, sampler_LinearClamp, input.uv);
    float3 hsv = RgbToHsv(blurV.rgb);
    hsv.g += hsv.g * 0.2; // 饱和度
    hsv.g = saturate(hsv.g);
    blurV.rgb = HsvToRgb(hsv);

    return blurH + blurV * 0.7;
}
