#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

TEXTURE2D(_TempColorTexture);
TEXTURE2D_FLOAT(_NormalTexture);
TEXTURE2D_FLOAT(_DepthTexture);
SAMPLER(sampler_PointClamp);
SAMPLER(sampler_LinearClamp);

float4 _DepthTexture_TexelSize;

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

float SobelDepth(float2 uv)
{
    const half2 DX[9] = {
        {0, 0},
        {-1, 0}, {-2, 0},
        {1, 0}, {2, 0},
        {0, -1}, {0, -2},
        {0, 1}, {0, 2},
    };

    const half SO[9] = {
        8,
        -1, -1,
        -1, -1,
        -1, -1,
        -1, -1
    };

    float edge = 0;

    for (int i = 0; i < 9; i++) {
        float2 _uv = uv + DX[i] * _DepthTexture_TexelSize.xy;
        float depth = SAMPLE_DEPTH_TEXTURE_LOD(_DepthTexture, sampler_LinearClamp, _uv, 0);
        depth = 1 - Linear01Depth(depth, _ZBufferParams);
        
        float3 normal = SAMPLE_TEXTURE2D_LOD(_NormalTexture, sampler_PointClamp, _uv, 0);
        normal = (normal + 1) / 2; // [-1, 1] -> [0, 1]
        float3 hsv = RgbToHsv(normal);

        edge += saturate(depth) * SO[i];
    }

    edge = saturate(edge);
    edge = edge > 0.0005 ? 1 : 0;

    return edge;
}

float SobelNormal(float2 uv)
{
    const half2 DX[5] = {
        {0, 0}, {-1, 0}, {1, 0}, {0, -1}, {0, 1}
    };

    const half SO[5] = {
        4, -1, -1, -1, -1
    };

    float edge = 0;

    for (int i = 0; i < 5; i++) {
        float2 _uv = uv + DX[i] * _DepthTexture_TexelSize.xy;
        float3 normal = SAMPLE_TEXTURE2D_LOD(_NormalTexture, sampler_PointClamp, _uv, 0);
        normal = (normal + 1) / 2; // [-1, 1] -> [0, 1]
        float3 hsv = RgbToHsv(normal);
        edge += hsv.r * SO[i];
    }

    edge = saturate(edge);
    edge = edge > 0.15 ? 1 : 0;

    return edge;
}

Varyings Vert(Attributes input)
{
    Varyings output;
    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
    output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

    return output;
}

float4 Frag(Varyings input) : SV_TARGET
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_TempColorTexture, sampler_PointClamp, input.uv, 0);
    float edge = SobelDepth(input.uv);
    color.rgb *= lerp(0.15, 1, edge);

    /*
    float3 normal = SAMPLE_TEXTURE2D_LOD(_NormalTexture, sampler_PointClamp, input.uv, 0);
    normal = (normal + 1) / 2; // [-1, 1] -> [0, 1]

    float3 hsv = RgbToHsv(normal);
    */

    return edge;
}
