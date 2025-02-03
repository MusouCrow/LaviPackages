#pragma once

#include "./Include.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Shadow.hlsl"

float2 _OutlineParams; // OutlineBrightness, OutlineThickness

TEXTURE2D(_ColorTexture);
TEXTURE2D(_AmbientOcclusionTexture);
TEXTURE2D(_RawParamTexture);
TEXTURE2D(_BloomBlurTexture);

float4 _AmbientOcclusionTexture_TexelSize;
/*
float SampleAO(float2 uv)
{
    const float2 DX[9] = {
        {-1, -1},
        {-1, 1},
        {1, -1},
        {1, 1},
        {1, 0},
        {-1, 0},
        {0, -1},
        {0, 1},
        {0, 0}
    };

    float2 size = _AmbientOcclusionTexture_TexelSize.xy;
    float ao = 0;

    for (int i = 0; i < 9; i++)
    {
        ao += SAMPLE_TEXTURE2D_LOD(_AmbientOcclusionTexture, sampler_LinearClamp, uv + DX[i] * size, 2) / 9;
    }

    return ao;
}
*/
float4 Frag(Varyings input) : SV_Target
{
    float2 uv = lerp(0.02, 0.98, input.uv);
    // float2 uv = input.uv;
    float4 color = SAMPLE_TEXTURE2D_LOD(_ColorTexture, sampler_LinearClamp, uv, 0);
    float4 param = SAMPLE_TEXTURE2D_LOD(_RawParamTexture, sampler_LinearClamp, uv, 0);
    float4 blur = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, uv, 0);
    float layer = 1 - param.g;
    float ao = blur.a;
    
    float3 hsv = RgbToHsv(blur.rgb); // 色相、飽和度、明度
    hsv.b = hsv.b / (1 + hsv.b * 0.5);

    float saturation = lerp(5, 1, hsv.b);
    hsv.g = saturate(hsv.g * saturation);
    // hsv.g = lerp(hsv.g, 1, blur.a);
    blur.rgb = HsvToRgb(hsv);

    hsv = RgbToHsv(color.rgb);
    color.rgb *= lerp(lerp(_OutlineParams.x * 2, _OutlineParams.x * 0.5, hsv.b), 1, layer);
    
    ao = saturate(pow(ao, 2) + pow(ao, 0.6));
    color.rgb = lerp(color.rgb, _ShadowAttens.y * color.rgb, ao);
    color.rgb += blur.rgb;

    return color;
}
