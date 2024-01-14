#pragma once

#include "./Include.hlsl"

TEXTURE2D(_ColorTexture);
TEXTURE2D(_BloomBlurTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_ColorTexture, sampler_LinearClamp, input.uv, 0);
    float4 blur = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, input.uv, 0);
    
    float3 hsv = RgbToHsv(blur.rgb); // 色相、飽和度、明度
    float saturation = lerp(0.5, 0, hsv.b);
    hsv.g = saturate(hsv.g + saturation);
    blur.rgb = HsvToRgb(hsv);
    
    color.rgb += blur.rgb;
    
    return color;
}
