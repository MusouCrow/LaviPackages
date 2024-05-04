#pragma once

#include "./Include.hlsl"

float2 _OutlineParams; // OutlineBrightness, OutlineThickness

TEXTURE2D(_ColorTexture);
TEXTURE2D(_RawParamTexture);
TEXTURE2D(_BloomBlurTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_ColorTexture, sampler_LinearClamp, input.uv, 0);
    float4 param = SAMPLE_TEXTURE2D_LOD(_RawParamTexture, sampler_LinearClamp, input.uv, 0);
    float4 blur = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, input.uv, 0);
    float layer = max(blur.a, param.g);
    
    float3 hsv = RgbToHsv(blur.rgb); // 色相、飽和度、明度
    hsv.b = hsv.b / (1 + hsv.b);

    float saturation = lerp(3, 1, hsv.b);
    hsv.g = saturate(hsv.g * saturation);
    // hsv.g = lerp(hsv.g, 1, blur.a);
    blur.rgb = HsvToRgb(hsv);

    hsv = RgbToHsv(color.rgb);
    color.rgb *= lerp(lerp(_OutlineParams.x * 2, _OutlineParams.x * 0.5, hsv.b), 1, 1 - layer);
    
    color.rgb += blur.rgb;

    return color;
}
