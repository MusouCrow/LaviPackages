#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

TEXTURE2D_FLOAT(_MainTex);
SAMPLER(sampler_LinearClamp);

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

float Frag(Varyings input) : SV_Depth
{
    float depth = SAMPLE_DEPTH_TEXTURE(_MainTex, sampler_LinearClamp, input.uv);

    return depth;
}
