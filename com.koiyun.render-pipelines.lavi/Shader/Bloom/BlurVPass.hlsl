#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurTexture);
float4 _BloomBlurTexture_TexelSize;

float4 Frag(Varyings input) : SV_Target
{
    const half2 DX[9] = {
        {-1, -1}, {0, -1}, {1, -1},
        {-1, 0}, {0, 0}, {1, 0},
        {-1, 1}, {0, 1}, {1, 1}
    };

    float2 size = _BloomBlurTexture_TexelSize;
    float2 uv = input.uv;
    float4 color = 0;
    
    for (int i = 0; i < 9; i++)
    {
        float4 c = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, uv + size * DX[i], 0);
        color += c * 0.11111111;
    }

    return color;
}