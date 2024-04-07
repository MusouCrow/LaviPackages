#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurTexture);
float4 _BloomBlurTexture_TexelSize;

float4 Frag(Varyings input) : SV_Target
{
    const float2 DX[9] = {
        {-4, 0.01621622},
        {-3, 0.05405405},
        {-2, 0.12162162},
        {-1, 0.19459459},
        {0, 0.22702703},
        {1, 0.19459459},
        {2, 0.12162162},
        {3, 0.05405405},
        {4, 0.01621622},
    };

    float size = _BloomBlurTexture_TexelSize.x * 2;
    float2 uv = input.uv;
    float4 color = 0;

    for (int i = 0; i < 9; i++)
    {
        float4 c = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, uv + float2(size * DX[i].x, 0), 0);

        color += c * DX[i].y;
        // color.rgb = DDDD(color, c);
        // color.a += c.a * DX[i].y;
    }

    return color;
}