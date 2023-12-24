#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

TEXTURE2D(_GBufferColor);
TEXTURE2D(_GBufferOther);
SAMPLER(sampler_PointClamp);

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
    half4 color = SAMPLE_TEXTURE2D_LOD(_GBufferColor, sampler_PointClamp, input.uv, 0);
    float4 other = SAMPLE_TEXTURE2D_LOD(_GBufferOther, sampler_PointClamp, input.uv, 0);
    float glow = other.r * 10;

    return half4(color.rgb * glow, color.a);
}
