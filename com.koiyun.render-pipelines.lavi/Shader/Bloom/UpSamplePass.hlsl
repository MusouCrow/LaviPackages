#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurHTexture);
TEXTURE2D(_BloomBlurVTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 blurH = SAMPLE_TEXTURE2D(_BloomBlurHTexture, sampler_LinearClamp, input.uv);
    float4 blurV = SAMPLE_TEXTURE2D(_BloomBlurVTexture, sampler_LinearClamp, input.uv);
    float3 hsv = RgbToHsv(blurV.rgb);
    hsv.g += hsv.g * 0.2; // 饱和度
    hsv.g = saturate(hsv.g);
    blurV.rgb = HsvToRgb(hsv);

    return blurH + blurV * 0.7;
}
