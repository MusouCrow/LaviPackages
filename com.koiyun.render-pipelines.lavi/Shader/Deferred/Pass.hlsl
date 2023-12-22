#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Shadow.hlsl"

TEXTURE2D(_GBufferColor);
SAMPLER(sampler_PointClamp);
SAMPLER(sampler_LinearClamp);

struct Attributes
{
    uint vertexID : SV_VertexID;
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

half4 Frag(Varyings input) : SV_Target
{
    half4 color = SAMPLE_TEXTURE2D(_GBufferColor, sampler_LinearClamp, input.uv);

    return color;
}
