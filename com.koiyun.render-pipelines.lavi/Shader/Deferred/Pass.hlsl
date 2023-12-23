#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Shadow.hlsl"

TEXTURE2D(_GBufferColor);
TEXTURE2D(_DepthMap);
SAMPLER(sampler_PointClamp);

float4x4 _ScreenToWorld;

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
    float depth = SAMPLE_TEXTURE2D_LOD(_DepthMap, sampler_PointClamp, input.uv, 0);

    float4 positionWS = mul(_ScreenToWorld, float4(input.positionCS.xy, depth, 1));
    positionWS.xyz /= positionWS.w;

    float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
    float shadow = SAMPLE_SHADOW(_SceneShadowMap, shadowCoord);
    shadow = lerp(0.3, 1, shadow);

    return color * shadow;
}
