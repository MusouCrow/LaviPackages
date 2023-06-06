#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

TEXTURE2D_FLOAT(_MainTex);
SAMPLER(sampler_MainTex);

struct Attributes
{
    uint vertexID : SV_VertexID;
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
    output.positionCS = GetQuadVertexPosition(input.vertexID);
    output.positionCS.xy = output.positionCS.xy * float2(2.0f, -2.0f) + float2(-1.0f, 1.0f);
    output.uv = GetQuadTexCoord(input.vertexID);

    return output;
}

float Frag(Varyings input) : SV_Depth
{
    float depth = SAMPLE_DEPTH_TEXTURE(_MainTex, sampler_MainTex, input.uv);

    return depth;
}