#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Depth.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "./Lib.hlsl"

float4 _LightColor;

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
    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
    output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

    return output;
}

float4 Frag(Varyings input) : SV_TARGET
{
    float depth = SampleDepth(input.uv, 3);
    depth = LinearEyeDepth(depth, _ZBufferParams);
    
    float3 positionVS = ReconstructViewPosition(input.uv, depth);
    float3 normalVS = ReconstructNormal(positionVS);

    float ao = AOProcess(4, 1, input.uv, normalVS, positionVS, depth);
    ao += AOProcess(3, 5, input.uv, normalVS, positionVS, depth);

    return ao;
}
