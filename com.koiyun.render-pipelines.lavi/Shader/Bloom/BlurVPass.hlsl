#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurTexture);
float4 _BloomBlurTexture_TexelSize;

float4 Frag(Varyings input) : SV_Target
{
    const float2 DX[5] = {
        {-3.23076923, 0.07027027},
        {-1.38461538, 0.3162162},
        {0, 0.22702703},
        {1.38461538, 0.3162162},
        {3.23076923, 0.07027027},
    };

    float size = _BloomBlurTexture_TexelSize.y;
    float2 uv = input.uv;
    float4 color = 0;

    for (int i = 0; i < 5; i++)
    {
        float4 c = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, uv + float2(0, size * DX[i].x), 0);

        color += c * DX[i].y;
        // color.rgb = DDDD(color, c);
        // color.a += c.a * DX[i].y;
    }

    return color;
}