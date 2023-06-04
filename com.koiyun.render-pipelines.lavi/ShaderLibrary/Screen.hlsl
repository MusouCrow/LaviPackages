#pragma once

#include "./Input.hlsl"

float4 _ScaledScreenParams; // x: width, y: height, z: 1 + 1.0 / width, w: 1 + 1.0 / height

#ifdef UNITY_UV_STARTS_AT_TOP
    #define SCALE_BIAS float3(-1, 1, -1)
#else
    #define SCALE_BIAS float3(1, 0, 1)
#endif

void TransformScreenUV(inout float2 uv, float screenHeight)
{
#if UNITY_UV_STARTS_AT_TOP
    float3 bias = SCALE_BIAS;
    uv.y = screenHeight - (uv.y * bias.x + bias.y * screenHeight);
#endif
}

void TransformScreenUV(inout float2 uv)
{
#if UNITY_UV_STARTS_AT_TOP
    TransformScreenUV(uv, _ScaledScreenParams.y);
#endif
}

void TransformNormalizedScreenUV(inout float2 uv)
{
#if UNITY_UV_STARTS_AT_TOP
    TransformScreenUV(uv, 1.0);
#endif
}

float2 GetNormalizedScreenSpaceUV(float2 positionCS)
{
    float2 normalizedScreenSpaceUV = positionCS.xy * rcp(_ScaledScreenParams.xy);
    TransformNormalizedScreenUV(normalizedScreenSpaceUV);
    return normalizedScreenSpaceUV;
}

float2 GetNormalizedScreenSpaceUV(float4 positionCS)
{
    return GetNormalizedScreenSpaceUV(positionCS.xy);
}

float4 ComputeScreenPos(float4 positionCS)
{
    float4 ndc = positionCS * 0.5f;
    ndc.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
    ndc.zw = positionCS.zw;

    return ndc;
}
