#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

TEXTURE2D(_MainTex);
float4 _MainTex_TexelSize;

struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

Varyings Vert(Attributes input)
{
    Varyings output;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = input.texcoord;

    return output;
}

half4 Frag(Varyings input) : SV_Target
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

    float4 color = 0;
    float2 size = _MainTex_TexelSize.xy;

    for (int i = 0; i < 9; i++)
    {
        color += SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_LinearClamp, input.uv + DX[i] * size, 0) / 9;
    }

    // color = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_LinearClamp, input.uv, 0);

    return color;
}
